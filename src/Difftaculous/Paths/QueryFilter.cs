
using System;
using System.Collections.Generic;
using System.Text;
using Difftaculous.Paths.Expressions;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal class QueryFilter : PathFilter
    {
        public QueryExpression Expression { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current, bool errorWhenNoMatch)
        {
            foreach (ZToken t in current)
            {
                foreach (ZToken v in t)
                {
                    if (Expression.IsMatch(v))
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
