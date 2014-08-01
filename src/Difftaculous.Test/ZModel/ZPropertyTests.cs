
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;

namespace Difftaculous.Test.ZModel
{
    [TestFixture]
    public class ZPropertyTests
    {

        [Test]
        public void ParentIsSetOnPropertyWhenAddingToObject()
        {
            ZObject o = new ZObject();
            ZProperty p = new ZProperty("foo", new ZValue(1));

            o.AddProperty(p);

            p.Parent.ShouldBe(o);
        }

    }
}
