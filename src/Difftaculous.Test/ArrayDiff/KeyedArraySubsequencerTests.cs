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
using Difftaculous.ArrayDiff;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.ArrayDiff
{
    [TestFixture]
    public class KeyedArraySubsequencerTests
    {
        private IArraySubsequencer _arraySubsequencer;

        [SetUp]
        public void BeforeEachTest()
        {
            _arraySubsequencer = new KeyedArraySubsequencer("name");
        }


        [Test]
        public void EmptyArrays()
        {
            var list = _arraySubsequencer.ComputeSubsequences(new ZArray(), new ZArray());

            list.ShouldBeEmpty();
        }



        [Test]
        public void SameArrays()
        {
            var a = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Wilma", 32));
            var b = a;

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 2, 0, 2)
            });
        }



        [Test]
        public void BackwardsArrays()
        {
            var a = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Wilma", 32));
            var b = new ZArray(Make("Wilma", 32), Make("Barney", 22), Make("Fred", 23));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 2, 2),
                ElementGroup.Equal(1, 1, 1, 1),
                ElementGroup.Equal(2, 2, 0, 0)
            });
        }



        [Test]
        public void DifferentMiddle()
        {
            var a = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Wilma", 32));
            var b = new ZArray(Make("Wilma", 32), Make("Barney", 23), Make("Fred", 23));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 2, 2),
                ElementGroup.Replace(1, 1, 1, 1),
                ElementGroup.Equal(2, 2, 0, 0)
            });
        }



        [Test]
        public void DifferentMiddleTwo()
        {
            var a = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Pebbles", 4), Make("Wilma", 32));
            var b = new ZArray(Make("Fred", 23), Make("Barney", 24), Make("Pebbles", 5), Make("Wilma", 32));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Replace(1, 2, 1, 2),
                ElementGroup.Equal(3, 3, 3, 3)
            });
        }



        [Test]
        public void DifferentBackwardMiddleTwo()
        {
            var a = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Pebbles", 4), Make("Wilma", 32));
            var b = new ZArray(Make("Fred", 23), Make("Pebbles", 5), Make("Barney", 24), Make("Wilma", 32));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Replace(1, 1, 2, 2),
                ElementGroup.Replace(2, 2, 1, 1),
                ElementGroup.Equal(3, 3, 3, 3)
            });
        }



        [Test]
        public void RemovedOneMiddle()
        {
            var a = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Pebbles", 4), Make("Wilma", 32));
            var b = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Wilma", 32));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 1, 0, 1),
                ElementGroup.Delete(2, 2),
                ElementGroup.Equal(3, 3, 2, 2)
            });
        }



        [Test]
        public void RemovedTwoMiddle()
        {
            var a = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Pebbles", 4), Make("Wilma", 32));
            var b = new ZArray(Make("Fred", 23), Make("Wilma", 32));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Delete(1, 2),
                ElementGroup.Equal(3, 3, 1, 1)
            });
        }



        [Test]
        public void AddedOneMiddle()
        {
            var a = new ZArray(Make("Fred", 23), Make("Wilma", 32));
            var b = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Wilma", 32));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Equal(1, 1, 2, 2),
                ElementGroup.Insert(1, 1)
            });
        }



        [Test]
        public void AddedTwoMiddle()
        {
            var a = new ZArray(Make("Fred", 23), Make("Wilma", 32));
            var b = new ZArray(Make("Fred", 23), Make("Barney", 22), Make("Pebbles", 4), Make("Wilma", 32));

            var list = _arraySubsequencer.ComputeSubsequences(a, b);

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 0, 0, 0),
                ElementGroup.Equal(1, 1, 3, 3),
                ElementGroup.Insert(1, 2)
            });
        }



        private static ZObject Make(string name, int value)
        {
            return new ZObject(new ZProperty("name", name), new ZProperty("value", value));
        }
    }
}
