using System;
using System.Collections;
using System.Collections.Generic;
using ValueInjection.Test.Data;
using Xunit;

namespace ValueInjection.Test
{
    public class ValueInjectionEnumerableTests
    {
        [Fact]
        public void ShouldThrowExceptionIfEnumerableIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionEnumerable(null));
        }

        [Fact]
        public void ShouldReturnRightEnumerator()
        {
            var data = new ArrayList().ToValueInjection();

            Assert.IsType<ValueInjectionEnumerator>(data.GetEnumerator());
        }

        [Fact]
        public void ShouldThrowExceptionIfEnumerableIsNullGeneric()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionEnumerable<TestData>(null));
        }

        [Fact]
        public void ShouldReturnRightEnumeratorGeneric()
        {
            var data = new List<TestData>().ToValueInjection();

            Assert.IsType<ValueInjectionEnumerator<TestData>>(data.GetEnumerator());
        }

        [Fact]
        public void ShouldReturnOverridenEnumeratorGeneric()
        {
            var data = new ValueInjectionEnumerable<TestData>(new List<TestData>());

            Assert.IsType<ValueInjectionEnumerator<TestData>>(((IEnumerable) data).GetEnumerator());
        }
    }
}
