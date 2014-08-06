
using System;
using System.Collections.Generic;
using Difftaculous.ZModel;


namespace Difftaculous.Paths.Expressions
{
    internal class CompositeExpression : QueryExpression
    {
        public List<QueryExpression> Expressions { get; set; }

        public CompositeExpression()
        {
            Expressions = new List<QueryExpression>();
        }


        public override bool IsMatch(IToken t)
        {
#if false
            switch (Operator)
            {
                case QueryOperator.And:
                    foreach (QueryExpression e in Expressions)
                    {
                        if (!e.IsMatch(t))
                            return false;
                    }
                    return true;
                case QueryOperator.Or:
                    foreach (QueryExpression e in Expressions)
                    {
                        if (e.IsMatch(t))
                            return true;
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endif
            throw new NotImplementedException();
        }
    }
}
