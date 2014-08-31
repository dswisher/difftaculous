
using System;
using System.Collections.Generic;
using System.Linq;
using Difftaculous.Misc;
using Difftaculous.ZModel;


namespace Difftaculous.ArrayDiff
{
    internal class KeyedArraySubsequencer : IArraySubsequencer
    {
        private readonly string _key;


        public KeyedArraySubsequencer(string key)
        {
            _key = key;
        }



        public List<ElementGroup> ComputeSubsequences(ZArray arrayA, ZArray arrayB)
        {
            // This only works on arrays that contain objects (they're the only things
            // that have properties we can use as keys).  If we have non-objects, we have
            // a difference (or perhaps an error, or ??).
            if (arrayA.Any(x => x.Type != TokenType.Object) || arrayB.Any(x => x.Type != TokenType.Object))
            {
                throw new NotImplementedException("Keyed-array diff for non-objects is not implemented.");
            }

            var dictA = MakeDict(arrayA);
            var dictB = MakeDict(arrayB);

            var join = dictA.FullOuterJoin(dictB, x => x.Key, x => x.Key, (a, b, k) => new { Key = k, A = a, B = b })
                .OrderBy(x => x.A.Value.Index);

            List<ElementGroup> list = new List<ElementGroup>();

            foreach (var row in join)
            {
                int aIndex = row.A.Value.Index;
                int bIndex = row.B.Value.Index;

                if (row.A.Key == null)
                {
                    // TODO - delete
                    throw new NotImplementedException();
                }
                else if (row.B.Key == null)
                {
                    // TODO - insert
                    throw new NotImplementedException();
                }
                else
                {
                    if (row.A.Value.Token.Equals(row.B.Value.Token))
                    {
                        var prev = (list.Count >= 1) ? list[list.Count - 1] : null;

                        if ((prev != null)
                            && (prev.Operation == Operation.Equal)
                            && (prev.EndA == aIndex - 1)
                            && (prev.EndA == bIndex - 1))
                        {
                            prev.Extend(1);
                        }
                        else
                        {
                            list.Add(ElementGroup.Equal(aIndex, aIndex, bIndex, bIndex));
                        }
                    }
                    else
                    {
                        // TODO - keys match, values are different
                        throw new NotImplementedException();
                    }
                }
            }

            return list;
        }



        private Dictionary<string, Entry> MakeDict(ZArray array)
        {
            var foo = array
                .Select((z, i) => new Entry
                {
                    Index = i,
                    Token = z,
                    Key = (string) ((ZValue) (((ZObject) z).Property(_key, false)).Value)
                })
                .ToDictionary(x => x.Key);

            return foo;
        }



        private class Entry
        {
            public int Index { get; set; }
            public string Key { get; set; }
            public ZToken Token { get; set; }
        }
    }
}
