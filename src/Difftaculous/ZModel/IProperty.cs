

namespace Difftaculous.ZModel
{
    public interface IProperty : IToken
    {
        string Name { get; }
        IToken Value { get; }
    }
}
