

namespace Difftaculous.Adapters
{
    public interface IProperty
    {
        string Name { get; }
        IToken Value { get; }
    }
}
