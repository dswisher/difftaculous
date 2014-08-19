
using Difftaculous.Paths;


namespace Difftaculous.Results
{
    /// <summary>
    /// Annotation for a key missing when comparing two arrays
    /// </summary>
    public class MissingKeyAnnotation : AbstractDiffAnnotation
    {
        private readonly bool _first;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path of the difference</param>
        /// <param name="first">True if the key is missing from the first list, false if from the second</param>
        public MissingKeyAnnotation(DiffPath path, bool first)
            : base(path)
        {
            _first = first;
        }


        /// <summary>
        /// A human-readable explanation of the annotation.
        /// </summary>
        public override string Message
        {
            get { return string.Format("key not found in {0} list", _first ? "first" : "second"); }
        }
    }
}
