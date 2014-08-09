
using System.Collections.Generic;
using System.Linq;
using Difftaculous.Adapters;
using Difftaculous.Paths;
using Difftaculous.Test.Helpers;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.Paths
{
    [TestFixture]
    public class DiffPathExecuteTests
    {
        [Test]
        public void SelectTokenAfterEmptyContainer()
        {
            const string json = @"{
    'cont': [],
    'test': 'no one will find me'
}";

            ZObject o = ParseJson(json);

            IList<ZToken> results = o.SelectTokens(DiffPath.FromJsonPath("$..test")).ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("no one will find me", (string)results[0]);
        }


        [Test]
        public void EvaluatePropertyWithRequired()
        {
            const string json = "{\"bookId\":\"1000\"}";
            ZObject o = ParseJson(json);

            string bookId = (string)o.SelectToken(DiffPath.FromJsonPath("bookId"), true);

            Assert.AreEqual("1000", bookId);
        }


        [Test]
        public void EvaluateEmptyPropertyIndexer()
        {
            ZObject o = new ZObject(
                new ZProperty("", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("['']"));
            Assert.AreEqual(1, (int)t);
        }


        [Test]
        public void EvaluateEmptyString()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath(""));
            Assert.AreEqual(o, t);

            t = o.SelectToken(DiffPath.FromJsonPath("['']"));
            Assert.AreEqual(null, t);
        }


        [Test]
        public void EvaluateEmptyStringWithMatchingEmptyProperty()
        {
            ZObject o = new ZObject(
                new ZProperty(" ", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("[' ']"));
            Assert.AreEqual(1, (int)t);
        }


        [Test]
        public void EvaluateWhitespaceString()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath(" "));
            Assert.AreEqual(o, t);
        }


        [Test]
        public void EvaluateDollarString()
        {
            ZObject o = new ZObject(new ZProperty("Blah", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("$"));
            t.ShouldBeSameAs(o);
        }


        [Test]
        public void EvaluateDollarTypeString()
        {
            ZObject o = new ZObject(new ZProperty("$values", new ZArray(1, 2, 3)));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("$values[1]"));
            ((int)t).ShouldBe(2);
        }


        [Test]
        public void EvaluateSingleProperty()
        {
            ZObject o = new ZObject(new ZProperty("Blah", 1));

            var t = o.SelectToken(DiffPath.FromJsonPath("Blah"));
            t.ShouldNotBe(null);
            t.Type.ShouldBe(TokenType.Integer);
            ((int)t).ShouldBe(1);
        }


        [Test]
        public void EvaluateWildcardProperty()
        {
            ZObject o = new ZObject(new ZProperty("Blah", 1), new ZProperty("Blah2", 2));

            IList<ZToken> t = o.SelectTokens(DiffPath.FromJsonPath("$.*")).ToList();
            t.ShouldNotBe(null);
            t.Count.ShouldBe(2);
            ((int)t[0]).ShouldBe(1);
            ((int)t[1]).ShouldBe(2);
        }


        [Test]
        public void QuoteName()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("['Blah']"));
            Assert.IsNotNull(t);
            Assert.AreEqual(TokenType.Integer, t.Type);
            Assert.AreEqual(1, (int)t);
        }


        [Test]
        public void EvaluateMissingProperty()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("Missing[1]"));
            Assert.IsNull(t);
        }


        [Test]
        public void EvaluateIndexerOnObject()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("[1]"));
            Assert.IsNull(t);
        }


        [Test]
        public void EvaluateIndexerOnObjectWithError()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ExceptionAssert.Throws<JsonPathException>(
                @"Index 1 not valid on ZObject.",
                () => o.SelectToken(DiffPath.FromJsonPath("[1]"), true));
        }


        [Test]
        public void EvaluateWildcardIndexOnObjectWithError()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ExceptionAssert.Throws<JsonPathException>(
                @"Index * not valid on ZObject.",
                () => o.SelectToken(DiffPath.FromJsonPath("[*]"), true));
        }


        [Test]
        public void EvaluateSliceOnObjectWithError()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ExceptionAssert.Throws<JsonPathException>(
                @"Array slice is not valid on ZObject.",
                () => o.SelectToken(DiffPath.FromJsonPath("[:]"), true));
        }


        [Test]
        public void EvaluatePropertyOnArray()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ZToken t = a.SelectToken(DiffPath.FromJsonPath("BlahBlah"));
            Assert.IsNull(t);
        }


        [Test]
        public void EvaluateMultipleResultsError()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonPathException>(
                @"Path returned multiple tokens.",
                () => a.SelectToken(DiffPath.FromJsonPath("[0, 1]")));
        }


        [Test]
        public void EvaluatePropertyOnArrayWithError()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonPathException>(
                @"Property 'BlahBlah' not valid on ZArray.",
                () => a.SelectToken(DiffPath.FromJsonPath("BlahBlah"), true));
        }


        [Test]
        public void EvaluateNoResultsWithMultipleArrayIndexes()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonPathException>(
                @"Index 9 outside the bounds of ZArray.",
                () => a.SelectToken(DiffPath.FromJsonPath("[9,10]"), true));
        }


        [Test]
        public void EvaluateMissingPropertyWithError()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ExceptionAssert.Throws<JsonPathException>(
                "Property 'Missing' does not exist on ZObject.",
                () => o.SelectToken(DiffPath.FromJsonPath("Missing"), true));
        }


        [Test]
        public void EvaluatePropertyWithoutError()
        {
            ZObject o = new ZObject(new ZProperty("Blah", 1));

            ZValue v = (ZValue)o.SelectToken(DiffPath.FromJsonPath("Blah"), true);
            Assert.AreEqual(1, v.Value);
        }


        [Test]
        public void EvaluateMissingPropertyIndexWithError()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", 1));

            ExceptionAssert.Throws<JsonPathException>(
                "Property 'Missing' does not exist on ZObject.",
                () => o.SelectToken(DiffPath.FromJsonPath("['Missing','Missing2']"), true));
        }


        [Test]
        public void EvaluateMultiPropertyIndexOnArrayWithError()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonPathException>(
                "Properties 'Missing', 'Missing2' not valid on ZArray.",
                () => a.SelectToken(DiffPath.FromJsonPath("['Missing','Missing2']"), true));
        }


        [Test]
        public void EvaluateArraySliceWithError()
        {
// ReSharper disable AccessToModifiedClosure

            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonPathException>(
                "Array slice of 99 to * returned no results.",
                () => a.SelectToken(DiffPath.FromJsonPath("[99:]"), true));

            ExceptionAssert.Throws<JsonPathException>(
                "Array slice of 1 to -19 returned no results.",
                () => a.SelectToken(DiffPath.FromJsonPath("[1:-19]"), true));

            ExceptionAssert.Throws<JsonPathException>(
                "Array slice of * to -19 returned no results.",
                () => a.SelectToken(DiffPath.FromJsonPath("[:-19]"), true));

            a = new ZArray();

            ExceptionAssert.Throws<JsonPathException>(
                "Array slice of * to * returned no results.",
                () => a.SelectToken(DiffPath.FromJsonPath("[:]"), true));

// ReSharper restore AccessToModifiedClosure
        }


        [Test]
        public void EvaluateOutOfBoundsIndxer()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ZToken t = a.SelectToken(DiffPath.FromJsonPath("[1000].Ha"));
            Assert.IsNull(t);
        }


        [Test]
        public void EvaluateArrayOutOfBoundsIndxerWithError()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonPathException>(
                "Index 1000 outside the bounds of ZArray.",
                () => { a.SelectToken(DiffPath.FromJsonPath("[1000].Ha"), true); });
        }


        [Test]
        public void EvaluateArray()
        {
            ZArray a = new ZArray(1, 2, 3, 4);

            ZToken t = a.SelectToken(DiffPath.FromJsonPath("[1]"));
            Assert.IsNotNull(t);
            Assert.AreEqual(TokenType.Integer, t.Type);
            Assert.AreEqual(2, (int)t);
        }


        [Test]
        public void EvaluateArraySlice()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
            IList<ZToken> t;

            t = a.SelectTokens(DiffPath.FromJsonPath("[-3:]")).ToList();
            Assert.AreEqual(3, t.Count);
            Assert.AreEqual(7, (int)t[0]);
            Assert.AreEqual(8, (int)t[1]);
            Assert.AreEqual(9, (int)t[2]);

            t = a.SelectTokens(DiffPath.FromJsonPath("[-1:-2:-1]")).ToList();
            Assert.AreEqual(1, t.Count);
            Assert.AreEqual(9, (int)t[0]);

            t = a.SelectTokens(DiffPath.FromJsonPath("[-2:-1]")).ToList();
            Assert.AreEqual(1, t.Count);
            Assert.AreEqual(8, (int)t[0]);

            t = a.SelectTokens(DiffPath.FromJsonPath("[1:1]")).ToList();
            Assert.AreEqual(0, t.Count);

            t = a.SelectTokens(DiffPath.FromJsonPath("[1:2]")).ToList();
            Assert.AreEqual(1, t.Count);
            Assert.AreEqual(2, (int)t[0]);

            t = a.SelectTokens(DiffPath.FromJsonPath("[::-1]")).ToList();
            Assert.AreEqual(9, t.Count);
            Assert.AreEqual(9, (int)t[0]);
            Assert.AreEqual(8, (int)t[1]);
            Assert.AreEqual(7, (int)t[2]);
            Assert.AreEqual(6, (int)t[3]);
            Assert.AreEqual(5, (int)t[4]);
            Assert.AreEqual(4, (int)t[5]);
            Assert.AreEqual(3, (int)t[6]);
            Assert.AreEqual(2, (int)t[7]);
            Assert.AreEqual(1, (int)t[8]);

            t = a.SelectTokens(DiffPath.FromJsonPath("[::-2]")).ToList();
            Assert.AreEqual(5, t.Count);
            Assert.AreEqual(9, (int)t[0]);
            Assert.AreEqual(7, (int)t[1]);
            Assert.AreEqual(5, (int)t[2]);
            Assert.AreEqual(3, (int)t[3]);
            Assert.AreEqual(1, (int)t[4]);
        }


        [Test]
        public void EvaluateWildcardArray()
        {
            ZArray a = new ZArray(1, 2, 3, 4);

            List<ZToken> t = a.SelectTokens(DiffPath.FromJsonPath("[*]")).ToList();
            t.ShouldNotBe(null);
            t.Count.ShouldBe(4);
            ((int)t[0]).ShouldBe(1);
            ((int)t[1]).ShouldBe(2);
            ((int)t[2]).ShouldBe(3);
            ((int)t[3]).ShouldBe(4);
        }


        [Test]
        public void EvaluateArrayMultipleIndexes()
        {
            ZArray a = new ZArray(1, 2, 3, 4);

            var t = a.SelectTokens(DiffPath.FromJsonPath("[1,2,0]")).ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(3, t.Count());
            Assert.AreEqual(2, (int)t.ElementAt(0));
            Assert.AreEqual(3, (int)t.ElementAt(1));
            Assert.AreEqual(1, (int)t.ElementAt(2));
        }


        [Test]
        public void EvaluateScan()
        {
            ZObject o1 = new ZObject { { "Name", 1 } };
            ZObject o2 = new ZObject { { "Name", 2 } };
            ZArray a = new ZArray(o1, o2);

            IList<ZToken> t = a.SelectTokens(DiffPath.FromJsonPath("$..Name")).ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(1, (int)t[0]);
            Assert.AreEqual(2, (int)t[1]);
        }


        [Test]
        public void EvaluateWildcardScan()
        {
            ZObject o1 = new ZObject { { "Name", 1 } };
            ZObject o2 = new ZObject { { "Name", 2 } };
            ZArray a = new ZArray(o1, o2);

            IList<ZToken> t = a.SelectTokens(DiffPath.FromJsonPath("$..*")).ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(5, t.Count);
            Assert.IsTrue(ZToken.DeepEquals(a, t[0]));
            Assert.IsTrue(ZToken.DeepEquals(o1, t[1]));
            Assert.AreEqual(1, (int)t[2]);
            Assert.IsTrue(ZToken.DeepEquals(o2, t[3]));
            Assert.AreEqual(2, (int)t[4]);
        }


        [Test]
        public void EvaluateScanNestResults()
        {
            ZObject o1 = new ZObject(new ZProperty("Name", 1));
            ZObject o2 = new ZObject(new ZProperty("Name", 2));
            ZObject o3 = new ZObject(new ZProperty("Name", new ZObject(new ZProperty("Name", new ZArray(3)))));
            ZArray a = new ZArray(o1, o2, o3);

            IList<ZToken> t = a.SelectTokens(DiffPath.FromJsonPath("$..Name")).ToList();
            t.ShouldNotBe(null);
            t.Count.ShouldBe(4);
            ((int)t[0]).ShouldBe(1);
            ((int)t[1]).ShouldBe(2);
            ZToken.DeepEquals(new ZObject { { "Name", new ZArray(3) } }, t[2]).ShouldBe(true);
            ZToken.DeepEquals(new ZArray(3), t[3]).ShouldBe(true);
        }


        [Test]
        public void EvaluateWildcardScanNestResults()
        {
            ZObject o1 = new ZObject { { "Name", 1 } };
            ZObject o2 = new ZObject { { "Name", 2 } };
            ZObject o3 = new ZObject { { "Name", new ZObject { { "Name", new ZArray(3) } } } };
            ZArray a = new ZArray(o1, o2, o3);

            IList<ZToken> t = a.SelectTokens(DiffPath.FromJsonPath("$..*")).ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(9, t.Count);

            Assert.IsTrue(ZToken.DeepEquals(a, t[0]));
            Assert.IsTrue(ZToken.DeepEquals(o1, t[1]));
            Assert.AreEqual(1, (int)t[2]);
            Assert.IsTrue(ZToken.DeepEquals(o2, t[3]));
            Assert.AreEqual(2, (int)t[4]);
            Assert.IsTrue(ZToken.DeepEquals(o3, t[5]));
            Assert.IsTrue(ZToken.DeepEquals(new ZObject { { "Name", new ZArray(3) } }, t[6]));
            Assert.IsTrue(ZToken.DeepEquals(new ZArray(3), t[7]));
            Assert.AreEqual(3, (int)t[8]);
        }


        [Test]
        public void EvaluateSinglePropertyReturningArray()
        {
            ZObject o = new ZObject(
                new ZProperty("Blah", new[] { 1, 2, 3 }));

            ZToken t = o.SelectToken(DiffPath.FromJsonPath("Blah"));
            Assert.IsNotNull(t);
            Assert.AreEqual(TokenType.Array, t.Type);

            t = o.SelectToken(DiffPath.FromJsonPath("Blah[2]"));
            Assert.AreEqual(TokenType.Integer, t.Type);
            Assert.AreEqual(3, (int)t);
        }


        [Test]
        public void EvaluateLastSingleCharacterProperty()
        {
            ZObject o2 = ParseJson("{'People':[{'N':'Jeff'}]}");
            string a2 = (string)o2.SelectToken(DiffPath.FromJsonPath("People[0].N"));

            Assert.AreEqual("Jeff", a2);
        }


