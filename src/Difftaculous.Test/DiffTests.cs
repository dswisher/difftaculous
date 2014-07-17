
using System.Linq;
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
            const string a = "{ \"hello\": \"World\" }";
            const string b = a;

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void AlteredValueResultsInOneDifference()
        {
            const string a = "{ \"hello\": \"World\" }";
            const string b = "{ \"hello\": \"There\" }";

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(false);

            result.Annotations.ShouldContain(x => x.Path.Equals(DiffPath.FromJsonPath("$.hello")));
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

            result.Annotations.ShouldContain(x => x.Path.Equals(DiffPath.FromJsonPath("$[1]")));
        }



        [Test]
        public void AlteredNestedPropertyResultsInOneDifference()
        {
            const string a = "{ \"fixture\": { \"title\": \"test A\" } }";
            const string b = "{ \"fixture\": { \"title\": \"test B\" } }";

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(false);

            result.Annotations.ShouldContain(x => x.Path.Equals(DiffPath.FromJsonPath("$.fixture.title")));
        }
    }
}
