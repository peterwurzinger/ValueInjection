using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ValueInjection
{
    //TODO: Make it non-static
    //TODO: Make it Thread-safe (?)
    public static class ValueInjector
    {
        //Cache Metadata by destination type
        private static readonly IDictionary<Type, List<ValueInjectionMetadata>> MetadataCache = new ConcurrentDictionary<Type, List<ValueInjectionMetadata>>();

        //Cache Lookup-Values by source type and key
        private static readonly IDictionary<Tuple<Type, object>, object> ValueCache = new ConcurrentDictionary<Tuple<Type, object>, object>();
        private static readonly IDictionary<Type, IValueObtainer> ValueObtainers = new ConcurrentDictionary<Type, IValueObtainer>();

        private static readonly ISet<Type> NotInjectableTypes = new HashSet<Type> { typeof(string), typeof(Enum) };

        public static void UseValueObtainer<TLookupType>(IValueObtainer<TLookupType> valueObtainer)
        {
            ValueObtainers[typeof(TLookupType)] = valueObtainer;
        }

        public static T InjectValues<T>(T @object, bool recursive = true)
        {
            return (T)InjectValues((object) @object, recursive);
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
                        tasks.Add(Task.Factory.StartNew(() => InjectValues(referenceProperty.GetValue(@object))));
                    }
                }
            }

            //1.  Determine Properties
            //1.1 Fill or use Cache
            var metadatas = GetOrAddMetadata(objectType);

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

        internal static void UseMetadata<TDestination>(ValueInjectionMetadata metadata)
        {
            if (!MetadataCache.ContainsKey(typeof(TDestination)))
                MetadataCache[typeof(TDestination)] = new List<ValueInjectionMetadata>();
            MetadataCache[typeof(TDestination)].Add(metadata);
        }

        private static IEnumerable<ValueInjectionMetadata> GetOrAddMetadata(Type type)
        {
            if (MetadataCache.ContainsKey(type))
                return MetadataCache[type];

            var metadataList = type.GetInterfaces().SelectMany(GetOrAddMetadata).ToList();
            if (type.BaseType != null)
                metadataList.AddRange(GetOrAddMetadata(type.BaseType));

            var properties = type
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ValueInjectionAttribute)));

            foreach (var destinationProperty in properties)
            {
                var metadata = new ValueInjectionMetadata();

                if (!destinationProperty.CanWrite)
                    throw new InvalidOperationException($"Cannot write marked property {destinationProperty.Name}!");

                metadata.DestinationProperty = destinationProperty;

                var attr = destinationProperty.GetCustomAttribute<ValueInjectionAttribute>();
                var keyProperty = type.GetProperty(attr.KeyPropertyName);
                if (keyProperty == null)
                    throw new InvalidOperationException($"Property {attr.KeyPropertyName} marked as Key does not exist!");

                metadata.KeyProperty = keyProperty;

                if (attr.SourcePropertyName != null && attr.SourceType != null)
                {
                    var sourceProperty = attr.SourceType.GetProperty(attr.SourcePropertyName);
                    if (sourceProperty == null)
                        throw new InvalidOperationException(
                            $"Source Property {attr.SourcePropertyName} does not exist in Type {attr.SourceType.Name}!");

                    if (!sourceProperty.CanRead)
                        throw new InvalidOperationException(
                            $"Cannot read Sorce Property {sourceProperty.Name} in Type {attr.SourceType.Name}!");

                    metadata.SourceProperty = sourceProperty;
                    metadata.SourceType = sourceProperty.ReflectedType;
                }
                else
                {
                    //Obtain source type by type of destination property
                    metadata.SourceType = destinationProperty.PropertyType;
                }

                if (!ValueObtainers.ContainsKey(metadata.SourceType))
                    throw new NotSupportedException($"Lookup for Type {metadata.SourceType?.Name} is not supported!");

                metadataList.Add(metadata);
            }
            MetadataCache[type] = metadataList;
            return metadataList;
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
            MetadataCache.Clear();
        }
    }
}
