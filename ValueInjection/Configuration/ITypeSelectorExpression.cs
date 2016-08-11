using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public interface ITypeSelectorExpression<TDestination>
    {
        IDestinationPropertySelectorExpression<TDestination, TDestinationProperty> Of<TDestinationProperty>(Expression<Func<TDestination, TDestinationProperty>> destinationPropertySelector);
    }
}
