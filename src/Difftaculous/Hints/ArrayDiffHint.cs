
using Difftaculous.Paths;

namespace Difftaculous.Hints
{
    public class ArrayDiffHint : IHint
    {
        public enum DiffStrategy
        {
            Indexed,
            Keyed,
            DiffAlgo    // TODO - need better name
        }


        // TODO - need a way to specify multiple keys
        public ArrayDiffHint(IDiffPath path, string keyName)
        {
            KeyName = keyName;
            Path = path;
            Strategy = DiffStrategy.Keyed;
        }


        public IDiffPath Path { get; private set; }
        public DiffStrategy Strategy { get; private set; }
        public string KeyName { get; set; }
    }
}
