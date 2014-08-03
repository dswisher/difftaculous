using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Difftaculous.Paths;
using NUnit.Framework;
using Shouldly;

namespace Difftaculous.Test.Paths
{
    [TestFixture]
    public class JsonPathParserTests
    {

        [Test]
        public void CanParseRoot()
        {
            JsonPathParser parser = new JsonPathParser("$");

            // TODO
        }



        [Test]
        public void SingleProperty()
        {
            JsonPathParser parser = new JsonPathParser("Blah");

            parser.Expression.Terms.Count.ShouldBe(1);
            ((FieldTerm)parser.Expression.Terms[0]).Name.ShouldBe("Blah");
        }
    }
}
