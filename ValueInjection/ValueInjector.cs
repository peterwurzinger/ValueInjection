using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ValueInjection
{
    //TODO: Make it non-static
    //TODO: Make it Thread-safe
    public static class ValueInjector
    {
        private static readonly IDictionary<Type, List<ValueInjectionMetadata>> MetadataCache = new Dictionary<Type, List<ValueInjectionMetadata>>();

        //Cache Lookup-Values by source type and key
        private static readonly IDictionary<Tuple<Type, object>, object> ValueCache = new Dictionary<Tuple<Type, object>, object>();
        private static readonly IDictionary<Type, IValueObtainer> ValueObtainers = new Dictionary<Type, IValueObtainer>();

        public static void UseValueObtainer<TLookupType>(IValueObtainer<TLookupType> valueObtainer)
        {
            ValueObtainers[typeof(TLookupType)] = valueObtainer;
        }

        public static void InjectValues(object @object, bool recursive = true)
        {
            //Do not throw exception, null values are fine
            if (@object == null)
                return;

            var objectType = @object.GetType();

            if (recursive)
            {
                //TODO: Could asynchonous operations speed up the traversal?

                //Recursively analyze reference properties
                foreach (var referenceProperty in objectType.GetProperties().Where(p => p.CanRead
                        && (!p.PropertyType.IsValueType
                        && p.PropertyType != typeof(string)
                        && p.PropertyType != typeof(Enum))
                    ))
                {
                    if (typeof(IEnumerable).IsAssignableFrom(referenceProperty.PropertyType))
                    {
                        //TODO: See if element type is convenient as shown above (not string, Enum,...)
                        var enumerable = referenceProperty.GetValue(@object);
                        if (enumerable != null)
                        {
                            foreach (var element in enumerable as IEnumerable)
                                InjectValues(element);
                        }
                    }
                    else
                        InjectValues(referenceProperty.GetValue(@object));
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
                    if (lookupObject == null)
                        throw new InvalidOperationException("Obtained value is null!");

                    ValueCache[cacheAccessor] = lookupObject;
                }
                var value = metadata.SourceProperty.GetValue(lookupObject);
                //2.2 Set obtained value
                metadata.DestinationProperty.SetValue(@object, value);
            }
        }

        private static IEnumerable<ValueInjectionMetadata> GetOrAddMetadata(Type type)
        {
            if (MetadataCache.ContainsKey(type))
                return MetadataCache[type];

            var properties = type
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ValueInjectionAttribute)));

            var metadataList = new List<ValueInjectionMetadata>();

            foreach (var destinationProperty in properties)
            {
                if (!destinationProperty.CanWrite)
                    throw new InvalidOperationException($"Cannot write marked property {destinationProperty.Name}!");

                var attr = destinationProperty.GetCustomAttribute<ValueInjectionAttribute>();
                var keyProperty = type.GetProperty(attr.KeyPropertyName);
                if (keyProperty == null)
                    throw new InvalidOperationException($"Property {attr.KeyPropertyName} marked as Key does not exist!");

                var sourceProperty = attr.SourceType.GetProperty(attr.SourcePropertyName);
                if (sourceProperty == null)
                    throw new InvalidOperationException($"Source Property {attr.SourcePropertyName} does not exist in Type {attr.SourceType.Name}!");

                if (!sourceProperty.CanRead)
                    throw new InvalidOperationException($"Cannot read Sorce Property {sourceProperty.Name} in Type {attr.SourceType.Name}!");

                if (!ValueObtainers.ContainsKey(attr.SourceType))
                    throw new NotSupportedException($"Lookup for Type {attr.SourceType.Name} is not supported!");

                metadataList.Add(new ValueInjectionMetadata(destinationProperty, keyProperty, attr.SourceType, sourceProperty));
            }
            MetadataCache[type] = metadataList;
            return metadataList;
        }

        public static void Clear()
        {
            ValueCache.Clear();
            ValueObtainers.Clear();
            MetadataCache.Clear();
        }
    }
}
