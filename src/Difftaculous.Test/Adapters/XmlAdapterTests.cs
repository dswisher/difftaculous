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

using System;
using Difftaculous.Adapters;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.Adapters
{
    [TestFixture]
    public class XmlAdapterTests
    {

        [Test]
        public void EmptyElement()
        {
            const string xml = "<thing></thing>";
            var expected = new ZObject();

            RunTest(expected, xml);
        }



        [Test]
        public void OneProperty()
        {
            const string xml = "<thing><name>Fred</name></thing>";
            var expected = new ZObject(new ZProperty("name", "Fred"));

            RunTest(expected, xml);
        }



        [Test]
        public void OnePropertyAsAttribute()
        {
            const string xml = "<thing name=\"Fred\"></thing>";
            var expected = new ZObject(new ZProperty("name", "Fred"));

            RunTest(expected, xml);
        }



        [Test]
        public void TwoProperties()
        {
            const string xml = "<thing><name>Fred</name><age>44</age></thing>";
            var expected = new ZObject(new ZProperty("name", "Fred"), new ZProperty("age", 44));

            RunTest(expected, xml);
        }



        [Test]
        public void SimpleArray()
        {
            const string xml = "<team><player>Walt</player><player>Joe</player><player>Fred</player></team>";
            var expected = new ZArray("Walt", "Joe", "Fred");

            RunTest(expected, xml);
        }



        [Test]
        public void ArrayWithComplexTypes()
        {
            const string xml = "<team><player><name>Walt</name></player><player><name>Fred</name></player></team>";
            var walt = new ZObject(new ZProperty("name", "Walt"));
            var fred = new ZObject(new ZProperty("name", "Fred"));
            var expected = new ZArray(walt, fred);

            RunTest(expected, xml);
        }



        private void RunTest(ZToken expected, string xml)
        {
            ZToken actual = new XmlAdapter(xml).Content;

            Console.WriteLine("Expected:");
            Console.WriteLine(expected.AsJson());

            Console.WriteLine();

            Console.WriteLine("Actual:");
            Console.WriteLine(actual.AsJson());

            actual.ShouldBe(expected);
        }
    }
}