#if false
        [Test]
        public void ExistsQuery()
        {
            ZArray a = new ZArray(new ZObject(new ZProperty("hi", "ho")), new ZObject(new ZProperty("hi2", "ha")));

            IList<ZToken> t = a.SelectTokens("[ ?( @.hi ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(1, t.Count);
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", "ho")), t[0]));
        }

        [Test]
        public void EqualsQuery()
        {
            ZArray a = new ZArray(
                new ZObject(new ZProperty("hi", "ho")),
                new ZObject(new ZProperty("hi", "ha")));

            IList<ZToken> t = a.SelectTokens("[ ?( @.['hi'] == 'ha' ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(1, t.Count);
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", "ha")), t[0]));
        }

        [Test]
        public void NotEqualsQuery()
        {
            ZArray a = new ZArray(
                new ZArray(new ZObject(new ZProperty("hi", "ho"))),
                new ZArray(new ZObject(new ZProperty("hi", "ha"))));

            IList<ZToken> t = a.SelectTokens("[ ?( @..hi <> 'ha' ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(1, t.Count);
            Assert.IsTrue(ZToken.DeepEquals(new ZArray(new ZObject(new ZProperty("hi", "ho"))), t[0]));
        }

        [Test]
        public void NoPathQuery()
        {
            ZArray a = new ZArray(1, 2, 3);

            IList<ZToken> t = a.SelectTokens("[ ?( @ > 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(2, (int)t[0]);
            Assert.AreEqual(3, (int)t[1]);
        }

        [Test]
        public void MultipleQueries()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5, 6, 7, 8, 9);

            // json path does item based evaluation - http://www.sitepen.com/blog/2008/03/17/jsonpath-support/
            // first query resolves array to ints
            // int has no children to query
            IList<ZToken> t = a.SelectTokens("[?(@ <> 1)][?(@ <> 4)][?(@ < 7)]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(0, t.Count);
        }

        [Test]
        public void GreaterQuery()
        {
            ZArray a = new ZArray(
                new ZObject(new ZProperty("hi", 1)),
                new ZObject(new ZProperty("hi", 2)),
                new ZObject(new ZProperty("hi", 3)));

            IList<ZToken> t = a.SelectTokens("[ ?( @.hi > 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 2)), t[0]));
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 3)), t[1]));
        }

