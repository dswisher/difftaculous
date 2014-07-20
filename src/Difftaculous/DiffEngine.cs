﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Difftaculous.Caveats;
using Difftaculous.Results;
using Newtonsoft.Json.Linq;


namespace Difftaculous
{
    internal class DiffEngine
    {
        private readonly IEnumerable<ICaveat> _caveats;


        public DiffEngine(IEnumerable<ICaveat> caveats)
        {
            _caveats = caveats ?? Enumerable.Empty<ICaveat>();
        }



        public IDiffResult Diff(JToken tokenA, JToken tokenB, IDiffPath path)
        {
            var typeA = tokenA.GetType();
            var typeB = tokenB.GetType();

            if (typeA != typeB)
            {
                return new DiffResult(path, "types are not consistent");
            }

            if ((typeA == typeof (JArray)) && (typeB == typeof (JArray)))
            {
                return SubDiff((JArray)tokenA, (JArray)tokenB, path);
            }

            if ((typeA == typeof(JObject)) && (typeB == typeof(JObject)))
            {
                return SubDiff((JObject)tokenA, (JObject)tokenB, path);
            }

            if ((typeA == typeof(JValue)) && (typeB == typeof(JValue)))
            {
                return SubDiff((JValue)tokenA, (JValue)tokenB, path);
            }

            throw new NotImplementedException("Type " + typeA.Name + " is not yet handled.");
        }



        private IDiffResult SubDiff(JObject objA, JObject objB, IDiffPath path)
        {
            IDiffResult result = DiffResult.Same;

            // TODO - do full outer join between properties
            foreach (var pair in objA)
            {
                // TODO - what if other prop does not exist?
                var other = objB.Property(pair.Key).Value;

                result = result.Merge(Diff(pair.Value, other, path.Extend(pair.Key)));
            }

            return result;
        }



        private IDiffResult SubDiff(JValue valA, JValue valB, IDiffPath path)
        {
            // TODO - better value diff
            // TODO - if different, annotate result

            string a = valA.Value.ToString();
            string b = valB.Value.ToString();

            // If things are equal, we're done...
            if (a == b)
            {
                return DiffResult.Same;
            }

            // Okay, they're not equal...see if there are any caveats that will let this
            // discrepancy pass...
            var applicableCaveats = _caveats.Where(x => x.Path.Matches(path));

            // TODO - this should return something other than Same, because they are NOT
            // the same.  But, returning something like "WithinTolerance" makes the DiffResult
            // more complex...
            if (applicableCaveats.Any(x => x.IsAcceptable(a, b)))
            {
                return DiffResult.Same;
            }

            return new DiffResult(path, string.Format("values differ: '{0}' vs. '{1}'", a, b));
        }



        private IDiffResult SubDiff(JArray arrayA, JArray arrayB, IDiffPath path)
        {
            // TODO - see if there are any hints indicating how to difference this array



            IDiffResult result = DiffResult.Same;

            // TODO - allow various matching strategies - strict indexed (this one), keyed (join) or diff-algorithm

            // This implements simple indexed array diff: compare item 1 to item 1, then item 2 to item 2, etc.
            // Limited, simplistic, but easiest to implement.
            if (arrayA.Count != arrayB.Count)
            {
                // TODO - annotate result
                return new DiffResult(path, "array item counts differ");
            }

            for (int i = 0; i < arrayA.Count; i++)
            {
                var itemA = arrayA[i];
                var itemB = arrayB[i];

                result = result.Merge(Diff(itemA, itemB, path.Extend(i)));
            }

            return result;
        }


        private IDiffResult IndexedArrayDiff(JArray arrayA, JArray arrayB, IDiffPath path)
        {
            IDiffResult result = DiffResult.Same;

            // This implements simple indexed array diff: compare item 1 to item 1, then item 2 to item 2, etc.
            // Limited, simplistic, but easiest to implement.
            if (arrayA.Count != arrayB.Count)
            {
                // TODO - annotate result
                return new DiffResult(path, "array item counts differ");
            }

            for (int i = 0; i < arrayA.Count; i++)
            {
                var itemA = arrayA[i];
                var itemB = arrayB[i];

                result = result.Merge(Diff(itemA, itemB, path.Extend(i)));
            }

            return result;            
        }
    }
}
