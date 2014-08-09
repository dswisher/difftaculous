
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
#if false
            foreach (JToken t in current)
            {
                foreach (JToken v in t)
                {
                    if (Expression.IsMatch(v))
                        yield return v;
                }
            }
#endif

            throw new NotImplementedException();
        }



        public override void AddJsonPath(StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }
}
