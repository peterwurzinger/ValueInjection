using System;
using ValueInjection.Configuration;

namespace ValueInjection
{
    public static class MetadataBuilder
    {
        public static ITypeSelectorExpression<TTarget> Replacement<TTarget>()
        {
            return new TypeSelectorExpression<TTarget>();
        }

        public static void ConfigureReplacement<TDestination>(Func<ITypeSelectorExpression<TDestination>, ValueInjectionMetadata> configExpression)
        {
            ValueInjector.UseMetadata<TDestination>(configExpression(new TypeSelectorExpression<TDestination>()));
        }
    }
}
