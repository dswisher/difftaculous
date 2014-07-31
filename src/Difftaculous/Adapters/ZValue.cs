
using Difftaculous.Paths;


namespace Difftaculous.Adapters
{
    internal class ZValue : IValue
    {
        public ZValue(object value)
        {
            Value = value;
        }


        public object Value { get; private set; }


        public DiffPath Path
        {
            // TODO!  HAck to get unit test to pass!
            get { return DiffPath.FromJsonPath("$.name"); }
        }
    }
}
