
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
            var actual = Adapt("<thing></thing>");
            var expected = new ZObject();

            actual.ShouldBe(expected);
        }



        [Test]
        public void OneProp()
        {
            var actual = Adapt("<thing><name>Fred</name></thing>");
            var expected = new ZObject(new ZProperty("name", "Fred"));

            actual.ShouldBe(expected);
        }



        [Test]
        public void TwoProps()
        {
            var actual = Adapt("<thing><name>Fred</name><age>44</age></thing>");
            var expected = new ZObject(new ZProperty("name", "Fred"), new ZProperty("age", 44));

            actual.ShouldBe(expected);
        }




        private ZToken Adapt(string content)
        {
            var a = new XmlAdapter(content);

            // TODO - spew output?

            return (ZToken)a.Content.Content;
        }

    }
}
