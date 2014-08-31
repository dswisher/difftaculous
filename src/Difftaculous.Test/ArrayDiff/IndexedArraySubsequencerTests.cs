
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
                ElementGroup.Delete(0, 3),
                ElementGroup.Insert(0, 3)
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
                ElementGroup.Delete(3, 5),
                ElementGroup.Insert(3, 5)
            });
        }



        [Test]
        public void Suffix()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcdef"), StringToArray("xyzdef"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Delete(0, 2),
                ElementGroup.Insert(0, 2),
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
                ElementGroup.Delete(3, 5),
                ElementGroup.Insert(3, 5),
                ElementGroup.Equal(6, 8, 6, 8),
            });
        }



        [Test]
        public void Middle()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abcdefghi"), StringToArray("xyzdefxyz"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Delete(0, 2),
                ElementGroup.Insert(0, 2),
                ElementGroup.Equal(3, 5, 3, 5),
                ElementGroup.Delete(6, 8),
                ElementGroup.Insert(6, 8)
            });
        }



        private ZArray StringToArray(string s)
        {
            return new ZArray(s.ToCharArray().Select(x => x.ToString()));
        }
    }
}
