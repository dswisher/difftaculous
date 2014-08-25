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
using Difftaculous.Test.Helpers;
using Difftaculous.ZModel;
using NUnit.Framework;

namespace Difftaculous.Test.ZModel
{
    [TestFixture]
    public class ZArrayTests
    {

#if false
        [Test]
        public void RemoveSpecificAndRemoveSelf()
        {
            ZObject o = new ZObject
            {
                { "results", new ZArray(1, 2, 3, 4) }
            };

            ZArray a = (ZArray)o["results"];

            var last = a.Last();

            Assert.IsTrue(a.Remove(last));

            last = a.Last();
            last.Remove();

            Assert.AreEqual(2, a.Count);
        }

        [Test]
        public void Clear()
        {
            ZArray a = new ZArray { 1 };
            Assert.AreEqual(1, a.Count);

            a.Clear();
            Assert.AreEqual(0, a.Count);
        }

        [Test]
        public void AddToSelf()
        {
            ZArray a = new ZArray();
            a.Add(a);

            Assert.IsFalse(ReferenceEquals(a[0], a));
        }
#endif


        [Test]
        public void Contains()
        {
            ZValue v = new ZValue(1);

            ZArray a = new ZArray { v };

            Assert.AreEqual(false, a.Contains(new ZValue(2)));
            Assert.AreEqual(false, a.Contains(new ZValue(1)));
            Assert.AreEqual(false, a.Contains(null));
            Assert.AreEqual(true, a.Contains(v));
        }


#if false
        [Test]
        public void GenericCollectionCopyTo()
        {
            ZArray j = new ZArray();
            j.Add(new ZValue(1));
            j.Add(new ZValue(2));
            j.Add(new ZValue(3));
            Assert.AreEqual(3, j.Count);

            ZToken[] a = new ZToken[5];

            ((ICollection<ZToken>)j).CopyTo(a, 1);

            Assert.AreEqual(null, a[0]);

            Assert.AreEqual(1, (int)a[1]);

            Assert.AreEqual(2, (int)a[2]);

            Assert.AreEqual(3, (int)a[3]);

            Assert.AreEqual(null, a[4]);
        }

        [Test]
        public void GenericCollectionCopyToNullArrayShouldThrow()
        {
            ZArray j = new ZArray();

            ExceptionAssert.Throws<ArgumentNullException>(
                @"Value cannot be null.
Parameter name: array",
                () => { ((ICollection<ZToken>)j).CopyTo(null, 0); });
        }

        [Test]
        public void GenericCollectionCopyToNegativeArrayIndexShouldThrow()
        {
            ZArray j = new ZArray();

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                @"arrayIndex is less than 0.
Parameter name: arrayIndex",
                () => { ((ICollection<ZToken>)j).CopyTo(new ZToken[1], -1); });
        }

        [Test]
        public void GenericCollectionCopyToArrayIndexEqualGreaterToArrayLengthShouldThrow()
        {
            ZArray j = new ZArray();

            ExceptionAssert.Throws<ArgumentException>(
                @"arrayIndex is equal to or greater than the length of array.",
                () => { ((ICollection<ZToken>)j).CopyTo(new ZToken[1], 1); });
        }

        [Test]
        public void GenericCollectionCopyToInsufficientArrayCapacity()
        {
            ZArray j = new ZArray();
            j.Add(new ZValue(1));
            j.Add(new ZValue(2));
            j.Add(new ZValue(3));

            ExceptionAssert.Throws<ArgumentException>(
                @"The number of elements in the source ZObject is greater than the available space from arrayIndex to the end of the destination array.",
                () => { ((ICollection<ZToken>)j).CopyTo(new ZToken[3], 1); });
        }

        [Test]
        public void Remove()
        {
            ZValue v = new ZValue(1);
            ZArray j = new ZArray();
            j.Add(v);

            Assert.AreEqual(1, j.Count);

            Assert.AreEqual(false, j.Remove(new ZValue(1)));
            Assert.AreEqual(false, j.Remove(null));
            Assert.AreEqual(true, j.Remove(v));
            Assert.AreEqual(false, j.Remove(v));

            Assert.AreEqual(0, j.Count);
        }

        [Test]
        public void IndexOf()
        {
            ZValue v1 = new ZValue(1);
            ZValue v2 = new ZValue(1);
            ZValue v3 = new ZValue(1);

            ZArray j = new ZArray();

            j.Add(v1);
            Assert.AreEqual(0, j.IndexOf(v1));

            j.Add(v2);
            Assert.AreEqual(0, j.IndexOf(v1));
            Assert.AreEqual(1, j.IndexOf(v2));

            j.AddFirst(v3);
            Assert.AreEqual(1, j.IndexOf(v1));
            Assert.AreEqual(2, j.IndexOf(v2));
            Assert.AreEqual(0, j.IndexOf(v3));

            v3.Remove();
            Assert.AreEqual(0, j.IndexOf(v1));
            Assert.AreEqual(1, j.IndexOf(v2));
            Assert.AreEqual(-1, j.IndexOf(v3));
        }

