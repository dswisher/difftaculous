
using System.Collections.Generic;

namespace Difftaculous.ZModel
{
    public interface IArray : IToken, IEnumerable<IToken>
    {
        int Count { get; }
        IToken this[int index] { get; }
        int IndexOf(IToken item);
    }
}
