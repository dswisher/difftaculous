
using Difftaculous.ZModel;


namespace Difftaculous.Adapters
{
    /// <summary>
    /// Base class for adapters.
    /// </summary>
    public abstract class AbstractAdapter
    {

        internal ZToken Content { get; set; }

        // TODO - add Func that includes the preferred way of formatting paths (JsonPath vs. XPath)

    }
}
