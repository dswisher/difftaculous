
using Newtonsoft.Json;


namespace Difftaculous.Adapters
{
    /// <summary>
    /// Adapt an object so it can be run through the difference engine.
    /// </summary>
    public class ObjectAdapter : AbstractAdapter
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">The content to be adapted.</param>
        public ObjectAdapter(object content)
        {
            // TODO - this is a hack just to get something working.
            // It should really examine the properties using reflection or whatever.
            var json = JsonConvert.SerializeObject(content);

            Content = new JsonAdapter(json).Content;
        }
    }
}
