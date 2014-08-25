#region License
//The MIT License (MIT)

//Copyright (c) 2014 Doug Swisher

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion

using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.Paths
{
    [TestFixture]
    public class JsonPathTests
    {

        [Test]
        public void TopLevelObject()
        {
            ZObject o = new ZObject();

            o.Path.AsJsonPath.ShouldBe("");
        }



        [Test]
        public void PropertyPath()
        {
            ZObject o = new ZObject();
            ZProperty p = new ZProperty("name", new ZValue("Fred"));
            o.Add(p);

            p.Path.AsJsonPath.ShouldBe("name");            
        }



        [Test]
        public void ArrayPath()
        {
            ZArray a = new ZArray();
            ZValue v = new ZValue(1);
            a.Add(v);

            v.Path.AsJsonPath.ShouldBe("[0]");
        }



        [Test]
        public void NestedPropertyPath()
        {
            // Based on JPropertyPath() from Json.Net's LinqToJsonTest

            ZObject o = new ZObject();
            ZObject c = new ZObject();
            o.Add(new ZProperty("person", c));
            c.Add(new ZProperty("$id", new ZValue(1)));

            var prop = o["person"]["$id"].Parent;

            // NOTE: Json.Net has this as person.$id, but I think we want all paths to be rooted
            prop.Path.AsJsonPath.ShouldBe("person.$id");


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
