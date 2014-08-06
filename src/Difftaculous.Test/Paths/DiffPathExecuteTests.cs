
using System.Collections.Generic;
using System.Linq;
using Difftaculous.Paths;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.Paths
{
    [TestFixture]
    public class DiffPathExecuteTests
    {

#if false
        [Test]
        public void SelectTokenAfterEmptyContainer()
        {
            string json = @"{
    'cont': [],
    'test': 'no one will find me'
}";

            JObject o = JObject.Parse(json);

            IList<JToken> results = o.SelectTokens("$..test").ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("no one will find me", (string)results[0]);
        }

        [Test]
        public void EvaluatePropertyWithRequired()
        {
            string json = "{\"bookId\":\"1000\"}";
            JObject o = JObject.Parse(json);

            string bookId = (string)o.SelectToken("bookId", true);

            Assert.AreEqual("1000", bookId);
        }

        [Test]
        public void EvaluateEmptyPropertyIndexer()
        {
            JObject o = new JObject(
                new JProperty("", 1));

            JToken t = o.SelectToken("['']");
            Assert.AreEqual(1, (int)t);
        }

        [Test]
        public void EvaluateEmptyString()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            JToken t = o.SelectToken("");
            Assert.AreEqual(o, t);

            t = o.SelectToken("['']");
            Assert.AreEqual(null, t);
        }

        [Test]
        public void EvaluateEmptyStringWithMatchingEmptyProperty()
        {
            JObject o = new JObject(
                new JProperty(" ", 1));

            JToken t = o.SelectToken("[' ']");
            Assert.AreEqual(1, (int)t);
        }

        [Test]
        public void EvaluateWhitespaceString()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            JToken t = o.SelectToken(" ");
            Assert.AreEqual(o, t);
        }
#endif


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
            t.Type.ShouldBe(TokenType.Value);       // TODO - Integer?
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


