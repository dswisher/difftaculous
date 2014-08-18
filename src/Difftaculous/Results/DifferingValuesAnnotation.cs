
using Difftaculous.Paths;


namespace Difftaculous.Results
{
    /// <summary>
    /// Annotation indicating that values differ.
    /// </summary>
    public class DifferingValuesAnnotation : AbstractDiffAnnotation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path where the values differ</param>
        /// <param name="valueA">The first value</param>
        /// <param name="valueB">The second value</param>
        public DifferingValuesAnnotation(DiffPath path, object valueA, object valueB)
            : base(path)
        {
            ValueA = valueA;
            ValueB = valueB;
        }


        /// <summary>
        /// The first value
        /// </summary>
        public object ValueA { get; private set; }


        /// <summary>
        /// The second value
        /// </summary>
        public object ValueB { get; private set; }


        public override string Message
        {
            get { return string.Format("values differ: '{0}' vs. '{1}'", ValueA, ValueB); }
        }
    }
}
