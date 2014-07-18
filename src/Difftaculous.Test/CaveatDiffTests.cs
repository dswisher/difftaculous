using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Difftaculous.Adapters;
using Difftaculous.Caveats;
using NUnit.Framework;
using Shouldly;

namespace Difftaculous.Test
{
    [TestFixture]
    public class CaveatDiffTests
    {

        //[Test]
        [Test, Ignore("Focus on this!")]
        public void PropertyIsAllowedToVary()
        {
            const string a = "{ \"score\": 100 }";
            const string b = "{ \"score\": 99 }";

            // TODO - should have a nice, fluent interface to build caveats
            var caveats = new[] { new VarianceCaveat(DiffPath.FromJsonPath("$.score"), 5) };

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b), caveats);

            result.AreSame.ShouldBe(true);
        }

    }
}
