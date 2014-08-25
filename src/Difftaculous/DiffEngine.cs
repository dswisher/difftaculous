#region License
//The MIT License (MIT)

//Copyright (c) 2014 Doug Swisher

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion

using System;
using System.Linq;
using Difftaculous.Adapters;
using Difftaculous.Hints;
using Difftaculous.Misc;
using Difftaculous.Results;
using Difftaculous.ZModel;


namespace Difftaculous
{
    /// <summary>
    /// The engine used to compare things.
    /// </summary>
    public class DiffEngine
    {
        private DiffEngine()
        {
        }



        /// <summary>
        /// Compare two adapted things, using the specified (optional) settings.
        /// </summary>
        /// <param name="adapterA">The first thing to compare.</param>
        /// <param name="adapterB">The second thing to compare.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public static IDiffResult Compare(AbstractAdapter adapterA, AbstractAdapter adapterB, DiffSettings settings = null)
        {
            if (settings == null)
            {
                settings = new DiffSettings();
            }

            var a = adapterA.Content;
            var b = adapterB.Content;

            // Push the hints and caveats onto the models
            if (settings.Hints != null)
            {
                foreach (var hint in settings.Hints)
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

            if (settings.Caveats != null)
            {
                foreach (var caveat in settings.Caveats)
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
                return new DiffResult(new InconsistentTypesAnnotation(tokenA.Path, typeA, typeB));
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

            throw new NotImplementedException("Type " + typeA.Name + " is not yet handled.");
        }



        private IDiffResult SubDiff(ZObject objA, ZObject objB)
        {
            IDiffResult result = DiffResult.Same;

            foreach (var pair in objA.Properties().FullOuterJoin(objB.Properties(), x => x.Name, x => x.Name, (p1, p2, n) => new { PropA = p1, PropB = p2, Name = n }))
            {
                if (pair.PropA == null)
                {
                    result = result.Merge(new DiffResult(new MissingPropertyAnnotation(objA.Path, pair.Name)));
                    continue;
                }

                if (pair.PropB == null)
                {
                    result = result.Merge(new DiffResult(new MissingPropertyAnnotation(objB.Path, pair.Name)));
                    continue;
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

                return new DiffResult(new DifferingValuesAnnotation(valA.Path, valA.Value, valB.Value));
            }

            // If things are equal, we're done...
            if (valA.Equals(valB))
            {
                return DiffResult.Same;
            }

            string a = valA.Value.ToString();
            string b = valB.Value.ToString();

            // TODO - the caveats should be the same on A and B...verify?
            if (valA.Caveats.Any(x => x.IsAcceptable(a, b)))
            {
                return new DiffResult(new DifferingValuesAnnotation(valA.Path, a, b, true));
            }

            return new DiffResult(new DifferingValuesAnnotation(valA.Path, a, b));
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
                return new DiffResult(new DifferingArrayCountAnnotation(arrayA.Path, arrayA.Count, arrayB.Count));
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
            // This only works on arrays that contain objects (they're the only things
            // that have properties we can use as keys).  If we have non-objects, we have
            // a difference (or perhaps an error, or ??).
            if (arrayA.Any(x => x.Type != TokenType.Object) || arrayB.Any(x => x.Type != TokenType.Object))
            {
                throw new NotImplementedException("Key-array diff for non-objects is not implemented.");
            }

            var dictA = arrayA.ToDictionary(x => (string)((ZValue)(((ZObject)x).Property(keyName, false)).Value));
            var dictB = arrayB.ToDictionary(x => (string)((ZValue)(((ZObject)x).Property(keyName, false)).Value));

            // TODO - use x.Property() instead of x[] and use case-insensitive form?
            // var dictA = arrayA.ToDictionary(x => (string)(((ZValue)x[keyName]).Value));
            // var dictB = arrayB.ToDictionary(x => (string)(((ZValue)x[keyName]).Value));

            var join = dictA.FullOuterJoin(dictB, x => x.Key, x => x.Key, (a, b, k) => new { Key = k, A = a, B = b });

            IDiffResult result = DiffResult.Same;

            foreach (var row in join)
            {
                if (row.A.Key == null)
                {
                    result = result.Merge(new DiffResult(new MissingKeyAnnotation(arrayA.Path, true)));
                }
                else if (row.B.Key == null)
                {
                    result = result.Merge(new DiffResult(new MissingKeyAnnotation(arrayA.Path, false)));
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
