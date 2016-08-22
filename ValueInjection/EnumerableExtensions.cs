using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ValueInjection
{
    public static class EnumerableExtensions
    {
        public static IQueryable<T> ToValueInjection<T>(this IQueryable<T> query, bool recursiveInjection = true)
        {
            return new ValueInjectionQuery<T>(query, recursiveInjection);
        }

        public static IQueryable ToValueInjection(this IQueryable query, bool recursiveInjection = true)
        {
            return new ValueInjectionQuery(query, recursiveInjection);
        }

        public static IOrderedQueryable<T> ToValueInjection<T>(this IOrderedQueryable<T> query, bool recursiveInjection = true)
        {
            return new ValueInjectionQuery<T>(query, recursiveInjection);
        }

        public static IOrderedQueryable ToValueInjection(this IOrderedQueryable query, bool recursiveInjection = true)
        {
            return new ValueInjectionQuery(query, recursiveInjection);
        }

        public static IEnumerable<T> ToValueInjection<T>(this IEnumerable<T> enumerable, bool recursiveInjection = true)
        {
            return new ValueInjectionEnumerable<T>(enumerable, recursiveInjection);
        }

        public static IEnumerable ToValueInjection(this IEnumerable enumerable, bool recursiveInjection = true)
        {
            return new ValueInjectionEnumerable(enumerable, recursiveInjection);
        }
    }
}