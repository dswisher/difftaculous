
using Difftaculous.ZModel;


namespace Difftaculous.Paths.Expressions
{
    internal enum QueryOperator
    {
        None,
        Equals,
        NotEquals,
        Exists,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
        And,
        Or
    }


    internal abstract class QueryExpression
    {
        public QueryOperator Operator { get; set; }

        public abstract bool IsMatch(ZToken t);
    }
}
