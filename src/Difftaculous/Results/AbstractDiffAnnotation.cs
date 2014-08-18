
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


        /// <summary>
        /// An indication of whether this annotation is a difference
        /// </summary>
        /// <remarks>
        /// If not a difference, it is probably a variance
        /// </remarks>
        public virtual bool AreSame { get { return false; } }
    }
}