#if false
        [Test]
        public void QuoteName()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            JToken t = o.SelectToken("['Blah']");
            Assert.IsNotNull(t);
            Assert.AreEqual(JTokenType.Integer, t.Type);
            Assert.AreEqual(1, (int)t);
        }

        [Test]
        public void EvaluateMissingProperty()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            JToken t = o.SelectToken("Missing[1]");
            Assert.IsNull(t);
        }

        [Test]
        public void EvaluateIndexerOnObject()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            JToken t = o.SelectToken("[1]");
            Assert.IsNull(t);
        }

        [Test]
        public void EvaluateIndexerOnObjectWithError()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            ExceptionAssert.Throws<JsonException>(
                @"Index 1 not valid on JObject.",
                () => { o.SelectToken("[1]", true); });
        }

        [Test]
        public void EvaluateWildcardIndexOnObjectWithError()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            ExceptionAssert.Throws<JsonException>(
                @"Index * not valid on JObject.",
                () => { o.SelectToken("[*]", true); });
        }

        [Test]
        public void EvaluateSliceOnObjectWithError()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            ExceptionAssert.Throws<JsonException>(
                @"Array slice is not valid on JObject.",
                () => { o.SelectToken("[:]", true); });
        }

        [Test]
        public void EvaluatePropertyOnArray()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            JToken t = a.SelectToken("BlahBlah");
            Assert.IsNull(t);
        }

        [Test]
        public void EvaluateMultipleResultsError()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonException>(
                @"Path returned multiple tokens.",
                () => { a.SelectToken("[0, 1]"); });
        }

        [Test]
        public void EvaluatePropertyOnArrayWithError()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonException>(
                @"Property 'BlahBlah' not valid on JArray.",
                () => { a.SelectToken("BlahBlah", true); });
        }

        [Test]
        public void EvaluateNoResultsWithMultipleArrayIndexes()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonException>(
                @"Index 9 outside the bounds of JArray.",
                () => { a.SelectToken("[9,10]", true); });
        }

        [Test]
        public void EvaluateConstructorOutOfBoundsIndxerWithError()
        {
            JConstructor c = new JConstructor("Blah");

            ExceptionAssert.Throws<JsonException>(
                @"Index 1 outside the bounds of JConstructor.",
                () => { c.SelectToken("[1]", true); });
        }

        [Test]
        public void EvaluateConstructorOutOfBoundsIndxer()
        {
            JConstructor c = new JConstructor("Blah");

            Assert.IsNull(c.SelectToken("[1]"));
        }

        [Test]
        public void EvaluateMissingPropertyWithError()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            ExceptionAssert.Throws<JsonException>(
                "Property 'Missing' does not exist on JObject.",
                () => { o.SelectToken("Missing", true); });
        }

        [Test]
        public void EvaluatePropertyWithoutError()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            JValue v = (JValue)o.SelectToken("Blah", true);
            Assert.AreEqual(1, v.Value);
        }

        [Test]
        public void EvaluateMissingPropertyIndexWithError()
        {
            JObject o = new JObject(
                new JProperty("Blah", 1));

            ExceptionAssert.Throws<JsonException>(
                "Property 'Missing' does not exist on JObject.",
                () => { o.SelectToken("['Missing','Missing2']", true); });
        }

        [Test]
        public void EvaluateMultiPropertyIndexOnArrayWithError()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonException>(
                "Properties 'Missing', 'Missing2' not valid on JArray.",
                () => { a.SelectToken("['Missing','Missing2']", true); });
        }

        [Test]
        public void EvaluateArraySliceWithError()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonException>(
                "Array slice of 99 to * returned no results.",
                () => { a.SelectToken("[99:]", true); });

            ExceptionAssert.Throws<JsonException>(
                "Array slice of 1 to -19 returned no results.",
                () => { a.SelectToken("[1:-19]", true); });

            ExceptionAssert.Throws<JsonException>(
                "Array slice of * to -19 returned no results.",
                () => { a.SelectToken("[:-19]", true); });

            a = new JArray();

            ExceptionAssert.Throws<JsonException>(
                "Array slice of * to * returned no results.",
                () => { a.SelectToken("[:]", true); });
        }

        [Test]
        public void EvaluateOutOfBoundsIndxer()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            JToken t = a.SelectToken("[1000].Ha");
            Assert.IsNull(t);
        }

        [Test]
        public void EvaluateArrayOutOfBoundsIndxerWithError()
        {
            JArray a = new JArray(1, 2, 3, 4, 5);

            ExceptionAssert.Throws<JsonException>(
                "Index 1000 outside the bounds of JArray.",
                () => { a.SelectToken("[1000].Ha", true); });
        }

        [Test]
        public void EvaluateArray()
        {
            JArray a = new JArray(1, 2, 3, 4);

            JToken t = a.SelectToken("[1]");
            Assert.IsNotNull(t);
            Assert.AreEqual(JTokenType.Integer, t.Type);
            Assert.AreEqual(2, (int)t);
        }

        [Test]
        public void EvaluateArraySlice()
        {
            JArray a = new JArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
            IList<JToken> t = null;

            t = a.SelectTokens("[-3:]").ToList();
            Assert.AreEqual(3, t.Count);
            Assert.AreEqual(7, (int)t[0]);
            Assert.AreEqual(8, (int)t[1]);
            Assert.AreEqual(9, (int)t[2]);

            t = a.SelectTokens("[-1:-2:-1]").ToList();
            Assert.AreEqual(1, t.Count);
            Assert.AreEqual(9, (int)t[0]);

            t = a.SelectTokens("[-2:-1]").ToList();
            Assert.AreEqual(1, t.Count);
            Assert.AreEqual(8, (int)t[0]);

            t = a.SelectTokens("[1:1]").ToList();
            Assert.AreEqual(0, t.Count);

            t = a.SelectTokens("[1:2]").ToList();
            Assert.AreEqual(1, t.Count);
            Assert.AreEqual(2, (int)t[0]);

            t = a.SelectTokens("[::-1]").ToList();
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

            t = a.SelectTokens("[::-2]").ToList();
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
            JArray a = new JArray(1, 2, 3, 4);

            List<JToken> t = a.SelectTokens("[*]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(4, t.Count);
            Assert.AreEqual(1, (int)t[0]);
            Assert.AreEqual(2, (int)t[1]);
            Assert.AreEqual(3, (int)t[2]);
            Assert.AreEqual(4, (int)t[3]);
        }

        [Test]
        public void EvaluateArrayMultipleIndexes()
        {
            JArray a = new JArray(1, 2, 3, 4);

            IEnumerable<JToken> t = a.SelectTokens("[1,2,0]");
            Assert.IsNotNull(t);
            Assert.AreEqual(3, t.Count());
            Assert.AreEqual(2, (int)t.ElementAt(0));
            Assert.AreEqual(3, (int)t.ElementAt(1));
            Assert.AreEqual(1, (int)t.ElementAt(2));
        }

        [Test]
        public void EvaluateScan()
        {
            JObject o1 = new JObject { { "Name", 1 } };
            JObject o2 = new JObject { { "Name", 2 } };
            JArray a = new JArray(o1, o2);

            IList<JToken> t = a.SelectTokens("$..Name").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(1, (int)t[0]);
            Assert.AreEqual(2, (int)t[1]);
        }

        [Test]
        public void EvaluateWildcardScan()
        {
            JObject o1 = new JObject { { "Name", 1 } };
            JObject o2 = new JObject { { "Name", 2 } };
            JArray a = new JArray(o1, o2);

            IList<JToken> t = a.SelectTokens("$..*").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(5, t.Count);
            Assert.IsTrue(JToken.DeepEquals(a, t[0]));
            Assert.IsTrue(JToken.DeepEquals(o1, t[1]));
            Assert.AreEqual(1, (int)t[2]);
            Assert.IsTrue(JToken.DeepEquals(o2, t[3]));
            Assert.AreEqual(2, (int)t[4]);
        }

        [Test]
        public void EvaluateScanNestResults()
        {
            JObject o1 = new JObject { { "Name", 1 } };
            JObject o2 = new JObject { { "Name", 2 } };
            JObject o3 = new JObject { { "Name", new JObject { { "Name", new JArray(3) } } } };
            JArray a = new JArray(o1, o2, o3);

            IList<JToken> t = a.SelectTokens("$..Name").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(4, t.Count);
            Assert.AreEqual(1, (int)t[0]);
            Assert.AreEqual(2, (int)t[1]);
            Assert.IsTrue(JToken.DeepEquals(new JObject { { "Name", new JArray(3) } }, t[2]));
            Assert.IsTrue(JToken.DeepEquals(new JArray(3), t[3]));
        }

        [Test]
        public void EvaluateWildcardScanNestResults()
        {
            JObject o1 = new JObject { { "Name", 1 } };
            JObject o2 = new JObject { { "Name", 2 } };
            JObject o3 = new JObject { { "Name", new JObject { { "Name", new JArray(3) } } } };
            JArray a = new JArray(o1, o2, o3);

            IList<JToken> t = a.SelectTokens("$..*").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(9, t.Count);

            Assert.IsTrue(JToken.DeepEquals(a, t[0]));
            Assert.IsTrue(JToken.DeepEquals(o1, t[1]));
            Assert.AreEqual(1, (int)t[2]);
            Assert.IsTrue(JToken.DeepEquals(o2, t[3]));
            Assert.AreEqual(2, (int)t[4]);
            Assert.IsTrue(JToken.DeepEquals(o3, t[5]));
            Assert.IsTrue(JToken.DeepEquals(new JObject { { "Name", new JArray(3) } }, t[6]));
            Assert.IsTrue(JToken.DeepEquals(new JArray(3), t[7]));
            Assert.AreEqual(3, (int)t[8]);
        }

        [Test]
        public void EvaluateSinglePropertyReturningArray()
        {
            JObject o = new JObject(
                new JProperty("Blah", new[] { 1, 2, 3 }));

            JToken t = o.SelectToken("Blah");
            Assert.IsNotNull(t);
            Assert.AreEqual(JTokenType.Array, t.Type);

            t = o.SelectToken("Blah[2]");
            Assert.AreEqual(JTokenType.Integer, t.Type);
            Assert.AreEqual(3, (int)t);
        }

        [Test]
        public void EvaluateLastSingleCharacterProperty()
        {
            JObject o2 = JObject.Parse("{'People':[{'N':'Jeff'}]}");
            string a2 = (string)o2.SelectToken("People[0].N");

            Assert.AreEqual("Jeff", a2);
        }

        [Test]
        public void ExistsQuery()
        {
            JArray a = new JArray(new JObject(new JProperty("hi", "ho")), new JObject(new JProperty("hi2", "ha")));

            IList<JToken> t = a.SelectTokens("[ ?( @.hi ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(1, t.Count);
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", "ho")), t[0]));
        }

        [Test]
        public void EqualsQuery()
        {
            JArray a = new JArray(
                new JObject(new JProperty("hi", "ho")),
                new JObject(new JProperty("hi", "ha")));

            IList<JToken> t = a.SelectTokens("[ ?( @.['hi'] == 'ha' ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(1, t.Count);
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", "ha")), t[0]));
        }

        [Test]
        public void NotEqualsQuery()
        {
            JArray a = new JArray(
                new JArray(new JObject(new JProperty("hi", "ho"))),
                new JArray(new JObject(new JProperty("hi", "ha"))));

            IList<JToken> t = a.SelectTokens("[ ?( @..hi <> 'ha' ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(1, t.Count);
            Assert.IsTrue(JToken.DeepEquals(new JArray(new JObject(new JProperty("hi", "ho"))), t[0]));
        }

        [Test]
        public void NoPathQuery()
        {
            JArray a = new JArray(1, 2, 3);

            IList<JToken> t = a.SelectTokens("[ ?( @ > 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(2, (int)t[0]);
            Assert.AreEqual(3, (int)t[1]);
        }

        [Test]
        public void MultipleQueries()
        {
            JArray a = new JArray(1, 2, 3, 4, 5, 6, 7, 8, 9);

            // json path does item based evaluation - http://www.sitepen.com/blog/2008/03/17/jsonpath-support/
            // first query resolves array to ints
            // int has no children to query
            IList<JToken> t = a.SelectTokens("[?(@ <> 1)][?(@ <> 4)][?(@ < 7)]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(0, t.Count);
        }

        [Test]
        public void GreaterQuery()
        {
            JArray a = new JArray(
                new JObject(new JProperty("hi", 1)),
                new JObject(new JProperty("hi", 2)),
                new JObject(new JProperty("hi", 3)));

            IList<JToken> t = a.SelectTokens("[ ?( @.hi > 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2)), t[0]));
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 3)), t[1]));
        }

#if !(PORTABLE || PORTABLE40 || NET35 || NET20)
        [Test]
        public void GreaterQueryBigInteger()
        {
            JArray a = new JArray(
                new JObject(new JProperty("hi", new BigInteger(1))),
                new JObject(new JProperty("hi", new BigInteger(2))),
                new JObject(new JProperty("hi", new BigInteger(3))));

            IList<JToken> t = a.SelectTokens("[ ?( @.hi > 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2)), t[0]));
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 3)), t[1]));
        }
