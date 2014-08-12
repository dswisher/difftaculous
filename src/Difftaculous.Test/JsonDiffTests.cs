
using System;
using Difftaculous.Adapters;
using Difftaculous.Results;
using Difftaculous.ZModel;


namespace Difftaculous.Test
{
    public class JsonDiffTests : AbstractDiffTests
    {
        protected override IDiffResult DoCompare(object a, object b, DiffSettings settings = null)
        {
            var jsonA = AsJson(a);
            var jsonB = AsJson(b);

            Console.WriteLine("JSON, A:\n{0}", jsonA);
            Console.WriteLine();
            Console.WriteLine("JSON, B:\n{0}", jsonB);
            Console.WriteLine();

            var adapterA = new JsonAdapter(jsonA);
            var adapterB = new JsonAdapter(jsonB);

            Console.WriteLine("Z-JSON, A:\n{0}", adapterA.Content.AsJson());
            Console.WriteLine();
            Console.WriteLine("Z-JSON, A:\n{0}", adapterB.Content.AsJson());
            Console.WriteLine();

            var result = DiffEngine.Compare(adapterA, adapterB, settings);

            Console.WriteLine("Result:\n{0}", result);

            return result;
        }
    }
}
