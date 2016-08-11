using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public class TypeSelectorExpression<TDestination> : ITypeSelectorExpression<TDestination>
    {
        public IDestinationPropertySelectorExpression<TDestination, TDestinationProperty> Of<TDestinationProperty>(Expression<Func<TDestination, TDestinationProperty>> destinationPropertySelector)
        {
            return new DestinationPropertySelectorExpression<TDestination, TDestinationProperty>(destinationPropertySelector);
        }
    }
}