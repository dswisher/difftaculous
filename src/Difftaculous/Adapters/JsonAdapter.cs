using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Difftaculous.Adapters
{
    public class JsonAdapter
    {

        public JsonAdapter(string content)
        {
            Content = (JToken) JsonConvert.DeserializeObject(content);
        }


        // TODO - this needs to change to something non-JSON specific!
        public JToken Content { get; private set; }

    }
}
