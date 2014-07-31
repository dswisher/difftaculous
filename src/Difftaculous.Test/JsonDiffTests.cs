
using System;

using Difftaculous.Adapters;
using Difftaculous.Results;
using NUnit.Framework;


namespace Difftaculous.Test
{
    public class JsonDiffTests : AbstractDiffTests
    {
        protected override IDiffResult DoCompare(object a, object b)
        {
            Assert.Ignore("TBD");

            var jsonA = AsJson(a);
            var jsonB = AsJson(b);

            Console.WriteLine("JSON, A:\n{0}", jsonA);
            Console.WriteLine();
            Console.WriteLine("JSON, B:\n{0}", jsonB);

            var result = Diff.Compare(new JsonAdapterEx(jsonA), new JsonAdapterEx(jsonB));

            Console.WriteLine();
            Console.WriteLine("Result:\n{0}", result);

            return result;
        }
    }
}
