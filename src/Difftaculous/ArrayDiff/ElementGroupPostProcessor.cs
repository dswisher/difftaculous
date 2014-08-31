using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Difftaculous.ArrayDiff
{
    internal static class ElementGroupPostProcessor
    {


        public static List<ElementGroup> PostProcess(List<ElementGroup> groups)
        {
            // TODO - only create a new list if we actually need to

#if false
            return groups;
#else
            List<ElementGroup> list = new List<ElementGroup>();

            for (int i = 0; i < groups.Count; i++)
            {
                if ((groups[i].Operation == Operation.Delete)
                    && (i < groups.Count - 1)
                    && (groups[i + 1].Operation == Operation.Insert)
                    && (groups[i].EndA - groups[i].StartA) == (groups[i + 1].EndB - groups[i + 1].StartB))
                {
                    // We have a delete followed by an insert of the same size.  Turn it into a Replace...
                    list.Add(ElementGroup.Replace(groups[i], groups[i+1]));
                    i += 1;
                }
                else
                {
                    list.Add(groups[i]);
                }
            }

            return list;
#endif
        }

    }
}
