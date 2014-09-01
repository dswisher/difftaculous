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
    public class DiffArraySubsequencerTests
    {
        private IArraySubsequencer _arraySubsequencer;


        [SetUp]
        public void BeforeEachTest()
        {
            _arraySubsequencer = new DiffArraySubsequencer();
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
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abc"), StringToArray("abc"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 2, 0, 2)
            });
        }



        [Test]
        public void SimpleInsertion()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abc"), StringToArray("ab123c"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 1, 0, 1),
                ElementGroup.Insert(2, 4),
                ElementGroup.Equal(2, 2, 5, 5)
            });
        }



        [Test]
        public void SimpleDeletion()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("a123bc"), StringToArray("abc"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Delete(1, 3),
                ElementGroup.Equal(4, 5, 1, 2)
            });
        }



        [Test]
        public void TwoInsertions()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abc"), StringToArray("a123b456c"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Insert(1, 3),
                ElementGroup.Equal(1, 1, 4, 4),
                ElementGroup.Insert(5, 7),
                ElementGroup.Equal(2, 2, 8, 8)
            });
        }



        [Test]
        public void TwoDeletion()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("a123b456c"), StringToArray("abc"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Delete(1, 3),
                ElementGroup.Equal(4, 4, 1, 1),
                ElementGroup.Delete(5, 7),
                ElementGroup.Equal(8, 8, 2, 2)
            });
        }



        [Test]
        public void Replace()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("ab"), StringToArray("cd"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Replace(0, 1, 0, 1)
            });
        }



        private ZArray StringToArray(string s)
        {
            return new ZArray(s.ToCharArray().Select(x => x.ToString()));
        }
    }
}
