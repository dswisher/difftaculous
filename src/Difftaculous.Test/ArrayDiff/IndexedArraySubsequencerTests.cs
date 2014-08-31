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

using System.Collections.Generic;
using System.Linq;
using Difftaculous.ArrayDiff;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;

namespace Difftaculous.Test.ArrayDiff
{
    [TestFixture]
    public class IndexedArraySubsequencerTests
    {
        private IArraySubsequencer _arraySubsequencer;


        [SetUp]
        public void BeforeEachTest()
        {
            _arraySubsequencer = new IndexedArraySubsequencer();
        }


        [Test]
        public void EmptyArrays()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray(""), StringToArray(""));

            list.ShouldBeEmpty();
        }



        [Test]
        public void SameArrays()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcd"), StringToArray("abcd"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 3, 0, 3)
            });
        }



        [Test]
        public void DifferentArrays()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcd"), StringToArray("wxyz"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Replace(0, 3, 0, 3)
            });
        }



        [Test]
        public void MismatchedSize()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcd"), StringToArray("xyz"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Delete(0, 3),
                ElementGroup.Insert(0, 2)
            });
        }



        [Test]
        public void Prefix()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcdef"), StringToArray("abcxyz"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 2, 0, 2),
                ElementGroup.Replace(3, 5, 3, 5)
            });
        }



        [Test]
        public void Suffix()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcdef"), StringToArray("xyzdef"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Replace(0, 2, 0, 2),
                ElementGroup.Equal(3, 5, 3, 5)
            });
        }



        [Test]
        public void PrefixAndSuffix()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcdefghi"), StringToArray("abcxyzghi"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 2, 0, 2),
                ElementGroup.Replace(3, 5, 3, 5),
                ElementGroup.Equal(6, 8, 6, 8),
            });
        }



        [Test]
        public void Middle()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcdefghi"), StringToArray("xyzdefxyz"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Replace(0, 2, 0, 2),
                ElementGroup.Equal(3, 5, 3, 5),
                ElementGroup.Replace(6, 8, 6, 8)
            });
        }



        private ZArray StringToArray(string s)
        {
            return new ZArray(s.ToCharArray().Select(x => x.ToString()));
        }
    }
}
