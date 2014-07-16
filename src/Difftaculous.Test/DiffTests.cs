
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test
{
    [TestFixture]
    public class DiffTests
    {

        [Test]
        public void SimpleObjectComparedWithItself()
        {
            string a = "{ \"Hello\": \"World\" }";
            string b = a;

            var result = Diff.Json(a, b);

            result.AreSame.ShouldBe(true);
        }

    }
}
