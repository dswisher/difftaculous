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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Difftaculous.ZModel;


namespace Difftaculous.ArrayDiff
{
    internal class DiffArraySubsequencer : IArraySubsequencer
    {
        public List<ElementGroup> ComputeSubsequences(ZArray arrayA, ZArray arrayB)
        {
            List<ElementGroup> list = new List<ElementGroup>();

            // If both arrays are empty, this is easy...
            if ((arrayA.Count == 0) && (arrayB.Count == 0))
            {
                return list;
            }



            // Return what we've got...
            return list;
        }



        private int[,] ComputeArray(ZArray arrayA, ZArray arrayB)
        {
            // This uses the algorithm from http://en.wikipedia.org/wiki/Longest_common_subsequence_problem
#if false
function LCSLength(X[1..m], Y[1..n])
    C = array(0..m, 0..n)
    for i := 0..m
       C[i,0] = 0
    for j := 0..n
       C[0,j] = 0
    for i := 1..m
        for j := 1..n
            if X[i] = Y[j]
                C[i,j] := C[i-1,j-1] + 1
            else
                C[i,j] := max(C[i,j-1], C[i-1,j])
    return C[m,n]
#endif

            int[,] c = new int[arrayA.Count, arrayB.Count];

            for (int i = 0; i < arrayA.Count; i++)
            {
                c[i, 0] = 0;
            }

            for (int j = 0; j < arrayB.Count; j++)
            {
                c[0, j] = 0;
            }

            // TODO

            return c;
        }
    }
}
