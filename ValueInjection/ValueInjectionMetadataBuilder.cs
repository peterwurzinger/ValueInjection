using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ValueInjection.Configuration;

namespace ValueInjection
{
    public static class ValueInjectionMetadataBuilder
    {
        public static ITypeSelectorExpression<TTarget> Replacement<TTarget>()
        {
            return new TypeSelectorExpression<TTarget>();
        }

        public static void ConfigureReplacement<TDestination>(Func<ITypeSelectorExpression<TDestination>, ValueInjectionMetadata> configExpression)
        {
            ValueInjector.MetadataCache.UseMetadata<TDestination>(configExpression(new TypeSelectorExpression<TDestination>()));
        }

        internal static ISet<ValueInjectionMetadata> BuildMetadataForType<T>()
        {
            return BuildMetadataForType(typeof(T));
        } 

        internal static ISet<ValueInjectionMetadata> BuildMetadataForType(Type type)
        {
            //Analyze implementing interfaces for injection
            var metadataSet = new HashSet<ValueInjectionMetadata>();
            foreach (var metadata in type.GetInterfaces().SelectMany(ValueInjector.MetadataCache.GetOrAddMetadata).ToList())
                metadataSet.Add(metadata);

            //Analyze base type, if present
            if (type.BaseType != null)
                foreach (var metadata in (ValueInjector.MetadataCache.GetOrAddMetadata(type.BaseType)))
                    metadataSet.Add(metadata);

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

                metadataSet.Add(metadata);
            }

            return metadataSet;
        }
    }
}
