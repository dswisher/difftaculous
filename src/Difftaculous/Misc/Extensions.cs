
using System;
using System.Collections.Generic;
using System.Linq;


namespace Difftaculous.Misc
{
    internal static class Extensions
    {
        // See http://stackoverflow.com/questions/5489987/linq-full-outer-join
        internal static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
            this IEnumerable<TA> a,
            IEnumerable<TB> b,
            Func<TA, TK> selectKeyA,
            Func<TB, TK> selectKeyB,
            Func<TA, TB, TK, TR> projection,
            TA defaultA = default(TA),
            TB defaultB = default(TB),
            IEqualityComparer<TK> cmp = null)
        {
            cmp = cmp ?? EqualityComparer<TK>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = b.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       from xa in alookup[key].DefaultIfEmpty(defaultA)
                       from xb in blookup[key].DefaultIfEmpty(defaultB)
                       select projection(xa, xb, key);

            return join.ToList();
        }
    }
}
