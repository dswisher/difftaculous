

namespace Difftaculous
{
    public interface IDiffResult
    {
        // TODO - should this just be a count of differences?
        bool AreSame { get; }
    }
}
