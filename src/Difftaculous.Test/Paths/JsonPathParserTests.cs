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

using Difftaculous.Paths;
using Difftaculous.Paths.Expressions;
using Difftaculous.Test.Helpers;
using NUnit.Framework;
using Shouldly;


namespace Difftaculous.Test.Paths
{
    [TestFixture]
    public class JsonPathParserTests
    {
        [Test]
        public void SingleProperty()
        {
            var filters = JsonPathParser.Parse("Blah");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah");
        }



        [Test]
        public void SingleQuotedProperty()
        {
            var filters = JsonPathParser.Parse("['Blah']");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah");
        }



        [Test]
        public void SingleQuotedPropertyWithWhitespace()
        {
            var filters = JsonPathParser.Parse("[  'Blah'  ]");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah");
        }



        [Test]
        public void SingleQuotedPropertyWithDots()
        {
            var filters = JsonPathParser.Parse("['Blah.Ha']");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah.Ha");
        }



        [Test]
        public void SingleQuotedPropertyWithBrackets()
        {
            var filters = JsonPathParser.Parse("['[*]']");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("[*]");
        }



        [Test]
        public void SinglePropertyWithRoot()
        {
            var filters = JsonPathParser.Parse("$.Blah");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah");
        }



        [Test]
        public void SinglePropertyWithRootWithStartAndEndWhitespace()
        {
            var filters = JsonPathParser.Parse(" $.Blah ");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah");
        }



        [Test]
        public void RootWithBadWhitespace()
        {
            ExceptionAssert.Throws<PathException>(
                @"Unexpected character while parsing path:  ",
                () => JsonPathParser.Parse("$ .Blah"));
        }



        [Test]
        public void NoFieldNameAfterDot()
        {
            ExceptionAssert.Throws<PathException>(
                @"Unexpected end while parsing path.",
                () => JsonPathParser.Parse("$.Blah."));
        }



        [Test]
        public void RootWithBadWhitespace2()
        {
            ExceptionAssert.Throws<PathException>(
                @"Unexpected character while parsing path:  ",
                () => JsonPathParser.Parse("$. Blah"));
        }



        [Test]
        public void WildcardPropertyWithRoot()
        {
            var filters = JsonPathParser.Parse("$.*");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe(null);
        }



        [Test]
        public void WildcardArrayWithRoot()
        {
            var filters = JsonPathParser.Parse("$.[*]");

            filters.Count.ShouldBe(1);
            ((ArrayIndexFilter)filters[0]).Index.ShouldBe(null);
        }



        [Test]
        public void RootArrayNoDot()
        {
            var filters = JsonPathParser.Parse("$[1]");

            filters.Count.ShouldBe(1);
            ((ArrayIndexFilter)filters[0]).Index.ShouldBe(1);
        }



        [Test]
        public void WildcardArray()
        {
            var filters = JsonPathParser.Parse("[*]");

            filters.Count.ShouldBe(1);
            ((ArrayIndexFilter)filters[0]).Index.ShouldBe(null);
        }



        [Test]
        public void WildcardArrayWithProperty()
        {
            var filters = JsonPathParser.Parse("[ * ].derp");

            filters.Count.ShouldBe(2);
            ((ArrayIndexFilter)filters[0]).Index.ShouldBe(null);
            ((FieldFilter)filters[1]).Name.ShouldBe("derp");
        }



        [Test]
        public void QuotedWildcardPropertyWithRoot()
        {
            var filters = JsonPathParser.Parse("$.['*']");

            filters.Count.ShouldBe(1);
            ((FieldFilter)filters[0]).Name.ShouldBe("*");
        }


        [Test]
        public void SingleScanWithRoot()
        {
            var filters = JsonPathParser.Parse("$..Blah");

            filters.Count.ShouldBe(1);
            ((ScanFilter)filters[0]).Name.ShouldBe("Blah");
        }


