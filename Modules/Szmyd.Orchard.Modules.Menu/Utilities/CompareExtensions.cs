namespace Szmyd.Orchard.Modules.Menu.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CompareExtensions
    {
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first,
                                                           IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Except(second, new LambdaComparer<TSource>(comparer));
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first,
                                                          IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Union(second, new LambdaComparer<TSource>(comparer));
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first,
                                                              IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Intersect(second, new LambdaComparer<TSource>(comparer));
        }
    }
}