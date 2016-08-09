using System;
using System.Collections;
using System.Collections.Generic;

namespace ValueInjection.Test.Data
{
    public class StringEnumerableChildPropertyTestData
    {
        //The idea is to let the Framework get the value of the enumerable, but prevent to enumerate it,
        //which it would do if it identifies an injectable type
        public IEnumerable<string> StringEnumerable => new ExceptionThrowingEnumerable<string>();
    }

    public class ExceptionThrowingEnumerable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return new ExceptionThrowingEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ExceptionThrowingEnumerator<T> : IEnumerator<T>
    {
        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            return true;
        }

        public void Reset()
        {

        }

        public T Current { get { throw new InvalidOperationException(); } }

        object IEnumerator.Current => Current;
    }
}
