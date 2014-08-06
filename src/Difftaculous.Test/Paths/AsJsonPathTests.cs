
using Difftaculous.Paths;
using NUnit.Framework;
using Shouldly;

namespace Difftaculous.Test.Paths
{
    [TestFixture]
    public class AsJsonPathTests
    {
        [TestCase("name")]
        [TestCase("person.name")]
        [TestCase("[1]")]
        [TestCase("[1].fred")]
        [TestCase("[*]")]
        //[TestCase("fred[1]")]
        public void FromJsonAndBack(string jsonPath)
        {
            var path = DiffPath.FromJsonPath(jsonPath);
            path.AsJsonPathEx.ShouldBe(jsonPath);            
        }
    }
}
