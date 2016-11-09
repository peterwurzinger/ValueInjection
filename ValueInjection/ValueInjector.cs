using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueInjection
{
    //TODO: Make it non-static
    public static class ValueInjector
    {
        internal static readonly IValueInjectionMetadataCache MetadataCache = new ValueInjectionMetadataCache();

        //Cache Lookup-Values by source type and key
        private static readonly IDictionary<Tuple<Type, object>, object> ValueCache = new ConcurrentDictionary<Tuple<Type, object>, object>();
        private static readonly IDictionary<Type, IValueObtainer> ValueObtainers = new ConcurrentDictionary<Type, IValueObtainer>();

        private static readonly ISet<Type> NotInjectableTypes = new HashSet<Type> { typeof(string), typeof(Enum) };

        public static void UseValueObtainer<TLookupType>(IValueObtainer<TLookupType> valueObtainer)
        {
            ValueObtainers[typeof(TLookupType)] = valueObtainer;
        }

        public static T InjectValues<T>(T @object, bool recursive = true)
            where T : class
        {
            return (T)InjectValues((object)@object, recursive);
        }

        public static object InjectValues(object @object, bool recursive = true)
        {
            //Do not throw exception, null values are fine
            if (@object == null)
                return null;

            var tasks = new List<Task>();

            var objectType = @object.GetType();

            if (recursive)
            {

                //Recursively analyze reference properties
                foreach (var referenceProperty in objectType.GetProperties().Where(p => p.CanRead
                        && (!p.PropertyType.IsValueType
                        && !NotInjectableTypes.Contains(p.PropertyType))))
                {
                    //Handle reference properties of enumerable type
                    if (typeof(IEnumerable).IsAssignableFrom(referenceProperty.PropertyType))
                    {
                        var enumerableValue = (IEnumerable)referenceProperty.GetValue(@object);
                        if (enumerableValue != null)
                        {
                            var enumerableType = GetEnumerableType(enumerableValue);

                            //If target enumerable implements IEnumerable<T> see if <T> is an injectable type
                            if (enumerableType == null || NotInjectableTypes.Contains(enumerableType))
                                continue;

                            tasks.AddRange(from object element
                                           in enumerableValue
                                           select Task.Factory.StartNew(() => InjectValues(element)));
                        }
                    }
                    else
                    {
                        if (MetadataCache.GetOrAddMetadata(referenceProperty.PropertyType).Any())
                            tasks.Add(Task.Factory.StartNew(() => InjectValues(referenceProperty.GetValue(@object))));
                    }
                }
            }

            //1.  Determine Properties
            //1.1 Fill or use Cache
            var metadatas = MetadataCache.GetOrAddMetadata(objectType);

            //1.2 See if there are desired source types without corresponding value obtainer
            var notExistingTypes = metadatas.Select(m => m.SourceType).Except(ValueObtainers.Keys);
            if (notExistingTypes.Any())
                throw new NotSupportedException($"Lookup for following Types is not supported: {string.Join(",", notExistingTypes.Select(t => t.FullName))}");

            //2. Inject values
            foreach (var metadata in metadatas)
            {
                //2.1 Obtain Lookup-Value from Key-Property
                var key = metadata.KeyProperty.GetValue(@object);

                if (key == null)
                    throw new InvalidOperationException($"Value of Key-Property {metadata.KeyProperty.Name} is null!");

                var cacheAccessor = Tuple.Create(metadata.SourceType, key);

                object lookupObject;
                if (ValueCache.ContainsKey(cacheAccessor))
                    lookupObject = ValueCache[cacheAccessor];
                else
                {
                    lookupObject = ValueObtainers[metadata.SourceType].ObtainValue(key);

                    //TODO: This might be OK in some cases
                    if (lookupObject == null)
                        throw new InvalidOperationException("Obtained value is null!");

                    ValueCache[cacheAccessor] = lookupObject;
                }

                var value = metadata.SourceProperty != null
                    ? metadata.SourceProperty.GetValue(lookupObject)
                    : lookupObject;

                //2.2 Set obtained value
                metadata.DestinationProperty.SetValue(@object, value);
            }
            Task.WaitAll(tasks.ToArray());

            return @object;
        }

        private static Type GetEnumerableType(object enumerableObject)
        {
            //Dertermine type by IEnumerable<T>-Implementation
            //IEnumerable is not supported
            return enumerableObject.GetType()
                .GetInterfaces()
                .Where(f => f.IsGenericType
                            && f.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(f => f.GenericTypeArguments[0])
                .FirstOrDefault();
        }

        public static void Clear()
        {
            ValueCache.Clear();
            ValueObtainers.Clear();
        }
    }
}
