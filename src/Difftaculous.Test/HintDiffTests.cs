using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Difftaculous.Adapters;
using Difftaculous.Caveats;
using Difftaculous.Hints;
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



        [Test, Ignore("Get this working!")]
        public void KeyedArrayDoesNotNeedSameOrder()
        {
            const string a = "[{ \"name\": \"fred\", \"age\": 44 }, { \"name\": \"barney\", \"age\": 23}]";
            const string b = "[{ \"name\": \"barney\", \"age\": 23 }, { \"name\": \"fred\", \"age\": 44 }]";

            // TODO - is $ the right JsonPath for this??
            // TODO - should the key be a Path as well?
            var hints = new[] { new ArrayDiffHint(DiffPath.FromJsonPath("$"), "name") };

            var result = Diff.Compare(new JsonAdapter(a), new JsonAdapter(b));

            result.AreSame.ShouldBe(true);
        }

    }
}
