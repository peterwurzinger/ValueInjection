using System;
using System.Linq;
using System.Linq.Expressions;

namespace ValueInjection
{
    public class ValueInjectionQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider _provider;

        public ValueInjectionQueryProvider(IQueryProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _provider = provider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new ValueInjectionQuery(_provider.CreateQuery(expression));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new ValueInjectionQuery<TElement>(_provider.CreateQuery<TElement>(expression));
        }

        public object Execute(Expression expression)
        {
            return _provider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _provider.Execute<TResult>(expression);
        }
    }
}
