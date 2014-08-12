
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Difftaculous.ZModel;


namespace Difftaculous.Adapters
{
    /// <summary>
    /// Adapt XML content so it can be run through the difference engine.
    /// </summary>
    public class XmlAdapter : AbstractAdapter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">The content to be adapted.</param>
        public XmlAdapter(string content)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            Content = Adapt(doc.DocumentElement);
        }



        private ZToken Adapt(XmlNode node)
        {
            var counts = TallyChildren(node);

            // If there are no counts, we've got an empty object...
            if (counts.Count == 0)
            {
                return new ZObject();
            }

            // If the counts are all one, we have an object...
            if (counts.All(x => x.Value == 1))
            {
                return AdaptObject(node, counts);
            }

            // TODO - arrays and other cases

            throw new NotImplementedException("Boom");
        }



        private ZObject AdaptObject(XmlNode node, Dictionary<string, int> counts)
        {
            ZObject obj = new ZObject();

            foreach (XmlNode child in node.ChildNodes)
            {
                string name = null;
                if ((child is XmlElement))
                {
                    name = child.Name;

                    if (counts.ContainsKey(name))
                    {
                        if ((child.FirstChild == child.LastChild) && (child.FirstChild is XmlText))
                        {
                            // TODO - how to know what type to use for the property?
                            obj.Add(name, new ZValue(child.FirstChild.Value));
                        }
                        else
                        {
                            obj.Add(name, Adapt(child));                            
                        }
                    }
                }
                else if (child is XmlAttribute)
                {
                    name = child.Name;

                    if (counts.ContainsKey(name))
                    {
                        throw new NotImplementedException("Attributes are not yet handled!");
                    }
                }

                if ((name != null) && counts.ContainsKey(name))
                {
                    counts.Remove(name);
                }
            }

            return obj;
        }



        private Dictionary<string, int> TallyChildren(XmlNode node)
        {
            // TODO - replace with a nice Linq query at some point
            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (var child in node.ChildNodes)
            {
                string name = null;
                if ((child is XmlElement))
                {
                    name = ((XmlElement)child).Name;
                }
                else if (child is XmlAttribute)
                {
                    name = ((XmlAttribute)child).Name;
                }

                if (name == null)
                {
                    continue;
                }

                if (counts.ContainsKey(name))
                {
                    counts[name] += 1;
                }
                else
                {
                    counts.Add(name, 1);
                }
            }

            return counts;
        }
    }
}
