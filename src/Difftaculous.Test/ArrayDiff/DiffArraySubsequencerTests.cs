using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



        [Test, Ignore("Finish this!")]
        public void SameArrays()
        {
            var list = _arraySubsequencer.ComputeSubsequences(StringToArray("abc"), StringToArray("abc"));

            list.ShouldBe(new List<ElementGroup>
            {
                ElementGroup.Equal(0, 2, 0, 2)
            });
        }



        [Test, Ignore("Finish this!")]
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



#if false
            diffs = new List<Diff> { new Diff(Operation.EQUAL, "ab"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "c") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("abc", "ab123c", false), "diff_main: Simple insertion.");

            diffs = new List<Diff> { new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "bc") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("a123bc", "abc", false), "diff_main: Simple deletion.");

            diffs = new List<Diff> { new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.INSERT, "456"), new Diff(Operation.EQUAL, "c") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("abc", "a123b456c", false), "diff_main: Two insertions.");

            diffs = new List<Diff> { new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "456"), new Diff(Operation.EQUAL, "c") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("a123b456c", "abc", false), "diff_main: Two deletions.");

            // Perform a real diff.
            // Switch off the timeout.
            dmp.Diff_Timeout = 0;
            diffs = new List<Diff> { new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "b") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("a", "b", false), "diff_main: Simple case #1.");

            diffs = new List<Diff> { new Diff(Operation.DELETE, "Apple"), new Diff(Operation.INSERT, "Banana"), new Diff(Operation.EQUAL, "s are a"), new Diff(Operation.INSERT, "lso"), new Diff(Operation.EQUAL, " fruit.") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("Apples are a fruit.", "Bananas are also fruit.", false), "diff_main: Simple case #2.");

            diffs = new List<Diff> { new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "\u0680"), new Diff(Operation.EQUAL, "x"), new Diff(Operation.DELETE, "\t"), new Diff(Operation.INSERT, new string(new char[] { (char)0 })) };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("ax\t", "\u0680x" + (char)0, false), "diff_main: Simple case #3.");

            diffs = new List<Diff> { new Diff(Operation.DELETE, "1"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "y"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "2"), new Diff(Operation.INSERT, "xab") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("1ayb2", "abxab", false), "diff_main: Overlap #1.");

            diffs = new List<Diff> { new Diff(Operation.INSERT, "xaxcx"), new Diff(Operation.EQUAL, "abc"), new Diff(Operation.DELETE, "y") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("abcy", "xaxcxabc", false), "diff_main: Overlap #2.");

            diffs = new List<Diff> { new Diff(Operation.DELETE, "ABCD"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "="), new Diff(Operation.INSERT, "-"), new Diff(Operation.EQUAL, "bcd"), new Diff(Operation.DELETE, "="), new Diff(Operation.INSERT, "-"), new Diff(Operation.EQUAL, "efghijklmnopqrs"), new Diff(Operation.DELETE, "EFGHIJKLMNOefg") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("ABCDa=bcd=efghijklmnopqrsEFGHIJKLMNOefg", "a-bcd-efghijklmnopqrs", false), "diff_main: Overlap #3.");

            diffs = new List<Diff> { new Diff(Operation.INSERT, " "), new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "nd"), new Diff(Operation.EQUAL, " [[Pennsylvania]]"), new Diff(Operation.DELETE, " and [[New") };
            CollectionAssert.AreEqual(diffs, dmp.diff_main("a [[Pennsylvania]] and [[New", " and [[Pennsylvania]]", false), "diff_main: Large equality.");
#endif



        private ZArray StringToArray(string s)
        {
            return new ZArray(s.ToCharArray().Select(x => x.ToString()));
        }
    }
}
