

namespace Difftaculous.Results
{
    internal class DiffResult : IDiffResult
    {

        // TODO - this should have a private setter, to enforce immutability!
        public bool AreSame { get; set; }

        // TODO - need a way to "annotate" any differences (a single bool won't cut it)

        public IDiffResult Merge(IDiffResult other)
        {
            return new DiffResult { AreSame = AreSame && other.AreSame };
        }

    }
}
