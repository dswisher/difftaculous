
using System;
using Difftaculous.Adapters;
using Difftaculous.Results;
using Difftaculous.ZModel;


namespace Difftaculous.Test
{
    public class XmlDiffTests : AbstractDiffTests
    {
        protected override IDiffResult DoCompare(object a, object b, DiffSettings settings = null)
        {
            var xmlA = AsXml(a);
            var xmlB = AsXml(b);

            Console.WriteLine("XML, A:\n{0}", xmlA);
            Console.WriteLine();
            Console.WriteLine("XML, B:\n{0}", xmlB);
            Console.WriteLine();

            var adapterA = new XmlAdapter(xmlA);
            var adapterB = new XmlAdapter(xmlB);

            Console.WriteLine("Z-JSON, A:\n{0}", adapterA.Content.AsJson());
            Console.WriteLine();
            Console.WriteLine("Z-JSON, B:\n{0}", adapterB.Content.AsJson());
            Console.WriteLine();

            var result = DiffEngine.Compare(adapterA, adapterB, settings);

            Console.WriteLine("Result:\n{0}", result);

            return result;
        }
    }
}
