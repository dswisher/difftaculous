
using System;


namespace Difftaculous.Paths
{
    /// <summary>
    /// An exception that occurred creating or using a path
    /// </summary>
    public class PathException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message describing the exception</param>
        public PathException(string message)
            : base(message)
        {
        }
    }
}
