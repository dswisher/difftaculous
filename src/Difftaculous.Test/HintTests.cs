using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Difftaculous.Adapters;
using Difftaculous.Paths;
using NUnit.Framework;
using Shouldly;

namespace Difftaculous.Test
{
    [TestFixture]
    public class HintTests
    {

        [Test]
        public void HintCanIgnoreItemInMiddleOfIndexedArray()
        {
            const string a = "[10.5, 20.7, 30.8, 40.3]";
            const string b = "[10.5, 20.3, 30.8, 40.3]";

            var adapterA = new JsonAdapter(a);
            var adapterB = new JsonAdapter(b);

            var settings = new DiffSettings().CanVaryBy(DiffPath.FromJsonPath("[1]"), 2);

            var result = DiffEngine.Compare(adapterA, adapterB, settings);

            foreach (var anno in result.Annotations)
            {
                Console.WriteLine("{0}: {1}", anno.Path, anno.Message);
            }

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void HintCanIgnoreLastItemOfIndexedArray()
        {
            const string a = "[10.5, 20.7, 30.8, 40.3]";
            const string b = "[10.5, 20.7, 30.8, 40.6]";

            var adapterA = new JsonAdapter(a);
            var adapterB = new JsonAdapter(b);

            var settings = new DiffSettings().CanVaryBy(DiffPath.FromJsonPath("[-1:]"), 2);

            var result = DiffEngine.Compare(adapterA, adapterB, settings);

            foreach (var anno in result.Annotations)
            {
                Console.WriteLine("{0}: {1}", anno.Path, anno.Message);
            }

            result.AreSame.ShouldBe(true);
        }


        // TODO - add hint test for keyed arrays
    }
}
