
using Difftaculous.Paths;

namespace Difftaculous.Caveats
{
    internal interface ICaveat
    {
        DiffPath Path { get; }

        bool IsAcceptable(string a, string b);
    }
}
