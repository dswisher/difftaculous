
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Difftaculous.Results
{
    internal class DiffResult : IDiffResult
    {
        private readonly static DiffResult _same = new DiffResult { AreSame = true };


        public static DiffResult Same { get { return _same; } }

        private DiffResult()
        {
            Annotations = Enumerable.Empty<AbstractDiffAnnotation>();
        }



        public DiffResult(AbstractDiffAnnotation annotation)
        {
            Annotations = new[] { annotation };
            AreSame = annotation.AreSame;
        }



        public IEnumerable<AbstractDiffAnnotation> Annotations { get; private set; }
        public bool AreSame { get; private set; }


        public IDiffResult Merge(IDiffResult other)
        {
            return new DiffResult { AreSame = AreSame && other.AreSame, Annotations = Annotations.Union(other.Annotations) };
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Format("DiffResult, AreSame={0}", AreSame));

            foreach (var anno in Annotations)
            {
                builder.AppendLine(string.Format("Anno, path={0}, msg={1}", anno.Path.AsJsonPath, anno.Message));
            }

            return builder.ToString();
        }
    }
}
