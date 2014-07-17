
using System;
using Difftaculous.Results;
using Newtonsoft.Json.Linq;


namespace Difftaculous
{
    internal class DiffEngine
    {
        // TODO - this should take a list of "concessions" - items that relax the strict diff, like "within 10%"

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
            IDiffResult result = new DiffResult { AreSame = true };

            // TODO - allow various matching strategies - strict indexed (this one), keyed (join) or diff-algorithm

            // This implements simple indexed array diff: compare item 1 to item 1, then item 2 to item 2, etc.
            // Limited, simplistic, but easiest to implement.
            if (arrayA.Count != arrayB.Count)
            {
                // TODO - annotate result
                return new DiffResult { AreSame = false };
            }

            for (int i = 0; i < arrayA.Count; i++)
            {
                var itemA = arrayA[i];
                var itemB = arrayB[i];

                result = result.Merge(Diff(itemA, itemB));
            }

            return result;
        }
    }
}
