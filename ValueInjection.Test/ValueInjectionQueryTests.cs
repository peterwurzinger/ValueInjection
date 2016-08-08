using System;
using System.Collections.Generic;
using System.Linq;
using ValueInjection.Test.Data;
using Xunit;

namespace ValueInjection.Test
{
    public class ValueInjectionQueryTests
    {
        [Fact]
        public void ShouldReturnUnderlyingQueryProperties()
        {
            var enumerableQuery = ((IQueryable)(new List<object>().AsQueryable())).ToValueInjection();
            var injectedQuery = enumerableQuery.ToValueInjection();

            Assert.Equal(enumerableQuery.ElementType, injectedQuery.ElementType);
            Assert.Equal(enumerableQuery.Expression, injectedQuery.Expression);
        }

        [Fact]
        public void ShouldCreateRightQueryProvider()
        {
            var enumerableQuery = ((IQueryable)(new List<object>().AsQueryable())).ToValueInjection();

            Assert.IsType<ValueInjectionQueryProvider>(enumerableQuery.Provider);
        }

        [Fact]
        public void ShouldThrowExceptionIfQueryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionQuery(null));
        }

        [Fact]
        public void ShouldReturnUnderlyingQueryPropertiesGeneric()
        {
            var enumerableQuery = new List<TestData>().AsQueryable();
            var injectedQuery = enumerableQuery.ToValueInjection();

            Assert.Equal(enumerableQuery.ElementType, injectedQuery.ElementType);
            Assert.Equal(enumerableQuery.Expression, injectedQuery.Expression);
        }

        [Fact]
        public void ShouldCreateRightQueryProviderGeneric()
        {
            var enumerableQuery = new List<TestData>().AsQueryable().ToValueInjection();

            Assert.IsType<ValueInjectionQueryProvider>(enumerableQuery.Provider);
        }

        [Fact]
        public void ShouldThrowExceptionIfQueryIsNullGeneric()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionQuery<TestData>(null));
        }
    }
}
