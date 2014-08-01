

namespace Difftaculous.Adapters
{
    public interface IArray : IToken
    {
        int Count { get; }
        IToken this[int index] { get; }
    }
}
