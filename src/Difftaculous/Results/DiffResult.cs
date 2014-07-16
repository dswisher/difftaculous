

namespace Difftaculous.Results
{
    internal class DiffResult : IDiffResult
    {

        // TODO - this should have a private setter, to enforce immutability!
        public bool AreSame { get; set; }

    }
}
