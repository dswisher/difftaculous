
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Difftaculous.Adapters
{
    public class JsonAdapter // : IAdapter
    {

        public JsonAdapter(string content)
        {
            Content = (JToken) JsonConvert.DeserializeObject(content);
        }


        // TODO - this needs to change to something non-JSON specific!
        public JToken Content { get; private set; }

    }
}
