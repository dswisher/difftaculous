
using System;
using Difftaculous.ZModel;
using Newtonsoft.Json.Linq;


namespace Difftaculous.Adapters
{
    public class JsonAdapter : IAdapter
    {
        public JsonAdapter(string content)
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


        private IToken Adapt(JObject jobject)
        {
            ZObject zobject = new ZObject();

            foreach (var jprop in jobject.Properties())
            {
                zobject.AddProperty(new ZProperty(jprop.Name, Adapt(jprop.Value)));
            }

            return zobject;
        }



        private IToken Adapt(JValue jvalue)
        {
            // TODO - pass along the type?!

            ZValue zvalue = new ZValue(jvalue.Value);

            return zvalue;
        }



        private IToken Adapt(JArray jarray)
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
