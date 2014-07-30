
using System.IO;
using System.Xml.Serialization;
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


        public class SimpleObject
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
            XmlSerializer cereal = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new StringWriter())
            {
                cereal.Serialize(writer, obj);

                return writer.ToString();
            }
        }

        #endregion
    }
}
