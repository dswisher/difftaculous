
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
        [TestCase("fred[1]")]
        [TestCase("[*]")]
        [TestCase("*")]
        [TestCase("*.*")]
        public void FromJsonAndBack(string jsonPath)
        {
            var path = DiffPath.FromJsonPath(jsonPath);
            path.AsJsonPath.ShouldBe(jsonPath);            
        }



        [TestCase("$.['*']", "['*']")]
        [TestCase("$.*", "*")]
        public void FromJsonAndBackWithTwist(string inPath, string outPath)
        {
            var path = DiffPath.FromJsonPath(inPath);
            path.AsJsonPath.ShouldBe(outPath);
        }
    }
}
