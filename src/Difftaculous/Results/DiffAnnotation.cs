
using Difftaculous.Paths;

namespace Difftaculous.Results
{
    /// <summary>
    /// A description of a difference between two items.
    /// </summary>
    public class DiffAnnotation
    {
        internal DiffAnnotation(DiffPath path, string message)
        {
            Path = path;
            Message = message;
        }

        /// <summary>
        /// The path where the difference was detected.
        /// </summary>
        public DiffPath Path { get; private set; }

        /// <summary>
        /// A description of the difference.
        /// </summary>
        public string Message { get; private set; }
    }
}