        [Test]
        public void RemoveAt()
        {
            ZValue v1 = new ZValue(1);
            ZValue v2 = new ZValue(1);
            ZValue v3 = new ZValue(1);

            ZArray j = new ZArray();

            j.Add(v1);
            j.Add(v2);
            j.Add(v3);

            Assert.AreEqual(true, j.Contains(v1));
            j.RemoveAt(0);
            Assert.AreEqual(false, j.Contains(v1));

            Assert.AreEqual(true, j.Contains(v3));
            j.RemoveAt(1);
            Assert.AreEqual(false, j.Contains(v3));

            Assert.AreEqual(1, j.Count);
        }

        [Test]
        public void RemoveAtOutOfRangeIndexShouldError()
        {
            ZArray j = new ZArray();

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                @"Index is equal to or greater than Count.
Parameter name: index",
                () => { j.RemoveAt(0); });
        }

        [Test]
        public void RemoveAtNegativeIndexShouldError()
        {
            ZArray j = new ZArray();

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                @"Index is less than 0.
Parameter name: index",
                () => { j.RemoveAt(-1); });
        }
#endif


        [Test]
        public void Insert()
        {
            ZValue v1 = new ZValue(1);
            ZValue v2 = new ZValue(2);
            ZValue v3 = new ZValue(3);
            ZValue v4 = new ZValue(4);

            ZArray j = new ZArray();

            j.Add(v1);
            j.Add(v2);
            j.Add(v3);
            j.Insert(1, v4);

            Assert.AreEqual(0, j.IndexOf(v1));
            Assert.AreEqual(1, j.IndexOf(v4));
            Assert.AreEqual(2, j.IndexOf(v2));
            Assert.AreEqual(3, j.IndexOf(v3));
        }


        [Test]
        public void AddFirstAddedTokenShouldBeFirst()
        {
            ZValue v1 = new ZValue(1);
            ZValue v2 = new ZValue(2);
            ZValue v3 = new ZValue(3);

            ZArray j = new ZArray();
            Assert.AreEqual(null, j.First);
            Assert.AreEqual(null, j.Last);

            j.AddFirst(v1);
            Assert.AreEqual(v1, j.First);
            Assert.AreEqual(v1, j.Last);

            j.AddFirst(v2);
            Assert.AreEqual(v2, j.First);
            Assert.AreEqual(v1, j.Last);

            j.AddFirst(v3);
            Assert.AreEqual(v3, j.First);
            Assert.AreEqual(v1, j.Last);
        }


        [Test]
        public void InsertShouldInsertAtZeroIndex()
        {
            ZValue v1 = new ZValue(1);
            ZValue v2 = new ZValue(2);

            ZArray j = new ZArray();

            j.Insert(0, v1);
            Assert.AreEqual(0, j.IndexOf(v1));

            j.Insert(0, v2);
            Assert.AreEqual(1, j.IndexOf(v1));
            Assert.AreEqual(0, j.IndexOf(v2));
        }


        [Test]
        public void InsertNull()
        {
            ZArray j = new ZArray();
            j.Insert(0, null);

            Assert.AreEqual(null, ((ZValue)j[0]).Value);
        }


        [Test]
        public void InsertNegativeIndexShouldThrow()
        {
            ZArray j = new ZArray();

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                @"Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index",
                () => j.Insert(-1, new ZValue(1)));
        }


        [Test]
        public void InsertOutOfRangeIndexShouldThrow()
        {
            ZArray j = new ZArray();

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                @"Index must be within the bounds of the List.
Parameter name: index",
                () => j.Insert(2, new ZValue(1)));
        }


        [Test]
        public void Item()
        {
            ZValue v1 = new ZValue(1);
            ZValue v2 = new ZValue(2);
            ZValue v3 = new ZValue(3);
            ZValue v4 = new ZValue(4);

            ZArray j = new ZArray();

            j.Add(v1);
            j.Add(v2);
            j.Add(v3);

            j[1] = v4;

            Assert.AreEqual(null, v2.Parent);
            Assert.AreEqual(-1, j.IndexOf(v2));
            Assert.AreEqual(j, v4.Parent);
            Assert.AreEqual(1, j.IndexOf(v4));
        }


