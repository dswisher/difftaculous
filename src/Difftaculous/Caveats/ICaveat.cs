
using Difftaculous.Paths;

namespace Difftaculous.Caveats
{
    public interface ICaveat
    {
        DiffPath Path { get; }

        bool IsAcceptable(string a, string b);
    }
}
