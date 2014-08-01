
using Difftaculous.Paths;

namespace Difftaculous.ZModel
{
    /// <summary>
    /// A token can be an object, value or array
    /// </summary>
    public interface IToken
    {
        DiffPath Path { get; }

        IToken this[string key] { get; }

        TokenType Type { get; }

        // NOTE: Json.Net defines this as a JContainer
        IToken Parent { get; }
    }
}
