using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public interface ISourcTypeSelectorExpression<TDestination, TDestinationProperty, TSource>
    {
        //TODO: Injection of whole objects not supported yet
        //void FromKey();

        ISourcePropertySelectorExpression<TDestination, TDestinationProperty, TSource, TSourceProperty> Property<TSourceProperty>(Expression<Func<TSource, TSourceProperty>> sourcePropertySelector);
    }
}