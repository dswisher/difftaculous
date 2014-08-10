
using System;


namespace Difftaculous.ZModel
{
    /// <summary>
    /// An exception from the internal Diff model.
    /// </summary>
    public class ZException : Exception
    {
        // TODO - give this a better name!

        internal ZException(string message)
            : base(message)
        {
        }

    }
}
