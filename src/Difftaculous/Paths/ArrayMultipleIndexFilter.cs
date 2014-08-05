
using System;
using System.Collections.Generic;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal class ArrayMultipleIndexFilter : PathFilter
    {
        public List<int> Indexes { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current)
        {
#if false
            foreach (JToken t in current)
            {
                foreach (int i in Indexes)
                {
                    JToken v = GetTokenIndex(t, errorWhenNoMatch, i);

                    if (v != null)
                        yield return v;
                }
            }
#endif
            throw new NotImplementedException();
        }
    }
}