        [Test]
        public void WildcardScanWithRoot()
        {
            var filters = JsonPathParser.Parse("$..*");

            filters.Count.ShouldBe(1);
            ((ScanFilter)filters[0]).Name.ShouldBe(null);
        }



#if false
        [Test]
        public void WildcardScanWithRootWithWhitespace()
        {
            JPath path = new JPath("$..* ");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(null, ((ScanFilter)path.Filters[0]).Name);
        }

        [Test]
        public void TwoProperties()
        {
            JPath path = new JPath("Blah.Two");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            Assert.AreEqual("Two", ((FieldFilter)path.Filters[1]).Name);
        }

        [Test]
        public void OnePropertyOneScan()
        {
            JPath path = new JPath("Blah..Two");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            Assert.AreEqual("Two", ((ScanFilter)path.Filters[1]).Name);
        }

        [Test]
        public void SinglePropertyAndIndexer()
        {
            JPath path = new JPath("Blah[0]");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            Assert.AreEqual(0, ((ArrayIndexFilter)path.Filters[1]).Index);
        }
#endif


        [Test]
        public void SinglePropertyAndExistsQuery()
        {
            var filters = JsonPathParser.Parse("Blah[ ?( @..name ) ]");

            filters.Count.ShouldBe(2);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah");
            BooleanQueryExpression expression = (BooleanQueryExpression)((QueryFilter)filters[1]).Expression;
            expression.Operator.ShouldBe(QueryOperator.Exists);
            expression.Path.Count.ShouldBe(1);
            ((ScanFilter)expression.Path[0]).Name.ShouldBe("name");
        }


        [Test]
        public void SinglePropertyAndFilterWithWhitespace()
        {
            var filters = JsonPathParser.Parse("Blah[ ?( @.name=='hi' ) ]");

            filters.Count.ShouldBe(2);
            ((FieldFilter)filters[0]).Name.ShouldBe("Blah");
            BooleanQueryExpression expression = (BooleanQueryExpression)((QueryFilter)filters[1]).Expression;
            expression.Operator.ShouldBe(QueryOperator.Equals);

            // TODO - check value!
            //((string)expression.Value).ShouldBe("hi");
            //Assert.AreEqual("hi", (string)expressions.Value);
        }


#if false
        [Test]
        public void SinglePropertyAndFilterWithEscapeQuote()
        {
            JPath path = new JPath(@"Blah[ ?( @.name=='h\'i' ) ]");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.AreEqual(QueryOperator.Equals, expressions.Operator);
            Assert.AreEqual("h'i", (string)expressions.Value);
        }

        [Test]
        public void SinglePropertyAndFilterWithDoubleEscape()
        {
            JPath path = new JPath(@"Blah[ ?( @.name=='h\\i' ) ]");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.AreEqual(QueryOperator.Equals, expressions.Operator);
            Assert.AreEqual("h\\i", (string)expressions.Value);
        }

        [Test]
        public void SinglePropertyAndFilterWithUnknownEscape()
        {
            ExceptionAssert.Throws<JsonException>(
                @"Unknown escape chracter: \i",
                () => { new JPath(@"Blah[ ?( @.name=='h\i' ) ]"); });
        }

        [Test]
        public void SinglePropertyAndFilterWithFalse()
        {
            JPath path = new JPath("Blah[ ?( @.name==false ) ]");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.AreEqual(QueryOperator.Equals, expressions.Operator);
            Assert.AreEqual(false, (bool)expressions.Value);
        }

        [Test]
        public void SinglePropertyAndFilterWithTrue()
        {
            JPath path = new JPath("Blah[ ?( @.name==true ) ]");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.AreEqual(QueryOperator.Equals, expressions.Operator);
            Assert.AreEqual(true, (bool)expressions.Value);
        }

