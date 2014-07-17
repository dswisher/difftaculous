
using System;
using Difftaculous.Results;
using Newtonsoft.Json.Linq;


namespace Difftaculous
{
    internal class DiffEngine
    {

        public IDiffResult Diff(JToken tokenA, JToken tokenB)
        {
            var typeA = tokenA.GetType();
            var typeB = tokenB.GetType();

            if (typeA != typeB)
            {
                // TODO!  Annotate result - structural difference
                return new DiffResult { AreSame = false };
            }

            if ((typeA == typeof (JArray)) && (typeB == typeof (JArray)))
            {
                return SubDiff((JArray)tokenA, (JArray)tokenB);
            }

            if ((typeA == typeof(JObject)) && (typeB == typeof(JObject)))
            {
                return SubDiff((JObject)tokenA, (JObject)tokenB);
            }

            if ((typeA == typeof(JValue)) && (typeB == typeof(JValue)))
            {
                return SubDiff((JValue)tokenA, (JValue)tokenB);
            }

            throw new NotImplementedException("Type " + typeA.Name + " is not yet handled.");
        }



        private IDiffResult SubDiff(JObject objA, JObject objB)
        {
            IDiffResult result = new DiffResult { AreSame = true };

            // TODO - do full outer join between properties
            foreach (var pair in objA)
            {
                // TODO - what if other prop does not exist?
                var other = objB.Property(pair.Key).Value;

                result = result.Merge(Diff(pair.Value, other));
            }

            return result;
        }



        private IDiffResult SubDiff(JValue valA, JValue valB)
        {
            // TODO - better value diff
            // TODO - if different, annotate result
            return new DiffResult { AreSame = valA.Value.ToString() == valB.Value.ToString() };
        }



        private IDiffResult SubDiff(JArray arrayA, JArray arrayB)
        {
            // TODO - array diff
            return new DiffResult { AreSame = true };
        }
    }
}
