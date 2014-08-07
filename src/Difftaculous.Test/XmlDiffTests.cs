
using System;
using System.Collections.Generic;
using Difftaculous.Adapters;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Results;

using NUnit.Framework;


namespace Difftaculous.Test
{
    public class XmlDiffTests : AbstractDiffTests
    {
        protected override IDiffResult DoCompare(object a, object b, IEnumerable<ICaveat> caveats, IEnumerable<IHint> hints)
        {
            Assert.Ignore("TBD");

            var xmlA = AsXml(a);
            var xmlB = AsXml(b);

            Console.WriteLine("XML, A:\n{0}", xmlA);
            Console.WriteLine();
            Console.WriteLine("XML, B:\n{0}", xmlB);

            var result = DiffEngine.Compare(new XmlAdapter(xmlA), new XmlAdapter(xmlB), caveats, hints);

            Console.WriteLine();
            Console.WriteLine("Result:\n{0}", result);

            return result;
        }
    }
}
