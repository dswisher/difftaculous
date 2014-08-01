
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;

namespace Difftaculous.Test.ZModel
{
    [TestFixture]
    public class ZValueTests
    {

        [Test]
        public void ParentIsSetOnValueWhenAddingToProperty()
        {
            ZValue v = new ZValue(1);
            ZProperty p = new ZProperty("foo", v);

            v.Parent.ShouldBe(p);
        }

    }
}
