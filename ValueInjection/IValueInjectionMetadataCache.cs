using System;
using System.Collections.Generic;

namespace ValueInjection
{
    public interface IValueInjectionMetadataCache
    {
        void UseMetadata(Type type, ValueInjectionMetadata metadata);
        void UseMetadata<TDestination>(ValueInjectionMetadata metadata);

        ISet<ValueInjectionMetadata> GetOrAddMetadata<TDestination>();
        ISet<ValueInjectionMetadata> GetOrAddMetadata(Type type);
    }
}
