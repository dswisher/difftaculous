

using Difftaculous.Adapters;

namespace Difftaculous.ZModel
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
