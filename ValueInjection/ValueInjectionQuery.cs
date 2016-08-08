using System;
using System.Linq;
using System.Linq.Expressions;

namespace ValueInjection
{
    public class ValueInjectionQuery : ValueInjectionEnumerable, IOrderedQueryable
    {
        public Expression Expression { get; }
        public Type ElementType { get; }
        public IQueryProvider Provider { get; }

        public ValueInjectionQuery(IQueryable query, bool recursiveInjection = true) : base(query, recursiveInjection)
        {
            Provider = new ValueInjectionQueryProvider(query.Provider);
            Expression = query.Expression;
            ElementType = query.ElementType;
        }
    }

    public class ValueInjectionQuery<T> : ValueInjectionEnumerable<T>, IOrderedQueryable<T>
    {
        public Expression Expression { get; }
        public Type ElementType { get; }
        public IQueryProvider Provider { get; }

        public ValueInjectionQuery(IQueryable<T> query, bool recursiveInjection = true) : base(query, recursiveInjection)
        {
            Expression = query.Expression;
            ElementType = query.ElementType;
            Provider = new ValueInjectionQueryProvider(query.Provider);
        }
    }
}