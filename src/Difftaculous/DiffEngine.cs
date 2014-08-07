
using System;
using System.Collections.Generic;
using System.Linq;
using Difftaculous.Adapters;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Misc;
using Difftaculous.Results;
using Difftaculous.ZModel;


namespace Difftaculous
{
    public class DiffEngine
    {
        private DiffEngine()
        {
        }



        public static IDiffResult Compare(IAdapter adapterA, IAdapter adapterB)
        {
            return Compare(adapterA, adapterB, null, null);
        }


        public static IDiffResult Compare(IAdapter adapterA, IAdapter adapterB, IEnumerable<ICaveat> caveats)
        {
            return Compare(adapterA, adapterB, caveats, null);
        }


        public static IDiffResult Compare(IAdapter adapterA, IAdapter adapterB, IEnumerable<IHint> hints)
        {
            return Compare(adapterA, adapterB, null, hints);
        }


        public static IDiffResult Compare(IAdapter adapterA, IAdapter adapterB, IEnumerable<ICaveat> caveats, IEnumerable<IHint> hints)
        {
            var a = (ZToken)adapterA.Content.Content;
            var b = (ZToken)adapterB.Content.Content;

            // Push the hints and caveats onto the models
            if (hints != null)
            {
                foreach (var hint in hints)
                {
                    foreach (var token in a.SelectTokens(hint.Path))
                    {
                        token.AddHint(hint);
                    }

                    foreach (var token in b.SelectTokens(hint.Path))
                    {
                        token.AddHint(hint);
                    }
                }
            }

            if (caveats != null)
            {
                foreach (var caveat in caveats)
                {
                    foreach (var token in a.SelectTokens(caveat.Path))
                    {
                        token.AddCaveat(caveat);
                    }

                    foreach (var token in b.SelectTokens(caveat.Path))
                    {
                        token.AddCaveat(caveat);
                    }
                }
            }

            return new DiffEngine().Diff(a, b);
        }



        private IDiffResult Diff(ZToken tokenA, ZToken tokenB)
        {
            // TODO - switch this to use IToken.Type

            var typeA = tokenA.GetType();
            var typeB = tokenB.GetType();

            if (typeA != typeB)
            {
                return new DiffResult(tokenA.Path, "types are not consistent");
            }

            if (tokenA is ZObject)
            {
                return SubDiff((ZObject)tokenA, (ZObject)tokenB);
            }

            if (tokenA is ZValue)
            {
                return SubDiff((ZValue)tokenA, (ZValue)tokenB);
            }

            if (tokenA is ZArray)
            {
                return SubDiff((ZArray)tokenA, (ZArray)tokenB);
            }

            // TODO!

            throw new NotImplementedException("Type " + typeA.Name + " is not yet handled.");
        }



        private IDiffResult SubDiff(ZObject objA, ZObject objB)
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



        private IDiffResult SubDiff(ZValue valA, ZValue valB)
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

            // TODO - this should return something other than Same, because they are NOT
            // the same.  But, returning something like "WithinTolerance" makes the DiffResult
            // more complex...
            // TODO - the caveats should be the same on A and B...verify?
            if (valA.Caveats.Any(x => x.IsAcceptable(a, b)))
            {
                return DiffResult.Same;
            }

            return new DiffResult(valA.Path, string.Format("values differ: '{0}' vs. '{1}'", a, b));
        }



        private IDiffResult SubDiff(ZArray arrayA, ZArray arrayB)
        {
            ArrayDiffHint.DiffStrategy strategy = ArrayDiffHint.DiffStrategy.Indexed;
            string keyName = null;

            // TODO - the hints should be the same on A and B...verify?
            var hint = arrayA.Hints.FirstOrDefault(x => x.GetType() == typeof(ArrayDiffHint));

            if (hint != null)
            {
                var arrayHint = (ArrayDiffHint)hint;
                strategy = arrayHint.Strategy;
                keyName = arrayHint.KeyName;
            }

            switch (strategy)
            {
                case ArrayDiffHint.DiffStrategy.Keyed:
                    return KeyedArrayDiff(arrayA, arrayB, keyName);

                case ArrayDiffHint.DiffStrategy.Indexed:
                    return IndexedArrayDiff(arrayA, arrayB);

                default:
                    throw new NotImplementedException("Array diff strategy '" + strategy + "' is not yet implemented.");
            }
        }



        private IDiffResult IndexedArrayDiff(ZArray arrayA, ZArray arrayB)
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




        private IDiffResult KeyedArrayDiff(ZArray arrayA, ZArray arrayB, string keyName)
        {
            var dictA = arrayA.ToDictionary(x => (string)(((ZValue)x[keyName]).Value));
            var dictB = arrayB.ToDictionary(x => (string)(((ZValue)x[keyName]).Value));

            var join = dictA.FullOuterJoin(dictB, x => x.Key, x => x.Key, (a, b, k) => new { Key = k, A = a, B = b });

            IDiffResult result = DiffResult.Same;

            foreach (var row in join)
            {
                if (row.A.Key == null)
                {
                    // TODO - need better annotation for this difference!
                    //_logger.Info("    ...key {0} not found in {1} {2} list...", row.Key, _hostHolder.OldHost.Name, name);
                    result = result.Merge(new DiffResult(arrayA.Path, "key not found in first list"));
                }
                else if (row.B.Key == null)
                {
                    // TODO - need better annotation for this difference!
                    //_logger.Info("    ...key {0} not found in {1} {2} list...", row.Key, _hostHolder.NewHost.Name, name);
                    result = result.Merge(new DiffResult(arrayA.Path, "key not found in second list"));
                }
                else
                {
                    var objA = row.A.Value;
                    var objB = row.B.Value;

                    result = result.Merge(Diff(objA, objB));
                }
            }

            return result;
        }
    }
}
