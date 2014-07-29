
using Difftaculous.Adapters;
using Difftaculous.Caveats;
using Difftaculous.Paths;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test
{
    [TestFixture]
    public class CaveatDiffTests
    {

        [Test]
        public void PropertyIsAllowedToVary()
        {
            const string a = "{ \"score\": 100 }";
            const string b = "{ \"score\": 99 }";

            // TODO - should have a nice, fluent interface to build caveats
            var caveats = new[] { new VarianceCaveat(DiffPath.FromJsonPath("$.score"), 2) };

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b), caveats);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void PropertyOutsideVarianceIsNotSame()
        {
            const string a = "{ \"score\": 100 }";
            const string b = "{ \"score\": 110 }";

            var caveats = new[] { new VarianceCaveat(DiffPath.FromJsonPath("$.score"), 2) };

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b), caveats);

            result.AreSame.ShouldBe(false);
        }
    }
}
