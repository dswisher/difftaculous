
using System;
using System.Collections.Generic;
using System.Linq;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Misc;
using Difftaculous.Paths;
using Difftaculous.Results;
using Difftaculous.ZModel;
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




        public IDiffResult Diff(IToken tokenA, IToken tokenB)
        {
            // TODO - switch this to use IToken.Type

            var typeA = tokenA.GetType();
            var typeB = tokenB.GetType();

            if (typeA != typeB)
            {
                return new DiffResult(tokenA.Path, "types are not consistent");
            }

            if (tokenA is IObject)
            {
                return SubDiff((IObject)tokenA, (IObject)tokenB);
            }

            if (tokenA is IValue)
            {
                return SubDiff((IValue)tokenA, (IValue)tokenB);
            }

            if (tokenA is IArray)
            {
                return SubDiff((IArray)tokenA, (IArray)tokenB);
            }

            // TODO!

            throw new NotImplementedException("Type " + typeA.Name + " is not yet handled.");
        }



        private IDiffResult SubDiff(IObject objA, IObject objB)
        {
            IDiffResult result = DiffResult.Same;

            foreach (var pair in objA.Properties.FullOuterJoin(objB.Properties, x => x.Name, x => x.Name, (p1, p2, n) => new { PropA = p1, PropB = p2, Name = n }))
            {
                if ((pair.PropA == null) || (pair.PropB == null))
                {
                    // TODO - handle missing items!
                    throw new NotImplementedException("Handle missing items!");
                }

                result = result.Merge(Diff(pair.PropA.Value, pair.PropB.Value));
            }

            return result;
        }



        private IDiffResult SubDiff(IValue valA, IValue valB)
        {
            // TODO - better value diff - numerics - 34.0 vs. 34.00 isn't really a difference, is it?

            if ((valA.Value == null) || (valB.Value == null))
            {
                if ((valA.Value == null) && ((valB.Value == null)))
                {
                    return DiffResult.Same;
                }

                // TODO - what about null caveats?

                return new DiffResult(valA.Path, string.Format("values differ: '{0}' vs. '{1}'", valA.Value, valB.Value));
            }

            string a = valA.Value.ToString();
            string b = valB.Value.ToString();

            // If things are equal, we're done...
            if (a == b)
            {
                return DiffResult.Same;
            }

            // Okay, they're not equal...see if there are any caveats that will let this
            // discrepancy pass...
            var applicableCaveats = _caveats.Where(x => x.Path.Matches(valA.Path));

            // TODO - this should return something other than Same, because they are NOT
            // the same.  But, returning something like "WithinTolerance" makes the DiffResult
            // more complex...
            if (applicableCaveats.Any(x => x.IsAcceptable(a, b)))
            {
                return DiffResult.Same;
            }

            return new DiffResult(valA.Path, string.Format("values differ: '{0}' vs. '{1}'", a, b));
        }



        private IDiffResult SubDiff(IArray arrayA, IArray arrayB)
        {
            ArrayDiffHint.DiffStrategy strategy = ArrayDiffHint.DiffStrategy.Indexed;
            //string keyName = null;

            var hint = _hints.FirstOrDefault(x => x.Path.Matches(arrayA) && x.GetType() == typeof(ArrayDiffHint));

            if (hint != null)
            {
                var arrayHint = (ArrayDiffHint)hint;
                strategy = arrayHint.Strategy;
                //keyName = arrayHint.KeyName;
            }

            // TODO - should the adapter shoulder some of the burden of handling key'd arrays??

            switch (strategy)
            {
                //case ArrayDiffHint.DiffStrategy.Keyed:
                //    return KeyedArrayDiff(arrayA, arrayB, keyName);

                case ArrayDiffHint.DiffStrategy.Indexed:
                    return IndexedArrayDiff(arrayA, arrayB);

                default:
                    throw new NotImplementedException("Index diff strategy '" + strategy + "' is not yet implemented.");
            }
        }



        private IDiffResult IndexedArrayDiff(IArray arrayA, IArray arrayB)
        {
            IDiffResult result = DiffResult.Same;

            // This implements simple indexed array diff: compare item 1 to item 1, then item 2 to item 2, etc.
            // Limited, simplistic, but easiest to implement.
            if (arrayA.Count != arrayB.Count)
            {
                // TODO - annotate result
                return new DiffResult(arrayA.Path, "array item counts differ");
            }

            for (int i = 0; i < arrayA.Count; i++)
            {
                var itemA = arrayA[i];
                var itemB = arrayB[i];

                result = result.Merge(Diff(itemA, itemB));
            }

            return result;
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
            // TODO - better value diff - numerics - 34.0 vs. 34.00 isn't really a difference, is it?

            if ((valA.Value == null) || (valB.Value == null))
            {
                if ((valA.Value == null) && ((valB.Value == null)))
                {
                    return DiffResult.Same;
                }

                // TODO - what about null caveats?

                return new DiffResult(path, string.Format("values differ: '{0}' vs. '{1}'", valA.Value, valB.Value));
            }

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
