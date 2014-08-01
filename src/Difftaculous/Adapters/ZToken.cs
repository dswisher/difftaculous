
using Difftaculous.Paths;


namespace Difftaculous.Adapters
{
    internal abstract class ZToken : IToken
    {
        public DiffPath Path { get { return DiffPath.FromToken(this); } }
    }
}
