
using System.Collections.Generic;

namespace Difftaculous.Results
{
    /// <summary>
    /// The result of computing the difference between things.
    /// </summary>
    public interface IDiffResult
    {
        // TODO - should this just be a count of differences?
        /// <summary>
        /// True if the objects are the same, false if not.
        /// </summary>
        bool AreSame { get; }

        /// <summary>
        /// Details of any differences that were found.
        /// </summary>
        IEnumerable<DiffAnnotation> Annotations { get; }

        /// <summary>
        /// Merge two difference results together
        /// </summary>
        /// <param name="other">The other results that should be merged into this one</param>
        /// <returns>The merged result</returns>
        IDiffResult Merge(IDiffResult other);
    }
}
