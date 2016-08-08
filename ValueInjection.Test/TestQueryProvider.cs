using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ValueInjection.Test
{
    class TestQueryProvider : IQueryProvider
    {
        public bool CreateQueryCalled { get; private set; }
        public IQueryable CreateQuery(Expression expression)
        {
            CreateQueryCalled = true;
            return ((IQueryable)(new List<object>().AsQueryable())).ToValueInjection();
        }

        public bool CreateQueryGenericCalled { get; private set; }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            CreateQueryGenericCalled = true;
            return new List<TElement>().AsQueryable();
        }

        public bool ExecuteCalled { get; private set; }
        public object Execute(Expression expression)
        {
            ExecuteCalled = true;
            return default(object);
        }

        public bool ExecuteGenericCalled { get; private set; }
        public TResult Execute<TResult>(Expression expression)
        {
            ExecuteGenericCalled = true;
            return default(TResult);
        }
    }
}
