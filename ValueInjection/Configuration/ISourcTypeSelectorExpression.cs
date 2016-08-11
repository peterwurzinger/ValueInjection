using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public interface ISourcTypeSelectorExpression<TDestination, TDestinationProperty, TSource>
    {
        ValueInjectionMetadata FromKey<TDestinationKey>(Expression<Func<TDestination, TDestinationKey>> keySelectorExpression);

        ISourcePropertySelectorExpression<TDestination, TDestinationProperty, TSource, TSourceProperty> Property<TSourceProperty>(Expression<Func<TSource, TSourceProperty>> sourcePropertySelector);
    }
}