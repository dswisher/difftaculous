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
using Difftaculous.Adapters;
using Difftaculous.Results;
using Difftaculous.ZModel;


namespace Difftaculous.Test
{
    public class ObjectDiffTests : AbstractDiffTests
    {
        protected override IDiffResult DoCompare(object a, object b, DiffSettings settings = null)
        {
            var adapterA = new ObjectAdapter(a);
            var adapterB = new ObjectAdapter(b);

            Console.WriteLine("Z-JSON, A:\n{0}", adapterA.Content.AsJson());
            Console.WriteLine();
            Console.WriteLine("Z-JSON, B:\n{0}", adapterB.Content.AsJson());
            Console.WriteLine();

            var result = DiffEngine.Compare(adapterA, adapterB, settings);

            Console.WriteLine("Result:\n{0}", result);

            return result;
        }
    }
}
