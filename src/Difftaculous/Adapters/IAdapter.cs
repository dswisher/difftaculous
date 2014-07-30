
using Newtonsoft.Json.Linq;

namespace Difftaculous.Adapters
{
    public interface IAdapter
    {

        // TODO - this needs to be something non-JSON!!!
        JToken Content { get; }

    }
}
