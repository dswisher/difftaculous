

namespace Difftaculous.Adapters
{
    internal class ZProperty : IProperty
    {
        public ZProperty(string name, IToken value)
        {
            Name = name;
            Value = value;
        }


        public string Name { get; private set; }
        public IToken Value { get; private set; }
    }
}
