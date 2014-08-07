
namespace Difftaculous.Adapters
{
    public interface IAdaptedContent
    {
        // TODO - add Func that includes the preferred way of formatting paths (JsonPath vs. XPath)

        object Content { get; }
    }



    internal class AdaptedContent : IAdaptedContent
    {
        public AdaptedContent(object content)
        {
            Content = content;
        }


        public object Content { get; private set; }
    }
}
