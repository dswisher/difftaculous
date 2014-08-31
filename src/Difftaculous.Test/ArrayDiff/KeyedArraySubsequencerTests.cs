
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



        private static ZObject Make(string name, int value)
        {
            return new ZObject(new ZProperty("name", name), new ZProperty("value", value));
        }
    }
}
