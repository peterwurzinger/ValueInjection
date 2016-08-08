using System;
using System.Collections;
using System.Collections.Generic;

namespace ValueInjection
{
    public class ValueInjectionEnumerable : IEnumerable
    {
        private readonly IEnumerable _enumerable;
        protected readonly bool RecursiveInjection;

        public ValueInjectionEnumerable(IEnumerable enumerable, bool recursiveInjection = true)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            _enumerable = enumerable;
            RecursiveInjection = recursiveInjection;
        }

        public IEnumerator GetEnumerator()
        {
            return new ValueInjectionEnumerator(_enumerable.GetEnumerator(), RecursiveInjection);
        }
    }

    public class ValueInjectionEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;
        protected readonly bool RecursiveInjection;

        public ValueInjectionEnumerable(IEnumerable<T> enumerable, bool recursiveInjection = true)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            _enumerable = enumerable;
            RecursiveInjection = recursiveInjection;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ValueInjectionEnumerator<T>(_enumerable.GetEnumerator(), RecursiveInjection);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