        [Test]
        public void SinglePropertyAndFilterWithNull()
        {
            JPath path = new JPath("Blah[ ?( @.name==null ) ]");
            Assert.AreEqual(2, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.AreEqual(QueryOperator.Equals, expressions.Operator);
            Assert.AreEqual(null, expressions.Value.Value);
        }

        [Test]
        public void FilterWithScan()
        {
            JPath path = new JPath("[?(@..name<>null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual("name", ((ScanFilter)expressions.Path[0]).Name);
        }

        [Test]
        public void FilterWithNotEquals()
        {
            JPath path = new JPath("[?(@.name<>null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(QueryOperator.NotEquals, expressions.Operator);
        }

        [Test]
        public void FilterWithNotEquals2()
        {
            JPath path = new JPath("[?(@.name!=null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(QueryOperator.NotEquals, expressions.Operator);
        }

        [Test]
        public void FilterWithLessThan()
        {
            JPath path = new JPath("[?(@.name<null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(QueryOperator.LessThan, expressions.Operator);
        }

        [Test]
        public void FilterWithLessThanOrEquals()
        {
            JPath path = new JPath("[?(@.name<=null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(QueryOperator.LessThanOrEquals, expressions.Operator);
        }

        [Test]
        public void FilterWithGreaterThan()
        {
            JPath path = new JPath("[?(@.name>null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(QueryOperator.GreaterThan, expressions.Operator);
        }

        [Test]
        public void FilterWithGreaterThanOrEquals()
        {
            JPath path = new JPath("[?(@.name>=null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(QueryOperator.GreaterThanOrEquals, expressions.Operator);
        }

        [Test]
        public void FilterWithInteger()
        {
            JPath path = new JPath("[?(@.name>=12)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(12, (int)expressions.Value);
        }

        [Test]
        public void FilterWithNegativeInteger()
        {
            JPath path = new JPath("[?(@.name>=-12)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(-12, (int)expressions.Value);
        }

        [Test]
        public void FilterWithFloat()
        {
            JPath path = new JPath("[?(@.name>=12.1)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(12.1d, (double)expressions.Value);
        }
#endif


        [Test]
        public void FilterExistWithAnd()
        {
            var filters = JsonPathParser.Parse("[?(@.name&&@.title)]");

            CompositeExpression expression = (CompositeExpression)((QueryFilter)filters[0]).Expression;
            expression.Operator.ShouldBe(QueryOperator.And);
            expression.Expressions.Count.ShouldBe(2);

            ((FieldFilter)((BooleanQueryExpression)expression.Expressions[0]).Path[0]).Name.ShouldBe("name");
            expression.Expressions[0].Operator.ShouldBe(QueryOperator.Exists);
            ((FieldFilter)((BooleanQueryExpression)expression.Expressions[1]).Path[0]).Name.ShouldBe("title");
            expression.Expressions[1].Operator.ShouldBe(QueryOperator.Exists);
        }


#if false
        [Test]
        public void FilterExistWithAndOr()
        {
            JPath path = new JPath("[?(@.name&&@.title||@.pie)]");
            CompositeExpression andExpression = (CompositeExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(QueryOperator.And, andExpression.Operator);
            Assert.AreEqual(2, andExpression.Expressions.Count);
            Assert.AreEqual("name", ((FieldFilter)((BooleanQueryExpression)andExpression.Expressions[0]).Path[0]).Name);
            Assert.AreEqual(QueryOperator.Exists, andExpression.Expressions[0].Operator);

            CompositeExpression orExpression = (CompositeExpression)andExpression.Expressions[1];
            Assert.AreEqual(2, orExpression.Expressions.Count);
            Assert.AreEqual("title", ((FieldFilter)((BooleanQueryExpression)orExpression.Expressions[0]).Path[0]).Name);
            Assert.AreEqual(QueryOperator.Exists, orExpression.Expressions[0].Operator);
            Assert.AreEqual("pie", ((FieldFilter)((BooleanQueryExpression)orExpression.Expressions[1]).Path[0]).Name);
            Assert.AreEqual(QueryOperator.Exists, orExpression.Expressions[1].Operator);
        }

        [Test]
        public void BadOr1()
        {
            ExceptionAssert.Throws<JsonException>("Unexpected character while parsing path query: )",
                () => new JPath("[?(@.name||)]"));
        }

        [Test]
        public void BaddOr2()
        {
            ExceptionAssert.Throws<JsonException>("Unexpected character while parsing path query: |",
                () => new JPath("[?(@.name|)]"));
        }

        [Test]
        public void BaddOr3()
        {
            ExceptionAssert.Throws<JsonException>("Unexpected character while parsing path query: |",
                () => new JPath("[?(@.name|"));
        }

        [Test]
        public void BaddOr4()
        {
            ExceptionAssert.Throws<JsonException>("Path ended with open query.",
                () => new JPath("[?(@.name||"));
        }

        [Test]
        public void NoAtAfterOr()
        {
            ExceptionAssert.Throws<JsonException>("Unexpected character while parsing path query: s",
                () => new JPath("[?(@.name||s"));
        }

        [Test]
        public void NoPathAfterAt()
        {
            ExceptionAssert.Throws<JsonException>(@"Path ended with open query.",
                () => new JPath("[?(@.name||@"));
        }

        [Test]
        public void NoPathAfterDot()
        {
            ExceptionAssert.Throws<JsonException>(@"Unexpected end while parsing path.",
                () => new JPath("[?(@.name||@."));
        }

        [Test]
        public void NoPathAfterDot2()
        {
            ExceptionAssert.Throws<JsonException>(@"Unexpected end while parsing path.",
                () => new JPath("[?(@.name||@.)]"));
        }

        [Test]
        public void FilterWithFloatExp()
        {
            JPath path = new JPath("[?(@.name>=5.56789e+0)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.AreEqual(5.56789e+0, (double)expressions.Value);
        }

        [Test]
        public void MultiplePropertiesAndIndexers()
        {
            JPath path = new JPath("Blah[0]..Two.Three[1].Four");
            Assert.AreEqual(6, path.Filters.Count);
            Assert.AreEqual("Blah", ((FieldFilter) path.Filters[0]).Name);
            Assert.AreEqual(0, ((ArrayIndexFilter) path.Filters[1]).Index);
            Assert.AreEqual("Two", ((ScanFilter)path.Filters[2]).Name);
            Assert.AreEqual("Three", ((FieldFilter)path.Filters[3]).Name);
            Assert.AreEqual(1, ((ArrayIndexFilter)path.Filters[4]).Index);
            Assert.AreEqual("Four", ((FieldFilter)path.Filters[5]).Name);
        }

        [Test]
        public void BadCharactersInIndexer()
        {
            ExceptionAssert.Throws<JsonException>(
                @"Unexpected character while parsing path indexer: [",
                () => { new JPath("Blah[[0]].Two.Three[1].Four"); });
        }

        [Test]
        public void UnclosedIndexer()
        {
            ExceptionAssert.Throws<JsonException>(
                @"Path ended with open indexer.",
                () => { new JPath("Blah[0"); });
        }

        [Test]
        public void IndexerOnly()
        {
            JPath path = new JPath("[111119990]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(111119990, ((ArrayIndexFilter)path.Filters[0]).Index);
        }

        [Test]
        public void IndexerOnlyWithWhitespace()
        {
            JPath path = new JPath("[  10  ]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(10, ((ArrayIndexFilter)path.Filters[0]).Index);
        }
#endif


        [Test]
        public void MultipleIndexes()
        {
            var filters = JsonPathParser.Parse("[111119990,3]");

            filters.Count.ShouldBe(1);
            ((ArrayMultipleIndexFilter) filters[0]).Indexes.Count.ShouldBe(2);
            ((ArrayMultipleIndexFilter)filters[0]).Indexes[0].ShouldBe(111119990);
            ((ArrayMultipleIndexFilter)filters[0]).Indexes[1].ShouldBe(3);
        }


        [Test]
        public void MultipleIndexesWithWhitespace()
        {
            var filters = JsonPathParser.Parse("[   111119990  ,   3   ]");

            filters.Count.ShouldBe(1);
            ((ArrayMultipleIndexFilter)filters[0]).Indexes.Count.ShouldBe(2);
            ((ArrayMultipleIndexFilter)filters[0]).Indexes[0].ShouldBe(111119990);
            ((ArrayMultipleIndexFilter)filters[0]).Indexes[1].ShouldBe(3);
        }


        [Test]
        public void MultipleQuotedIndexes()
        {
            var filters = JsonPathParser.Parse("['111119990','3']");

            filters.Count.ShouldBe(1);
            ((FieldMultipleFilter)filters[0]).Names.Count.ShouldBe(2);
            ((FieldMultipleFilter)filters[0]).Names[0].ShouldBe("111119990");
            ((FieldMultipleFilter)filters[0]).Names[1].ShouldBe("3");
        }


#if false




        [Test]
        public void MultipleQuotedIndexesWithWhitespace()
        {
            JPath path = new JPath("[ '111119990' , '3' ]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(2, ((FieldMultipleFilter)path.Filters[0]).Names.Count);
            Assert.AreEqual("111119990", ((FieldMultipleFilter)path.Filters[0]).Names[0]);
            Assert.AreEqual("3", ((FieldMultipleFilter)path.Filters[0]).Names[1]);
        }
#endif


        [Test]
        public void SlicingIndexAll()
        {
            var filters = JsonPathParser.Parse("[111119990:3:2]");

            filters.Count.ShouldBe(1);
            ((ArraySliceFilter)filters[0]).Start.ShouldBe(111119990);
            ((ArraySliceFilter)filters[0]).End.ShouldBe(3);
            ((ArraySliceFilter)filters[0]).Step.ShouldBe(2);
        }


#if false
        [Test]
        public void SlicingIndex()
        {
            JPath path = new JPath("[111119990:3]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(111119990, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.AreEqual(3, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.AreEqual(null, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Test]
        public void SlicingIndexNegative()
        {
            JPath path = new JPath("[-111119990:-3:-2]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(-111119990, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.AreEqual(-3, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.AreEqual(-2, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Test]
        public void SlicingIndexEmptyStop()
        {
            JPath path = new JPath("[  -3  :  ]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(-3, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.AreEqual(null, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.AreEqual(null, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Test]
        public void SlicingIndexEmptyStart()
        {
            JPath path = new JPath("[ : 1 : ]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(null, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.AreEqual(1, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.AreEqual(null, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Test]
        public void SlicingIndexWhitespace()
        {
            JPath path = new JPath("[  -111119990  :  -3  :  -2  ]");
            Assert.AreEqual(1, path.Filters.Count);
            Assert.AreEqual(-111119990, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.AreEqual(-3, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.AreEqual(-2, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Test]
        public void EmptyIndexer()
        {
            ExceptionAssert.Throws<JsonException>(
                "Array index expected.",
                () => { new JPath("[]"); });
        }

        [Test]
        public void IndexerCloseInProperty()
        {
            ExceptionAssert.Throws<JsonException>(
                "Unexpected character while parsing path: ]",
                () => { new JPath("]"); });
        }

        [Test]
        public void AdjacentIndexers()
        {
            JPath path = new JPath("[1][0][0][" + int.MaxValue + "]");
            Assert.AreEqual(4, path.Filters.Count);
            Assert.AreEqual(1, ((ArrayIndexFilter)path.Filters[0]).Index);
            Assert.AreEqual(0, ((ArrayIndexFilter)path.Filters[1]).Index);
            Assert.AreEqual(0, ((ArrayIndexFilter)path.Filters[2]).Index);
            Assert.AreEqual(int.MaxValue, ((ArrayIndexFilter)path.Filters[3]).Index);
        }

        [Test]
        public void MissingDotAfterIndexer()
        {
            ExceptionAssert.Throws<JsonException>(
                "Unexpected character following indexer: B",
                () => { new JPath("[1]Blah"); });
        }
#endif
    }
}
