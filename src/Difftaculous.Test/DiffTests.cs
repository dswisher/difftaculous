
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test
{
    [TestFixture]
    public class DiffTests
    {

        [Test]
        public void SimpleObjectComparedWithItselfHasNoDifferences()
        {
            const string a = "{ \"Hello\": \"World\" }";
            const string b = a;

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void ChangedValueResultsInOneDifference()
        {
            const string a = "{ \"Hello\": \"World\" }";
            const string b = "{ \"Hello\": \"There\" }";

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(false);
            // TODO - verify the difference is in the field value
        }



        [Test]
        public void SimpleArrayComparedWithItselfHasNoDifferences()
        {
            const string a = "[\"Red\", \"Green\", \"Blue\"]";
            const string b = a;

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(true);
        }
    }
}