#endif

        [Test]
        public void GreaterOrEqualQuery()
        {
            JArray a = new JArray(
                new JObject(new JProperty("hi", 1)),
                new JObject(new JProperty("hi", 2)),
                new JObject(new JProperty("hi", 2.0)),
                new JObject(new JProperty("hi", 3)));

            IList<JToken> t = a.SelectTokens("[ ?( @.hi >= 1 ) ]").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(4, t.Count);
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 1)), t[0]));
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2)), t[1]));
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2.0)), t[2]));
            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 3)), t[3]));
        }

        [Test]
        public void NestedQuery()
        {
            JArray a = new JArray(
                new JObject(
                    new JProperty("name", "Bad Boys"),
                    new JProperty("cast", new JArray(
                        new JObject(new JProperty("name", "Will Smith"))))),
                new JObject(
                    new JProperty("name", "Independence Day"),
                    new JProperty("cast", new JArray(
                        new JObject(new JProperty("name", "Will Smith"))))),
                new JObject(
                    new JProperty("name", "The Rock"),
                    new JProperty("cast", new JArray(
                        new JObject(new JProperty("name", "Nick Cage")))))
                );

            IList<JToken> t = a.SelectTokens("[?(@.cast[?(@.name=='Will Smith')])].name").ToList();
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count);
            Assert.AreEqual("Bad Boys", (string)t[0]);
            Assert.AreEqual("Independence Day", (string)t[1]);
        }

        [Test]
        public void PathWithConstructor()
        {
            JArray a = JArray.Parse(@"[
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
            JObject o = JObject.Parse(@"{
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

    }
}
