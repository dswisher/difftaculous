

namespace Difftaculous.Adapters
{
    internal class ZValue : ZToken, IValue
    {
        public ZValue(object value)
        {
            Value = value;
        }


        public object Value { get; private set; }
    }
}
