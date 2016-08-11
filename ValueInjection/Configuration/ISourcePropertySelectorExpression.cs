using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public interface ISourcePropertySelectorExpression<TDestination, TDestinationProperty, TSource, TSourceProperty>
    {
        ValueInjectionMetadata FromKey<TDestinationKey>(Expression<Func<TDestination, TDestinationKey>> keySelectorExpression);
    }
}