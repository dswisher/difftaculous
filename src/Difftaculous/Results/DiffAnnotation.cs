
using Difftaculous.Paths;

namespace Difftaculous.Results
{
    /// <summary>
    /// A description of a difference between two items.
    /// </summary>
    public class DiffAnnotation : AbstractDiffAnnotation
    {
        private readonly string _message;

        internal DiffAnnotation(DiffPath path, string message)
            : base(path)
        {
            _message = message;
        }


        public override string Message { get { return _message; } }
    }
}
