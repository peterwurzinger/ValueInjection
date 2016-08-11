using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public class DestinationPropertySelectorExpression<TDestination, TDestinationProperty> : IDestinationPropertySelectorExpression<TDestination, TDestinationProperty>
    {
        private readonly Expression<Func<TDestination, TDestinationProperty>> _destinationPropertySelector;

        public DestinationPropertySelectorExpression(Expression<Func<TDestination, TDestinationProperty>> destinationPropertySelector)
        {
            _destinationPropertySelector = destinationPropertySelector;
        }

        public ISourcTypeSelectorExpression<TDestination, TDestinationProperty, TSource> With<TSource>()
        {
            return new SourcTypeSelectorExpression<TDestination, TDestinationProperty, TSource>(_destinationPropertySelector);
        }
    }
}