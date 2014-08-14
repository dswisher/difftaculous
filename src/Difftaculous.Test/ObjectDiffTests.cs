
using System;
using Difftaculous.Adapters;
using Difftaculous.Results;
using Difftaculous.ZModel;


namespace Difftaculous.Test
{
    public class ObjectDiffTests : AbstractDiffTests
    {
        protected override IDiffResult DoCompare(object a, object b, DiffSettings settings = null)
        {
            var adapterA = new ObjectAdapter(a);
            var adapterB = new ObjectAdapter(b);

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
