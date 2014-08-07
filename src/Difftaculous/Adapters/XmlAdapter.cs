

using Difftaculous.ZModel;

namespace Difftaculous.Adapters
{
    public class XmlAdapter : IAdapter
    {

        public XmlAdapter(string content)
        {
            // TODO!
            Content = new AdaptedContent(new ZObject());
        }


        public IAdaptedContent Content { get; private set; }
    }
}
