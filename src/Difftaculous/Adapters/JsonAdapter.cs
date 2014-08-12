
using System;
using Difftaculous.ZModel;
using Newtonsoft.Json.Linq;


namespace Difftaculous.Adapters
{
    /// <summary>
    /// Adapt JSON content so it can be run through the difference engine.
    /// </summary>
    public class JsonAdapter : AbstractAdapter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">The content to be adapted.</param>
        public JsonAdapter(string content)
        {
            var top = JToken.Parse(content);

            Content = Adapt(top);
        }



        private ZToken Adapt(JToken jtoken)
        {
            var type = jtoken.GetType();

            if (type == typeof(JObject))
            {
                return Adapt((JObject)jtoken);
            }

            if (type == typeof(JValue))
            {
                return Adapt((JValue)jtoken);
            }

            if (type == typeof(JArray))
            {
                return Adapt((JArray)jtoken);
            }

            throw new NotImplementedException("Adapting type '" + type.Name + "' is not yet implemented.");
        }


        private ZToken Adapt(JObject jobject)
        {
            ZObject zobject = new ZObject();

            foreach (var jprop in jobject.Properties())
            {
                zobject.Add(new ZProperty(jprop.Name, Adapt(jprop.Value)));
            }

            return zobject;
        }



        private ZToken Adapt(JValue jvalue)
        {
            // TODO - pass along the type?!

            ZValue zvalue = new ZValue(jvalue.Value);

            return zvalue;
        }



        private ZToken Adapt(JArray jarray)
        {
            ZArray zarray = new ZArray();

            foreach (var item in jarray)
            {
                zarray.Add(Adapt(item));
            }

            return zarray;
        }
    }
}
