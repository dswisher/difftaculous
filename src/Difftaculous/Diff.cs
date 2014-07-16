
using System;
using Difftaculous.Results;
using Newtonsoft.Json;


namespace Difftaculous
{
    public static class Diff
    {

        public static IDiffResult Json(string a, string b)
        {
            // TODO - generalize this to use some sort of Navigator thingy so we can handle XML, too!
            var jsonA = JsonConvert.DeserializeObject(a);
            var jsonB = JsonConvert.DeserializeObject(b);

            // Should have either a JArray or a JObject...

            Console.WriteLine("jsonA.getType = {0}", jsonA.GetType());
            Console.WriteLine("jsonB.getType = {0}", jsonB.GetType());

            // TODO!
            return new DiffResult() { AreSame = true };
        }

    }
}
