
using System;
using System.Collections.Generic;
using Difftaculous.ZModel;


namespace Difftaculous.Paths.Expressions
{
    internal class BooleanQueryExpression : QueryExpression
    {
        public List<PathFilter> Path { get; set; }
        public IValue Value { get; set; }

        public override bool IsMatch(IToken t)
        {
#if false
            IEnumerable<JToken> pathResult = JPath.Evaluate(Path, t, false);

            foreach (JToken r in pathResult)
            {
                JValue v = r as JValue;
                switch (Operator)
                {
                    case QueryOperator.Equals:
                        if (v != null && v.Equals(Value))
                            return true;
                        break;
                    case QueryOperator.NotEquals:
                        if (v != null && !v.Equals(Value))
                            return true;
                        break;
                    case QueryOperator.GreaterThan:
                        if (v != null && v.CompareTo(Value) > 0)
                            return true;
                        break;
                    case QueryOperator.GreaterThanOrEquals:
                        if (v != null && v.CompareTo(Value) >= 0)
                            return true;
                        break;
                    case QueryOperator.LessThan:
                        if (v != null && v.CompareTo(Value) < 0)
                            return true;
                        break;
                    case QueryOperator.LessThanOrEquals:
                        if (v != null && v.CompareTo(Value) <= 0)
                            return true;
                        break;
                    case QueryOperator.Exists:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
#endif

            throw new NotImplementedException();
        }
    }
}
