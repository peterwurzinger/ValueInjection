using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ValueInjection.Test.Data;
using Xunit;

namespace ValueInjection.Test
{
    public class ValueInjectionExtensionsTests
    {
        [Fact]
        public void ShouldReturnRightEnumerableType()
        {
            var data = new ArrayList().ToValueInjection();

            Assert.IsType<ValueInjectionEnumerable>(data);
        }

        [Fact]
        public void ShouldReturnRightGenericEnumerableType()
        {
            var data = new List<TestData>().ToValueInjection();

            Assert.IsType<ValueInjectionEnumerable<TestData>>(data);
        }

        [Fact]
        public void ShouldReturnRightQueryableType()
        {
            var data = ((IQueryable)(new List<object>().AsQueryable())).ToValueInjection();

            Assert.IsType<ValueInjectionQuery>(data);
        }

        [Fact]
        public void ShouldReturnRightGenericQueryableType()
        {
            var data = new List<TestData>().AsQueryable().ToValueInjection();

            Assert.IsType<ValueInjectionQuery<TestData>>(data);
        }

        [Fact]
        public void ShouldReturnRightOrderedQueryableType()
        {
            var data = ((IOrderedQueryable)(new List<object>().AsQueryable().OrderBy(f => f))).ToValueInjection();

            Assert.IsType<ValueInjectionQuery>(data);
        }

        [Fact]
        public void ShouldReturnRightGenericOrderedQueryableType()
        {
            var data = new List<TestData>().AsQueryable().OrderBy(f => f).ToValueInjection();

            Assert.IsType<ValueInjectionQuery<TestData>>(data);
        }
    }
}
