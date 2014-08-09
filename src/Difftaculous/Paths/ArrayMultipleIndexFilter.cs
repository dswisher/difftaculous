
using System;
using System.Collections.Generic;
using System.Text;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal class ArrayMultipleIndexFilter : PathFilter
    {
        public List<int> Indexes { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current, bool errorWhenNoMatch)
        {
            foreach (ZToken t in current)
            {
                foreach (int i in Indexes)
                {
                    ZToken v = GetTokenIndex(t, errorWhenNoMatch, i);

                    if (v != null)
                        yield return v;
                }
            }
        }



        public override void AddJsonPath(StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }
}
