using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Difftaculous.ZModel;

namespace Difftaculous.ArrayDiff
{
    internal interface IArraySubsequencer
    {
        List<ElementGroup> ComputeSubsequences(ZArray arrayA, ZArray arrayB);
    }
}
