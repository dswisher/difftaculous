
using System;
using Newtonsoft.Json.Linq;


namespace Difftaculous.Adapters
{
    public class XmlAdapter : IAdapter
    {

        public XmlAdapter(string content)
        {
            throw new NotImplementedException("TBD");
        }


        // TODO - this needs to change to something non-JSON specific!
        public JToken Content { get; private set; }

    }
}
