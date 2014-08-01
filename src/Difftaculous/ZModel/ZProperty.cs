

namespace Difftaculous.ZModel
{
    internal class ZProperty : ZToken, IProperty
    {
        public ZProperty(string name, IToken value)
        {
            Name = name;
            Value = value;

            ((ZToken)Value).Parent = this;
        }

        public override TokenType Type { get { return TokenType.Property; } }

        public string Name { get; private set; }
        public IToken Value { get; private set; }
    }
}
