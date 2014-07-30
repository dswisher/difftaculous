

namespace Difftaculous.Adapters
{
    public class XmlAdapter : IAdapter
    {

        public XmlAdapter(string content)
        {
            // TODO!
            Content = new ZToken();
        }


        // TODO - this needs to change to something non-JSON specific!
        public IToken Content { get; private set; }

    }
}
