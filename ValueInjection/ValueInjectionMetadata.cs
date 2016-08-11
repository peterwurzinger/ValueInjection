using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ValueInjection
{
    [Serializable]
    public class ValueInjectionMetadata
    {
        //The property which's value will be overridden
        internal PropertyInfo DestinationProperty { get; set; }

        //The property which's value will be used as key to find the convenient Object 
        internal PropertyInfo KeyProperty { get; set; }

        //The Property of the source type to read value from
        internal PropertyInfo SourceProperty { get; set; }

        internal Type SourceType { get; set; }

        public ValueInjectionMetadata()
        {
            
        }

        internal ValueInjectionMetadata(PropertyInfo destinationProperty, PropertyInfo keyProperty, Type sourceType)
        {
            DestinationProperty = destinationProperty;
            KeyProperty = keyProperty;
            SourceType = sourceType;
        }

        internal ValueInjectionMetadata(PropertyInfo destinationProperty, PropertyInfo keyProperty, PropertyInfo sourceProperty)
        {
            DestinationProperty = destinationProperty;
            KeyProperty = keyProperty;
            SourceProperty = sourceProperty;
            SourceType = sourceProperty.ReflectedType;
        }

        internal static ValueInjectionMetadata FromExpression
            <TDestination, TDestinationProperty, TSource, TSourceProperty, TDestinationKey>(
            Expression<Func<TDestination, TDestinationProperty>> destinationPropertySelector,
            Expression<Func<TSource, TSourceProperty>> sourcePropertySelector,
            Expression<Func<TDestination, TDestinationKey>> keySelector)
        {
            var destinationProperty = ((PropertyInfo)(destinationPropertySelector.Body as MemberExpression).Member);
            var sourceProperty = ((PropertyInfo)(sourcePropertySelector.Body as MemberExpression).Member);
            var keyProperty = ((PropertyInfo)(keySelector.Body as MemberExpression).Member);

            return new ValueInjectionMetadata(destinationProperty, keyProperty, sourceProperty);
        }

        internal static ValueInjectionMetadata FromExpression
    <TDestination, TDestinationProperty, TSource, TDestinationKey>(
    Expression<Func<TDestination, TDestinationProperty>> destinationPropertySelector,
    Expression<Func<TDestination, TDestinationKey>> keySelector)
        {
            var destinationProperty = ((PropertyInfo)(destinationPropertySelector.Body as MemberExpression).Member);
            var keyProperty = ((PropertyInfo)(keySelector.Body as MemberExpression).Member);

            return new ValueInjectionMetadata(destinationProperty, keyProperty, typeof(TSource));
        }
    }
}
