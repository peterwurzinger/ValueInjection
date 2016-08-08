using System;

namespace ValueInjection
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueInjectionAttribute : Attribute
    {
        internal string KeyPropertyName { get; private set; }
        internal Type SourceType { get; private set; }
        internal string SourcePropertyName { get; private set; }

        public ValueInjectionAttribute(Type sourceType, string keyPropertyName, string sourcePropertyName)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (string.IsNullOrEmpty(sourcePropertyName))
                throw new ArgumentNullException(nameof(sourcePropertyName));

            if (string.IsNullOrEmpty(keyPropertyName))
                throw new ArgumentNullException(nameof(keyPropertyName));

            KeyPropertyName = keyPropertyName;
            SourceType = sourceType;
            SourcePropertyName = sourcePropertyName;
        }
    }
}