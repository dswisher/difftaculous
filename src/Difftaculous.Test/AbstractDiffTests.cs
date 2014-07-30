
using System;
using Difftaculous.Results;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test
{
    [TestFixture]
    public abstract class AbstractDiffTests
    {
        protected abstract IDiffResult DoCompare(object a, object b);


        protected class SimpleObject
        {
            public string Name { get; set; }
        }


        [Test]
        public void SimpleObjectComparedWithItselfHasNoDifferences()
        {
            SimpleObject a = new SimpleObject { Name = "Value" };
            SimpleObject b = a;

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(true);
        }



        #region Helpers

        protected string AsJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings {Formatting = Formatting.Indented});
        }

        protected string AsXml(object obj)
        {
            // TODO!
            throw new NotImplementedException("TBD");
        }

        #endregion
    }
}
