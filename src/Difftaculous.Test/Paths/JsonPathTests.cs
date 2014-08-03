
using Difftaculous.Paths;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.Paths
{
    [TestFixture]
    public class JsonPathTests
    {
        // TODO - is matching symmetric?  Does a.Matches(b) => b.Matches(a)?
        //      NO!  Matching two paths should be removed; Matches should take an IToken!

        [Ignore("Get this working!")]
        [TestCase("$.countries[1].points")]
        [TestCase("$.countries[55].points")]
        // [TestCase("$.countries[sort='x'].points")]
        public void WildcardArrayMatch(string s)
        {
            var template = DiffPath.FromJsonPath("$.countries[*].points");
            var path = DiffPath.FromJsonPath(s);

            // TODO - how to write this with shouldly, in such a way that we see both paths on failure?
            // path.Matches(template).ShouldBe(true);

            Assert.That(template.Matches(path), string.Format("{0} matches {1}", template.AsJsonPath, path.AsJsonPath));
        }



        [Test]
        public void TopLevelObject()
        {
            ZObject o = new ZObject();

            o.Path.AsJsonPath.ShouldBe("$");
        }



        [Test]
        public void PropertyPath()
        {
            ZObject o = new ZObject();
            ZProperty p = new ZProperty("name", new ZValue("Fred"));
            o.AddProperty(p);

            p.Path.AsJsonPath.ShouldBe("$.name");            
        }



        [Test]
        public void ArrayPath()
        {
            ZArray a = new ZArray();
            ZValue v = new ZValue(1);
            a.Add(v);

            v.Path.AsJsonPath.ShouldBe("$[0]");
        }



        [Test]
        public void NestedPropertyPath()
        {
            // Based on JPropertyPath() from Json.Net's LinqToJsonTest

            ZObject o = new ZObject();
            ZObject c = new ZObject();
            o.AddProperty(new ZProperty("person", c));
            c.AddProperty(new ZProperty("$id", new ZValue(1)));

            var prop = o["person"]["$id"].Parent;

            // NOTE: Json.Net has this as person.$id, but I think we want all paths to be rooted
            prop.Path.AsJsonPath.ShouldBe("$.person.$id");


            // TODO - make ZObject an IDictionary (of properties) so we can use a collection initializer
#if false
            JObject o = new JObject
            {
                {
                    "person",
                    new JObject
                    {
                        { "$id", 1 }
                    }
                }
            };

            JContainer idProperty = o["person"]["$id"].Parent;
            Assert.AreEqual("person.$id", idProperty.Path);
#endif
        }
    }
}
