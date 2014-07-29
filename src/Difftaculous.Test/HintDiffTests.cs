
using Difftaculous.Adapters;
using Difftaculous.Hints;
using Difftaculous.Paths;
using NUnit.Framework;

using Shouldly;


namespace Difftaculous.Test
{
    [TestFixture]
    public class HintDiffTests
    {

        [Test]
        public void IndexedArrayNeedsSameOrder()
        {
            const string a = "[{ \"name\": \"fred\", \"age\": 44 }, { \"name\": \"barney\", \"age\": 23}]";
            const string b = "[{ \"name\": \"barney\", \"age\": 23 }, { \"name\": \"fred\", \"age\": 44 }]";

            // TODO - should we explicitly specify indexed as the strategy, in case the default changes??

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(false);
        }



        [Test]
        public void KeyedArrayDoesNotNeedSameOrder()
        {
            const string a = "[{ \"name\": \"fred\", \"age\": 44 }, { \"name\": \"barney\", \"age\": 23}]";
            const string b = "[{ \"name\": \"barney\", \"age\": 23 }, { \"name\": \"fred\", \"age\": 44 }]";

            // TODO - is $ the right JsonPath for this??
            // TODO - should the key be a Path as well?
            var hints = new[] { new ArrayDiffHint(DiffPath.FromJsonPath("$"), "name") };

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b), hints);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void KeyedArrayStillNeedsToMatch()
        {
            const string a = "[{ \"name\": \"fred\", \"age\": 44 }, { \"name\": \"barney\", \"age\": 23}]";
            const string b = "[{ \"name\": \"barney\", \"age\": 33 }, { \"name\": \"fred\", \"age\": 44 }]";

            // TODO - is $ the right JsonPath for this??
            // TODO - should the key be a Path as well?
            var hints = new[] { new ArrayDiffHint(DiffPath.FromJsonPath("$"), "name") };

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b), hints);

            result.AreSame.ShouldBe(false);
            result.Annotations.ShouldContain(x => x.Path.Equals(DiffPath.FromJsonPath("$['barney'].age")));
        }
    }
}
