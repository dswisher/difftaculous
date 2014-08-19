
using System;
using Difftaculous.Paths;


namespace Difftaculous.Results
{
    /// <summary>
    /// Annotation indicating the types differ
    /// </summary>
    public class InconsistentTypesAnnotation : AbstractDiffAnnotation
    {
        private readonly Type _typeA;
        private readonly Type _typeB;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path where the types differ</param>
        /// <param name="typeA">The first type</param>
        /// <param name="typeB">The second type</param>
        public InconsistentTypesAnnotation(DiffPath path, Type typeA, Type typeB)
            : base(path)
        {
            _typeA = typeA;
            _typeB = typeB;
        }


        /// <summary>
        /// A human-readable explanation of the annotation.
        /// </summary>
        public override string Message
        {
            get { return string.Format("Mismatched types: {0} vs. {1}.", _typeA.Name, _typeB.Name); }
        }
    }
}
