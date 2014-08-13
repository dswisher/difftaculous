
using System;
using Difftaculous.Adapters;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.Adapters
{
    [TestFixture]
    public class XmlAdapterTests
    {

        [Test]
        public void EmptyElement()
        {
            const string xml = "<thing></thing>";
            var expected = new ZObject();

            RunTest(expected, xml);
        }



        [Test]
        public void OneProp()
        {
            const string xml = "<thing><name>Fred</name></thing>";
            var expected = new ZObject(new ZProperty("name", "Fred"));

            RunTest(expected, xml);
        }



        [Test]
        public void TwoProps()
        {
            const string xml = "<thing><name>Fred</name><age>44</age></thing>";
            var expected = new ZObject(new ZProperty("name", "Fred"), new ZProperty("age", 44));

            RunTest(expected, xml);
        }



        [Test]
        public void SimpleArray()
        {
            const string xml = "<team><player>Walt</player><player>Joe</player><player>Fred</player></team>";
            var expected = new ZArray("Walt", "Joe", "Fred");

            RunTest(expected, xml);
        }



        [Test, Ignore("Get this working!")]
        public void ArrayWithComplexTypes()
        {
            const string xml = "<team><player><name>Walt</name></player><player><name>Fred</name></player></team>";
            var walt = new ZObject(new ZProperty("name", "Walt"));
            var fred = new ZObject(new ZProperty("name", "Fred"));
            var expected = new ZArray(walt, fred);

            RunTest(expected, xml);
        }



        private void RunTest(ZToken expected, string xml)
        {
            ZToken actual = new XmlAdapter(xml).Content;

            Console.WriteLine("Expected:");
            Console.WriteLine(expected.AsJson());

            Console.WriteLine();

            Console.WriteLine("Actual:");
            Console.WriteLine(actual.AsJson());

            actual.ShouldBe(expected);
        }
    }
}
