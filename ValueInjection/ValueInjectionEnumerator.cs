using System;
using System.Collections;
using System.Collections.Generic;

namespace ValueInjection
{
    public class ValueInjectionEnumerator : IEnumerator, IDisposable
    {
        private readonly IEnumerator _underlyingEnumerator;
        protected readonly bool RecursiveInjection;

        public ValueInjectionEnumerator(IEnumerator underlyingEnumerator, bool recursiveInjection = true)
        {
            if (underlyingEnumerator == null)
                throw new ArgumentNullException(nameof(underlyingEnumerator));

            _underlyingEnumerator = underlyingEnumerator;
            RecursiveInjection = recursiveInjection;
        }

        public bool MoveNext()
        {
            return _underlyingEnumerator.MoveNext();
        }

        public void Reset()
        {
            _underlyingEnumerator.Reset();
        }

        private object _current;
        public object Current
        {
            get
            {
                var currentObject = _underlyingEnumerator.Current;
                if (_current == null || _current != currentObject)
                {
                    ValueInjector.InjectValues(currentObject, RecursiveInjection);
                    _current = currentObject;
                }
                return _current;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposableEnumerator = _underlyingEnumerator as IDisposable;
                disposableEnumerator?.Dispose();
            }
        }
    }

    public class ValueInjectionEnumerator<T> : ValueInjectionEnumerator, IEnumerator<T>
    {
        public ValueInjectionEnumerator(IEnumerator underlyingEnumerator, bool recursiveInjection = true) : base(underlyingEnumerator, recursiveInjection)
        {
        }

        //Simply cast the value of base.Current, since it should also experience the injection-magic
        public new T Current => (T)base.Current;
    }
}