using System;
using Xunit;

namespace ValueInjection.Test
{
    public class ValueInjectionEnumeratorTests
    {
        [Fact]
        public void ShouldThrowExceptionIfUnderlyingEnumeratorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionEnumerator(null));
        }

        [Fact]
        public void ShouldDelegateMoveNext()
        {
            var underlyingEnumerator = new TestEnumerator();
            var valueInjectionEnumerator = new ValueInjectionEnumerator(underlyingEnumerator);

            valueInjectionEnumerator.MoveNext();
            Assert.True(underlyingEnumerator.MoveNextCalled);
        }

        [Fact]
        public void ShouldDelegateReset()
        {
            var underlyingEnumerator = new TestEnumerator();
            var valueInjectionEnumerator = new ValueInjectionEnumerator(underlyingEnumerator);

            valueInjectionEnumerator.Reset();
            Assert.True(underlyingEnumerator.ResetCalled);
        }

        [Fact]
        public void ShouldDisposeUnderlyingEnumerator()
        {
            var underlyingEnumerator = new TestEnumerator();
            var valueInjectionEnumerator = new ValueInjectionEnumerator(underlyingEnumerator);

            valueInjectionEnumerator.Dispose();
            Assert.True(underlyingEnumerator.DisposeCalled);
        }
    }
}
