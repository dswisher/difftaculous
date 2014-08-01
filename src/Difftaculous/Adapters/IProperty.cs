

namespace Difftaculous.Adapters
{
    public interface IProperty : IToken
    {
        string Name { get; }
        IToken Value { get; }
    }
}
