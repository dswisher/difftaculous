
using Difftaculous.Paths;
using NUnit.Framework;


namespace Difftaculous.Test
{
    [TestFixture]
    public class JsonPathTests
    {
        // TODO - is matching symmetric?  Does a.Matches(b) => b.Matches(a)?
        //      NO!  Matching two paths should be removed; Matches should take an IToken!

        [Ignore("Get this working!")]
        [TestCase("$.countries[1].points")]
        [TestCase("$.countries[55].points")]
        // [TestCase("$.countries[sort='x'].points")]
        public void WildcardArrayMatch(string s)
        {
            var template = DiffPath.FromJsonPath("$.countries[*].points");
            var path = DiffPath.FromJsonPath(s);

            // TODO - how to write this with shouldly, in such a way that we see both paths on failure?
            // path.Matches(template).ShouldBe(true);

            Assert.That(template.Matches(path), string.Format("{0} matches {1}", template.AsJsonPath, path.AsJsonPath));
        }
    }
}
