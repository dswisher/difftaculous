
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
        public void AlteredValueResultsInOneDifference()
        {
            const string a = "{ \"Hello\": \"World\" }";
            const string b = "{ \"Hello\": \"There\" }";

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(false);
            // TODO - verify the differences were properly annotated
        }



        [Test]
        public void SimpleArrayComparedWithItselfHasNoDifferences()
        {
            const string a = "[\"Red\", \"Green\", \"Blue\"]";
            const string b = a;

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void AlteredSimpleArrayResultsInOneDifference()
        {
            const string a = "[\"Red\", \"Green\", \"Blue\"]";
            const string b = "[\"Red\", \"Black\", \"Blue\"]";

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(false);
            // TODO - verify the differences were properly annotated
        }



        [Test]
        public void AlteredNestedPropertyResultsInOneDifference()
        {
            const string a = "{ \"fixture\": { \"title\": \"test A\" } }";
            const string b = "{ \"fixture\": { \"title\": \"test B\" } }";

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(false);
            // TODO - verify the differences were properly annotated
        }
    }
}
