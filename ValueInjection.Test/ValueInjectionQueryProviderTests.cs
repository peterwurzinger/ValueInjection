using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ValueInjection.Test.Data;
using Xunit;

namespace ValueInjection.Test
{
    public class ValueInjectionQueryProviderTests
    {
        [Fact]
        public void ConstructorShouldThrowExceptionIfUnderlyingProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionQueryProvider(null));
        }

        [Fact]
        public void CreateQueryShouldReturnRightQueryType()
        {
            var query = new List<TestData>().AsQueryable().ToValueInjection();

            Assert.IsType<ValueInjectionQuery>(query.Provider.CreateQuery(query.Expression));
        }

        [Fact]
        public void CreateQueryShouldDelegateQueryCreation()
        {
            var testProvider = new TestQueryProvider();
            var provider = new ValueInjectionQueryProvider(testProvider);

            provider.CreateQuery(default(Expression));

            Assert.True(testProvider.CreateQueryCalled);
        }

        [Fact]
        public void ExecuteShouldDelegateQueryExecution()
        {
            var testProvider = new TestQueryProvider();
            var provider = new ValueInjectionQueryProvider(testProvider);

            provider.Execute(default(Expression));

            Assert.True(testProvider.ExecuteCalled);
        }

        [Fact]
        public void CreateQueryGenericShouldReturnRightQueryType()
        {
            var query = new List<TestData>().AsQueryable().ToValueInjection();

            Assert.IsType<ValueInjectionQuery<TestData>>(query.Provider.CreateQuery<TestData>(query.Expression));
        }

        [Fact]
        public void CreateQueryGenericShouldDelegateQueryCreation()
        {
            var testProvider = new TestQueryProvider();
            var provider = new ValueInjectionQueryProvider(testProvider);

            provider.CreateQuery<TestData>(default(Expression));

            Assert.True(testProvider.CreateQueryGenericCalled);
        }

        [Fact]
        public void ExecuteGenericShouldDelegateQueryExecution()
        {
            var testProvider = new TestQueryProvider();
            var provider = new ValueInjectionQueryProvider(testProvider);

            provider.Execute<TestData>(default(Expression));

            Assert.True(testProvider.ExecuteGenericCalled);
        }
    }
}
