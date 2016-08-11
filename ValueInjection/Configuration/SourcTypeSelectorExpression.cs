using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public class SourcTypeSelectorExpression<TDestination, TDestinationProperty, TSource> : ISourcTypeSelectorExpression<TDestination, TDestinationProperty, TSource>
    {
        private readonly Expression<Func<TDestination, TDestinationProperty>> _destinationPropertySelector;

        public SourcTypeSelectorExpression(Expression<Func<TDestination, TDestinationProperty>> destinationPropertySelector)
        {
            _destinationPropertySelector = destinationPropertySelector;
        }

        public ISourcePropertySelectorExpression<TDestination, TDestinationProperty, TSource, TSourceProperty> Property<TSourceProperty>(Expression<Func<TSource, TSourceProperty>> sourcePropertySelector)
        {
            return new SourcePropertySelectorExpression<TDestination, TDestinationProperty, TSource, TSourceProperty>(_destinationPropertySelector, sourcePropertySelector);
        }
    }
}