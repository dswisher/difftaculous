
using Difftaculous.Paths;

namespace Difftaculous.Adapters
{
    /// <summary>
    /// A token can be an object, value or array
    /// </summary>
    public interface IToken
    {
        DiffPath Path { get; }
    }
}
