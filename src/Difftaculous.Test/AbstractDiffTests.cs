
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Difftaculous.Paths;
using Difftaculous.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Shouldly;

// ReSharper disable PossibleNullReferenceException


namespace Difftaculous.Test
{
    [TestFixture]
    public abstract class AbstractDiffTests
    {
        protected abstract IDiffResult DoCompare(object a, object b, DiffSettings settings = null);


        public class EmptyObject 
        {
        }


        [Test]
        public void TwoEmptyObjectsAreSame()
        {
            var a = new EmptyObject();
            var b = new EmptyObject();

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(true);
        }



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



        [Test]
        public void NullValuesAreTolerated()
        {
            SimpleObject a = new SimpleObject { Name = null };
            SimpleObject b = a;

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void AlteredValueResultsInOneDifference()
        {
            SimpleObject a = new SimpleObject { Name = "One" };
            SimpleObject b = new SimpleObject { Name = "Two" };

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(false);
            result.Annotations.Count().ShouldBe(1);

            var anno = result.Annotations.First() as DifferingValuesAnnotation;

            anno.ShouldNotBe(null);
            anno.Path.AsJsonPath.ShouldBe("name", Case.Insensitive);
            anno.ValueA.ShouldBe("One");
            anno.ValueB.ShouldBe("Two");
        }



        public class AnotherSimpleObject
        {
            public string Address { get; set; }
        }


        [Test]
        public void MismatchedPropertiesAreDifferent()
        {
            SimpleObject a = new SimpleObject { Name = "One" };
            AnotherSimpleObject b = new AnotherSimpleObject { Address = "1 Main St." };

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(false);
            result.Annotations.Count().ShouldBe(2);

            var anno1 = result.Annotations.FirstOrDefault(x => ((MissingPropertyAnnotation)x).PropertyName.Equals("name", StringComparison.InvariantCultureIgnoreCase));
            var anno2 = result.Annotations.FirstOrDefault(x => ((MissingPropertyAnnotation)x).PropertyName.Equals("address", StringComparison.InvariantCultureIgnoreCase));

            anno1.ShouldNotBe(null);
            anno1.Path.AsJsonPath.ShouldBe("");

            anno2.ShouldNotBe(null);
            anno2.Path.AsJsonPath.ShouldBe("");
        }



        [Test]
        public void SimpleArrayComparedWithItselfHasNoDifferences()
        {
            string[] a = { "Red", "Green", "Blue" };
            string[] b = new string[3];
            Array.Copy(a, b, 3);

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void AlteredSimpleArrayResultsInOneDifference()
        {
            string[] a = { "Red", "Green", "Blue" };
            string[] b = { "Red", "Black", "Blue" };

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(false);

            // TODO - verify annotation!
            result.Annotations.ShouldContain(x => x.Path.AsJsonPath == "[1]");
        }





        public class NestedObject
        {
            public SimpleObject Thing { get; set; }
        }



        [Test]
        public void AlteredNestedPropertyResultsInOneDifference()
        {
            var a = new NestedObject { Thing = new SimpleObject { Name = "Fred" } };
            var b = new NestedObject { Thing = new SimpleObject { Name = "Barney" } };

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(false);
            result.Annotations.ShouldContain(x => x.Path.AsJsonPath.Equals("thing.name", StringComparison.InvariantCultureIgnoreCase));
        }




        public class TwoProp
        {
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
        }


        public class TwoPropRev
        {
            public string Prop2 { get; set; }
            public string Prop1 { get; set; }
        }



        [Test]
        public void ReorderedPropertiesAreSame()
        {
            var a = new TwoProp {Prop1 = "val1", Prop2 = "val2"};
            var b = new TwoPropRev { Prop1 = "val1", Prop2 = "val2" };

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(true);
        }




        public class ScoreClass
        {
            public int Score { get; set; }
        }



        [Test]
        public void HintedPropertyIsAllowedToVary()
        {
            var a = new ScoreClass { Score = 100 };
            var b = new ScoreClass { Score = 99 };

            var settings = new DiffSettings().Item(DiffPath.FromJsonPath("$.score")).CanVaryBy(2);

            var result = DoCompare(a, b, settings);

            result.AreSame.ShouldBe(true);
            result.Annotations.Count().ShouldBe(1);

            var anno = result.Annotations.First() as DifferingValuesAnnotation;

            anno.ShouldNotBe(null);
            // TODO - the values should be 100 and 99 - not strings.  Fix that!
            anno.ValueA.ShouldBe("100");
            anno.ValueB.ShouldBe("99");
            anno.Path.AsJsonPath.ShouldBe("score", Case.Insensitive);
        }



        [Test]
        public void PropertyOutsideVarianceIsNotSame()
        {
            var a = new ScoreClass { Score = 100 };
            var b = new ScoreClass { Score = 110 };

            var settings = new DiffSettings().Item(DiffPath.FromJsonPath("$.score")).CanVaryBy(2);

            var result = DoCompare(a, b, settings);

            result.AreSame.ShouldBe(false);
        }



        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }



        [Test]
        public void IndexedArrayNeedsSameOrder()
        {
            var a = new[] { new Person { Name = "Fred", Age = 44 }, new Person { Name = "Barney", Age = 23 } };
            var b = new[] { new Person { Name = "Barney", Age = 23 }, new Person { Name = "Fred", Age = 44 } };

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(false);
            // TODO - verify annotation(s)
        }



        [Test]
        public void IndexedArrayNeedsSameCount()
        {
            var a = new[] { 1, 3, 5, 7 };
            var b = new[] { 1, 2, 3 };

            var result = DoCompare(a, b);

            result.AreSame.ShouldBe(false);
            result.Annotations.Count().ShouldBe(1);

            var anno = result.Annotations.First() as DifferingArrayCountAnnotation;

            anno.ShouldNotBe(null);
            anno.Path.AsJsonPath.ShouldBe("");
            anno.CountA.ShouldBe(4);
            anno.CountB.ShouldBe(3);
        }



        [Test]
        public void KeyedArrayDoesNotNeedSameOrder()
        {
            var a = new[] { new Person { Name = "Fred", Age = 44 }, new Person { Name = "Barney", Age = 23 } };
            var b = new[] { new Person { Name = "Barney", Age = 23 }, new Person { Name = "Fred", Age = 44 } };


            var settings = new DiffSettings().Array(DiffPath.FromJsonPath("$")).KeyedBy("name");

            var result = DoCompare(a, b, settings);

            result.AreSame.ShouldBe(true);
        }



        [Test]
        public void KeyedArrayStillNeedsToMatch()
        {
            var a = new[] { new Person { Name = "Fred", Age = 44 }, new Person { Name = "Barney", Age = 23 } };
            var b = new[] { new Person { Name = "Barney", Age = 33 }, new Person { Name = "Fred", Age = 44 } };

            var settings = new DiffSettings().Array(DiffPath.FromJsonPath("$")).KeyedBy("name");

            var result = DoCompare(a, b, settings);

            result.AreSame.ShouldBe(false);
            // TODO - need both paths in this annotation...maybe?
            result.Annotations.ShouldContain(x => x.Path.AsJsonPath.Equals("[1].age", StringComparison.InvariantCultureIgnoreCase));
        }



        #region Helpers

        protected string AsJson(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(obj, settings);
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
