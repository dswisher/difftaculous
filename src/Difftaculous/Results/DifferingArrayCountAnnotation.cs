
using Difftaculous.Paths;


namespace Difftaculous.Results
{
    /// <summary>
    /// Annotation indicating that two arrays have differing number of elements.
    /// </summary>
    public class DifferingArrayCountAnnotation : AbstractDiffAnnotation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path of the array whose counts differ</param>
        /// <param name="countA">The first count</param>
        /// <param name="countB">The second count</param>
        public DifferingArrayCountAnnotation(DiffPath path, int countA, int countB)
            : base(path)
        {
            CountA = countA;
            CountB = countB;
        }


        /// <summary>
        /// The first count
        /// </summary>
        public int CountA { get; private set; }


        /// <summary>
        /// The second count
        /// </summary>
        public int CountB { get; private set; }


        public override string Message
        {
            get { return string.Format("array item counts differ: {0} vs. {1}", CountA, CountB); }
        }
    }
}
