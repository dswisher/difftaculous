
using System;
using System.Collections.Generic;
using System.Linq;
using Difftaculous.Adapters;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Misc;
using Difftaculous.Paths;
using Difftaculous.Results;

using Newtonsoft.Json.Linq;


namespace Difftaculous
{
    internal class DiffEngine
    {
        private readonly IEnumerable<IHint> _hints;
        private readonly IEnumerable<ICaveat> _caveats;


        public DiffEngine(IEnumerable<ICaveat> caveats, IEnumerable<IHint> hints)
        {
            _hints = hints ?? Enumerable.Empty<IHint>();
            _caveats = caveats ?? Enumerable.Empty<ICaveat>();
        }




        public IDiffResult Diff(IToken tokenA, IToken tokenB, IDiffPath path)
        {
            // TODO!
            return DiffResult.Same;
        }




        // TODO - this (and the other JSON-based methods) are obsolete and should be removed (once IToken implemented is complete)
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
            ArrayDiffHint.DiffStrategy strategy = ArrayDiffHint.DiffStrategy.Indexed;
            string keyName = null;

            var hint = _hints.FirstOrDefault(x => x.Path.Matches(path) && x.GetType() == typeof(ArrayDiffHint));

            if (hint != null)
            {
                var arrayHint = (ArrayDiffHint) hint;
                strategy = arrayHint.Strategy;
                keyName = arrayHint.KeyName;
            }

            switch (strategy)
            {
                case ArrayDiffHint.DiffStrategy.Keyed:
                    return KeyedArrayDiff(arrayA, arrayB, path, keyName);

                case ArrayDiffHint.DiffStrategy.Indexed:
                    return IndexedArrayDiff(arrayA, arrayB, path);

                default:
                    throw new NotImplementedException("Index diff strategy '" + strategy + "' is not yet implemented.");
            }
        }



        private IDiffResult KeyedArrayDiff(JArray arrayA, JArray arrayB, IDiffPath path, string keyName)
        {
            var dictA = arrayA.ToDictionary(x => (string)x.SelectToken(keyName));
            var dictB = arrayB.ToDictionary(x => (string)x.SelectToken(keyName));

            var join = dictA.FullOuterJoin(dictB, x => x.Key, x => x.Key, (a, b, k) => new { Key = k, A = a, B = b });

            IDiffResult result = DiffResult.Same;

            foreach (var row in join)
            {
                if (row.A.Key == null)
                {
                    // TODO - need better annotation for this difference!
                    //_logger.Info("    ...key {0} not found in {1} {2} list...", row.Key, _hostHolder.OldHost.Name, name);
                    result = result.Merge(new DiffResult(path, "key not found in first list"));
                }
                else if (row.B.Key == null)
                {
                    // TODO - need better annotation for this difference!
                    //_logger.Info("    ...key {0} not found in {1} {2} list...", row.Key, _hostHolder.NewHost.Name, name);
                    result = result.Merge(new DiffResult(path, "key not found in second list"));
                }
                else
                {
                    var objA = (JObject) row.A.Value;
                    var objB = (JObject) row.B.Value;

                    result = result.Merge(SubDiff(objA, objB, path.ArrayExtend(row.Key)));
                }
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

                result = result.Merge(Diff(itemA, itemB, path.ArrayExtend(i)));
            }

            return result;            
        }
    }
}
