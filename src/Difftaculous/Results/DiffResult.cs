
using System.Collections.Generic;
using System.Linq;


namespace Difftaculous.Results
{
    internal class DiffResult : IDiffResult
    {
        private readonly static DiffResult _same = new DiffResult { AreSame = true };


        public static DiffResult Same { get { return _same; } }

        private DiffResult()
        {
            Annotations = Enumerable.Empty<DiffAnnotation>();
        }


        // TODO - need to replace message with a structure of some sort
        public DiffResult(IDiffPath path, string message)
        {
            var annotation = new DiffAnnotation(path, message);

            Annotations = new[] { annotation };
        }

        public IEnumerable<DiffAnnotation> Annotations { get; private set; }
        public bool AreSame { get; private set; }


        public IDiffResult Merge(IDiffResult other)
        {
            return new DiffResult { AreSame = AreSame && other.AreSame, Annotations = Annotations.Union(other.Annotations) };
        }



    }
}
