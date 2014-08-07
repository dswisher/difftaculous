
using System;
using System.Xml;
using Difftaculous.ZModel;

namespace Difftaculous.Adapters
{
    public class XmlAdapter : IAdapter
    {

        public XmlAdapter(string content)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            Content = new AdaptedContent(Adapt(doc));
        }


        public IAdaptedContent Content { get; private set; }



        private ZToken Adapt(XmlNode node)
        {
            foreach (var child in node.ChildNodes)
            {
                if (child is XmlElement)
                {
                    throw new NotImplementedException("Elements are not yet handled!");
                }
                else if (child is XmlAttribute)
                {
                    throw new NotImplementedException("Attributes are not yet handled!");
                }
            }

            //return new ZObject();
            // TODO
            throw new NotImplementedException("Boom");
        }
    }
}
