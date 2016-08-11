using System;
using System.Linq.Expressions;

namespace ValueInjection.Configuration
{
    public class SourcePropertySelectorExpression<TTarget, TTargetProperty, TSource, TSourceProperty> : ISourcePropertySelectorExpression<TTarget, TTargetProperty, TSource, TSourceProperty>
    {
        private readonly Expression<Func<TSource, TSourceProperty>> _sourcePropertySelector;
        private readonly Expression<Func<TTarget, TTargetProperty>> _destinationPropertySelector;

        public SourcePropertySelectorExpression(Expression<Func<TTarget, TTargetProperty>> destinationPropertySelector, Expression<Func<TSource, TSourceProperty>> sourcePropertySelector)
        {
            _destinationPropertySelector = destinationPropertySelector;
            _sourcePropertySelector = sourcePropertySelector;
        }

        public ValueInjectionMetadata FromKey<TTargetKey>(Expression<Func<TTarget, TTargetKey>> keySelectorExpression)
        {
            return ValueInjectionMetadata.FromExpression(_destinationPropertySelector, _sourcePropertySelector,
                keySelectorExpression);
        }
    }
}