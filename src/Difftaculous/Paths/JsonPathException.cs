
using System;


namespace Difftaculous.Paths
{
    // TODO - should this be more general, like PathException?
    public class JsonPathException : Exception
    {
        public JsonPathException(string message)
            : base(message)
        {
        }
    }
}
