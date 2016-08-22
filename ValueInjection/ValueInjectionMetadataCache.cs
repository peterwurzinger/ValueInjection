using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ValueInjection
{
    internal class ValueInjectionMetadataCache : IValueInjectionMetadataCache
    {
        //Cache Metadata by destination type
        private readonly IDictionary<Type, ISet<ValueInjectionMetadata>> _metadataCache = new ConcurrentDictionary<Type, ISet<ValueInjectionMetadata>>();

        public void UseMetadata(Type type, ValueInjectionMetadata metadata)
        {
            if (!_metadataCache.ContainsKey(type))
                _metadataCache[type] = new HashSet<ValueInjectionMetadata>();
            _metadataCache[type].Add(metadata);
        }

        public void UseMetadata<TDestination>(ValueInjectionMetadata metadata)
        {
            UseMetadata(typeof (TDestination), metadata);
        }

        public ISet<ValueInjectionMetadata> GetOrAddMetadata<TDestination>()
        {
            return GetOrAddMetadata(typeof(TDestination));
        }

        public ISet<ValueInjectionMetadata> GetOrAddMetadata(Type type)
        {
            //Do not analyze type if metadata already present in cache
            if (_metadataCache.ContainsKey(type))
                return _metadataCache[type];

            //TODO: Use a semaphore to make write-access to cache exclusive to one thread/task
            _metadataCache[type] = ValueInjectionMetadataBuilder.BuildMetadataForType(type);
            return _metadataCache[type];
        }

        public void Clear()
        {
            _metadataCache.Clear();
        }
    }
}
