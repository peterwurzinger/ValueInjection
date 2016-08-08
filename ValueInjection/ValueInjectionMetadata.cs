using System;
using System.Reflection;

namespace ValueInjection
{
    [Serializable]
    internal class ValueInjectionMetadata
    {
        //The property which's value will be overridden
        internal PropertyInfo DestinationProperty { get; private set; }

        //The property which's value will be used as key to find the convenient Object 
        internal PropertyInfo KeyProperty { get; private set; }

        //The Source type (Bezirk, Gemeinde,...)
        internal Type SourceType { get; private set; }

        //The Property of the Type to read value from
        internal PropertyInfo SourceProperty { get; private set; }

        internal ValueInjectionMetadata(PropertyInfo destinationProperty, PropertyInfo keyProperty, Type sourceType, PropertyInfo sourceProperty)
        {
            DestinationProperty = destinationProperty;
            KeyProperty = keyProperty;
            SourceType = sourceType;
            SourceProperty = sourceProperty;
        }
    }
}
