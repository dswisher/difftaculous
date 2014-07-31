using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Difftaculous.Adapters
{
    // TODO - once this is functional, remove JsonAdapter and rename this to JsonAdapter
    public class JsonAdapterEx : IAdapter
    {
        public JsonAdapterEx(string content)
        {
            var top = JToken.Parse(content);

            Content = Adapt(top);
        }


        public IToken Content { get; private set; }


        private IToken Adapt(JToken jtoken)
        {
            var type = jtoken.GetType();

            if (type == typeof(JObject))
            {
                return Adapt((JObject)jtoken);
            }

            throw new NotImplementedException("Adapting type '" + type.Name + "' is not yet implemented.");
        }


        private IToken Adapt(JObject jobject)
        {
            // TODO
            return new ZObject();
        }
    }
}