#if !(PORTABLE || PORTABLE40 || NET35 || NET20)
        [Test]
        public void GreaterQueryBigInteger()
        {
            ZArray a = new ZArray(
                new ZObject(new ZProperty("hi", new BigInteger(1))),
                new ZObject(new ZProperty("hi", new BigInteger(2))),
                new ZObject(new ZProperty("hi", new BigInteger(3))));

            IList<ZToken> t = a.SelectTokens("[ ?( @.hi > 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 2)), t[0]));
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 3)), t[1]));
        }
#endif

        [Test]
        public void GreaterOrEqualQuery()
        {
            ZArray a = new ZArray(
                new ZObject(new ZProperty("hi", 1)),
                new ZObject(new ZProperty("hi", 2)),
                new ZObject(new ZProperty("hi", 2.0)),
                new ZObject(new ZProperty("hi", 3)));

            IList<ZToken> t = a.SelectTokens("[ ?( @.hi >= 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(4, t.Count);
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 1)), t[0]));
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 2)), t[1]));
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 2.0)), t[2]));
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(new ZProperty("hi", 3)), t[3]));
        }

        [Test]
        public void NestedQuery()
        {
            ZArray a = new ZArray(
                new ZObject(
                    new ZProperty("name", "Bad Boys"),
                    new ZProperty("cast", new ZArray(
                        new ZObject(new ZProperty("name", "Will Smith"))))),
                new ZObject(
                    new ZProperty("name", "Independence Day"),
                    new ZProperty("cast", new ZArray(
                        new ZObject(new ZProperty("name", "Will Smith"))))),
                new ZObject(
                    new ZProperty("name", "The Rock"),
                    new ZProperty("cast", new ZArray(
                        new ZObject(new ZProperty("name", "Nick Cage")))))
                );

            IList<ZToken> t = a.SelectTokens("[?(@.cast[?(@.name=='Will Smith')])].name").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.AreEqual("Bad Boys", (string)t[0]);
            Assert.AreEqual("Independence Day", (string)t[1]);
        }

        [Test]
        public void PathWithConstructor()
        {
            ZArray a = ZArray.Parse(@"[
  {
    ""Property1"": [
      1,
      [
        [
          []
        ]
      ]
    ]
  },
  {
    ""Property2"": new Constructor1(
      null,
      [
        1
      ]
    )
  }
]");

            JValue v = (JValue)a.SelectToken("[1].Property2[1][0]");
            Assert.AreEqual(1L, v.Value);
        }


        [Test]
        public void Example()
        {
            ZObject o = ZObject.Parse(@"{
        ""Stores"": [
          ""Lambton Quay"",
          ""Willis Street""
        ],
        ""Manufacturers"": [
          {
            ""Name"": ""Acme Co"",
            ""Products"": [
              {
                ""Name"": ""Anvil"",
                ""Price"": 50
              }
            ]
          },
          {
            ""Name"": ""Contoso"",
            ""Products"": [
              {
                ""Name"": ""Elbow Grease"",
                ""Price"": 99.95
              },
              {
                ""Name"": ""Headlight Fluid"",
                ""Price"": 4
              }
            ]
          }
        ]
      }");

            string name = (string)o.SelectToken("Manufacturers[0].Name");
            // Acme Co

            decimal productPrice = (decimal)o.SelectToken("Manufacturers[0].Products[0].Price");
            // 50

            string productName = (string)o.SelectToken("Manufacturers[1].Products[0].Name");
            // Elbow Grease

            Assert.AreEqual("Acme Co", name);
            Assert.AreEqual(50m, productPrice);
            Assert.AreEqual("Elbow Grease", productName);

            IList<string> storeNames = o.SelectToken("Stores").Select(s => (string)s).ToList();
            // Lambton Quay
            // Willis Street

            IList<string> firstProductNames = o["Manufacturers"].Select(m => (string)m.SelectToken("Products[1].Name")).ToList();
            // null
            // Headlight Fluid

            decimal totalPrice = o["Manufacturers"].Sum(m => (decimal)m.SelectToken("Products[0].Price"));
            // 149.95

            Assert.AreEqual(2, storeNames.Count);
            Assert.AreEqual("Lambton Quay", storeNames[0]);
            Assert.AreEqual("Willis Street", storeNames[1]);
            Assert.AreEqual(2, firstProductNames.Count);
            Assert.AreEqual(null, firstProductNames[0]);
            Assert.AreEqual("Headlight Fluid", firstProductNames[1]);
            Assert.AreEqual(149.95m, totalPrice);
        }
#endif


        private static ZObject ParseJson(string json)
        {
            return (ZObject)new JsonAdapter(json).Content.Content;
        }
    }
}
