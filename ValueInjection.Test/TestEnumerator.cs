using System;
using System.Collections;

namespace ValueInjection.Test
{
    public class TestEnumerator : IEnumerator, IDisposable
    {
        public bool MoveNextCalled { get; set; }
        public bool MoveNext()
        {
            MoveNextCalled = true;
            return true;
        }

        public bool ResetCalled { get; set; }
        public void Reset()
        {
            ResetCalled = true;
        }

        public object Current { get; set; }

        public bool DisposeCalled { get; set; }
        public void Dispose()
        {
            DisposeCalled = true;
        }
    }
}
