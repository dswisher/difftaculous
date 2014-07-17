
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Difftaculous
{
    public static class Diff
    {

        public static IDiffResult Json(string a, string b)
        {
            // TODO - generalize this to use some sort of Navigator thingy so we can handle XML, too!
            var jsonA = (JToken)JsonConvert.DeserializeObject(a);
            var jsonB = (JToken)JsonConvert.DeserializeObject(b);

            // Should have either a JArray or a JObject...
            DiffEngine engine = new DiffEngine();

            return engine.Diff(jsonA, jsonB);
        }

    }
}
