
using Difftaculous.Paths;


namespace Difftaculous.Results
{
    /// <summary>
    /// Annotation indicating that a property is missing.
    /// </summary>
    public class MissingPropertyAnnotation : AbstractDiffAnnotation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="propertyName">The name of the property that is missing</param>
        public MissingPropertyAnnotation(DiffPath path, string propertyName)
            : base(path)
        {
            PropertyName = propertyName;
        }


        /// <summary>
        /// The name of the missing property.
        /// </summary>
        public string PropertyName { get; private set; }


        /// <summary>
        /// A human-readable explanation of the annotation.
        /// </summary>
        public override string Message
        {
            get { return string.Format("Property '{0}' is missing.", PropertyName); }
        }
    }
}
