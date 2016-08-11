namespace ValueInjection.Configuration
{
    public interface IDestinationPropertySelectorExpression<TDestination, TDestinationProperty>
    {
        ISourcTypeSelectorExpression<TDestination, TDestinationProperty, TSource> With<TSource>();
    }
}