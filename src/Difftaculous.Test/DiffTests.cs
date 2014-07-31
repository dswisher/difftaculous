
using Difftaculous.Adapters;
using Difftaculous.Paths;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test
{
    [TestFixture]
    public class DiffTests
    {

        // TODO - these tests should all be removed once everything has been moved over to AbstractDiffTests

        [Test]
        public void SimpleObjectComparedWithItselfHasNoDifferences()
        {
            const string a = "{ \"hello\": \"World\" }";
            const string b = a;

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void NullValuesAreTolerated()
        {
            const string a = "{ \"hello\": null }";
            const string b = a;

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void AlteredValueResultsInOneDifference()
        {
            const string a = "{ \"hello\": \"World\" }";
            const string b = "{ \"hello\": \"There\" }";

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(false);

            result.Annotations.ShouldContain(x => x.Path.Equals(DiffPath.FromJsonPath("$.hello")));
        }



        [Test]
        public void SimpleArrayComparedWithItselfHasNoDifferences()
        {
            const string a = "[\"Red\", \"Green\", \"Blue\"]";
            const string b = a;

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void AlteredSimpleArrayResultsInOneDifference()
        {
            const string a = "[\"Red\", \"Green\", \"Blue\"]";
            const string b = "[\"Red\", \"Black\", \"Blue\"]";

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(false);

            result.Annotations.ShouldContain(x => x.Path.Equals(DiffPath.FromJsonPath("$[1]")));
        }



        [Test]
        public void AlteredNestedPropertyResultsInOneDifference()
        {
            const string a = "{ \"fixture\": { \"title\": \"test A\" } }";
            const string b = "{ \"fixture\": { \"title\": \"test B\" } }";

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(false);

            result.Annotations.ShouldContain(x => x.Path.Equals(DiffPath.FromJsonPath("$.fixture.title")));
        }



        [Test]
        public void ReorderedPropertiesAreSame()
        {
            const string a = "{ \"prop1\": \"val1\", \"prop2\": \"val2\" }";
            const string b = "{ \"prop2\": \"val2\", \"prop1\": \"val1\" }";

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(true);
        }
    }
}
