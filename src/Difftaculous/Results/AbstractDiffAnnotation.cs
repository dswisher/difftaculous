
using Difftaculous.Paths;


namespace Difftaculous.Results
{
    /// <summary>
    /// Base class for annotations.
    /// </summary>
    public abstract class AbstractDiffAnnotation
    {

        internal AbstractDiffAnnotation(DiffPath path)
        {
            Path = path;
        }


        /// <summary>
        /// The path where the difference was detected.
        /// </summary>
        public DiffPath Path { get; private set; }


        /// <summary>
        /// A human-readable explanation of the annotation.
        /// </summary>
        public abstract string Message { get; }
    }
}