#if false
        [Test]
        public void Parse_ShouldThrowOnUnexpectedToken()
        {
            string json = @"{""prop"":""value""}";

            ExceptionAssert.Throws<JsonReaderException>(
                "Error reading ZArray from JsonReader. Current JsonReader item is not an array: StartObject. Path '', line 1, position 1.",
                () => { ZArray.Parse(json); });
        }

        public class ListItemFields
        {
            public string ListItemText { get; set; }
            public object ListItemValue { get; set; }
        }

        [Test]
        public void ArrayOrder()
        {
            string itemZeroText = "Zero text";

            IEnumerable<ListItemFields> t = new List<ListItemFields>
            {
                new ListItemFields { ListItemText = "First", ListItemValue = 1 },
                new ListItemFields { ListItemText = "Second", ListItemValue = 2 },
                new ListItemFields { ListItemText = "Third", ListItemValue = 3 }
            };

            ZObject optionValues =
                new ZObject(
                    new JProperty("options",
                        new ZArray(
                            new ZObject(
                                new JProperty("text", itemZeroText),
                                new JProperty("value", "0")),
                            from r in t
                            orderby r.ListItemValue
                            select new ZObject(
                                new JProperty("text", r.ListItemText),
                                new JProperty("value", r.ListItemValue.ToString())))));

            string result = "myOptions = " + optionValues.ToString();

            Assert.AreEqual(@"myOptions = {
  ""options"": [
    {
      ""text"": ""Zero text"",
      ""value"": ""0""
    },
    {
      ""text"": ""First"",
      ""value"": ""1""
    },
    {
      ""text"": ""Second"",
      ""value"": ""2""
    },
    {
      ""text"": ""Third"",
      ""value"": ""3""
    }
  ]
}", result);
        }
#endif


        [Test]
        public void Iterate()
        {
            ZArray a = new ZArray(1, 2, 3, 4, 5);

            int i = 1;
            foreach (ZToken token in a)
            {
                Assert.AreEqual(i, (int)token);
                i++;
            }
        }


#if false
#if !(NETFX_CORE || PORTABLE || PORTABLE40)
        [Test]
        public void ITypedListGetItemProperties()
        {
            JProperty p1 = new JProperty("Test1", 1);
            JProperty p2 = new JProperty("Test2", "Two");
            ITypedList a = new ZArray(new ZObject(p1, p2));

            PropertyDescriptorCollection propertyDescriptors = a.GetItemProperties(null);
            Assert.IsNotNull(propertyDescriptors);
            Assert.AreEqual(2, propertyDescriptors.Count);
            Assert.AreEqual("Test1", propertyDescriptors[0].Name);
            Assert.AreEqual("Test2", propertyDescriptors[1].Name);
        }
#endif
#endif


        [Test, Ignore("Needs CloneToken")]
        public void AddArrayToSelf()
        {
            ZArray a = new ZArray(1, 2);
            a.Add(a);

            Assert.AreEqual(3, a.Count);
            Assert.AreEqual(1, (int)a[0]);
            Assert.AreEqual(2, (int)a[1]);
            Assert.AreNotSame(a, a[2]);
        }


        [Test]
        public void SetValueWithInvalidIndex()
        {
            ExceptionAssert.Throws<ArgumentException>(
                @"Set ZArray values with invalid key value: ""badvalue"". Array position index expected.",
                () =>
                {
                    ZArray a = new ZArray();
                    a["badvalue"] = new ZValue(3);
                });
        }


        [Test]
        public void SetValue()
        {
            object key = 0;

            ZArray a = new ZArray((object)null);
            a[key] = new ZValue(3);

            Assert.AreEqual(3, (int)a[key]);
        }


#if false
        [Test]
        public void ReplaceAll()
        {
            ZArray a = new ZArray(new[] { 1, 2, 3 });
            Assert.AreEqual(3, a.Count);
            Assert.AreEqual(1, (int)a[0]);
            Assert.AreEqual(2, (int)a[1]);
            Assert.AreEqual(3, (int)a[2]);

            a.ReplaceAll(1);
            Assert.AreEqual(1, a.Count);
            Assert.AreEqual(1, (int)a[0]);
        }

        [Test]
        public void ParseIncomplete()
        {
            ExceptionAssert.Throws<JsonReaderException>(
                "Unexpected end of content while loading ZArray. Path '[0]', line 1, position 2.",
                () => { ZArray.Parse("[1"); });
        }
#endif


        [Test]
        public void InsertAddEnd()
        {
            ZArray array = new ZArray();
            array.Insert(0, 123);
            array.Insert(1, 456);

            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(123, (int)array[0]);
            Assert.AreEqual(456, (int)array[1]);
        }


#if false
        [Test]
        public void ParseAdditionalContent()
        {
            string json = @"[
""Small"",
""Medium"",
""Large""
], 987987";

            ExceptionAssert.Throws<JsonReaderException>(
                "Additional text encountered after finished reading JSON content: ,. Path '', line 5, position 2.",
                () => { ZArray.Parse(json); });
        }

        [Test]
        public void ToListOnEmptyArray()
        {
            string json = @"{""decks"":[]}";

            ZArray decks = (ZArray)ZObject.Parse(json)["decks"];
            IList<ZToken> l = decks.ToList();
            Assert.AreEqual(0, l.Count);

            json = @"{""decks"":[1]}";

            decks = (ZArray)ZObject.Parse(json)["decks"];
            l = decks.ToList();
            Assert.AreEqual(1, l.Count);
        }
#endif

    }
}
