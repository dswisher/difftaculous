
using System.Collections.Generic;
using Difftaculous.ZModel;


namespace Difftaculous.ArrayDiff
{
    internal class IndexedArraySubsequencer : IArraySubsequencer
    {
        public List<ElementGroup> ComputeSubsequences(ZArray arrayA, ZArray arrayB)
        {
            List<ElementGroup> list = new List<ElementGroup>();

            if (arrayA.Count != arrayB.Count)
            {
                list.Add(ElementGroup.Delete(0, arrayA.Count - 1));
                list.Add(ElementGroup.Insert(0, arrayB.Count - 1));
            }
            else
            {
                for (int i = 0; i < arrayA.Count; i++)
                {
                    var itemA = arrayA[i];
                    var itemB = arrayB[i];

                    if (itemA.Equals(itemB))
                    {
                        // Match - can we just extend a prior match?
                        if ((list.Count >= 1) && (list[list.Count - 1].Operation == Operation.Equal))
                        {
                            list[list.Count - 1].Extend(1);
                        }
                        else
                        {
                            list.Add(ElementGroup.Equal(i, i, i, i));
                        }
                    }
                    else
                    {
                        // Not a match - can we extend a prior delete/insert pair?
                        if ((list.Count >= 2)
                            && (list[list.Count - 2].Operation == Operation.Delete)
                            && (list[list.Count - 1].Operation == Operation.Insert))
                        {
                            list[list.Count - 2].Extend(1);
                            list[list.Count - 1].Extend(1);
                        }
                        else
                        {
                            list.Add(ElementGroup.Delete(i, i));
                            list.Add(ElementGroup.Insert(i, i));
                        }
                    }
                }
            }

            return list;
        }
    }
}
