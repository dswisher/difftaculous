

namespace Difftaculous.ZModel
{
    internal class ZValue : ZToken
    {
        public ZValue(object value)
        {
            Value = value;
        }

        public override TokenType Type { get { return TokenType.Value; } }

        public object Value { get; private set; }
    }
}
