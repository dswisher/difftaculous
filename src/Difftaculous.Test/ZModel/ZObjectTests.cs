
using System;
using System.Collections;
using System.Collections.Generic;
using Difftaculous.Test.Helpers;
using Difftaculous.ZModel;
using NUnit.Framework;
using Shouldly;

// ReSharper disable InconsistentNaming


namespace Difftaculous.Test.ZModel
{
    [TestFixture]
    public class ZObjectTests
    {

        [Test]
        public void CanFetchProperty()
        {
            ZObject o = new ZObject { new ZProperty("id", new ZValue(1)) };

            o.Property("id").ShouldNotBe(null);
            o["id"].ShouldNotBe(null);
        }


        // -----------------------------------------------------


#if false
#if !(NET35 || NET20 || PORTABLE40)
        [Test]
        public void EmbedZValueStringInNewZObject()
        {
            string s = null;
            var v = new ZValue(s);
            dynamic o = ZObject.FromObject(new { title = v });

            string output = o.ToString();

            Assert.AreEqual(@"{
  ""title"": null
}", output);

            Assert.AreEqual(null, v.Value);
            Assert.IsNull((string)o.title);
        }
#endif

        [Test]
        public void ZObjectWithComments()
        {
            string json = @"{ /*comment2*/
        ""Name"": /*comment3*/ ""Apple"" /*comment4*/, /*comment5*/
        ""ExpiryDate"": ""\/Date(1230422400000)\/"",
        ""Price"": 3.99,
        ""Sizes"": /*comment6*/ [ /*comment7*/
          ""Small"", /*comment8*/
          ""Medium"" /*comment9*/,
          /*comment10*/ ""Large""
        /*comment11*/ ] /*comment12*/
      } /*comment13*/";

            ZToken o = ZToken.Parse(json);

            Assert.AreEqual("Apple", (string) o["Name"]);
        }

        [Test]
        public void WritePropertyWithNoValue()
        {
            var o = new ZObject();
            o.Add(new ZProperty("novalue"));

            Assert.AreEqual(@"{
  ""novalue"": null
}", o.ToString());
        }

        [Test]
        public void Keys()
        {
            var o = new ZObject();
            var d = (IDictionary<string, ZToken>)o;

            Assert.AreEqual(0, d.Keys.Count);

            o["value"] = true;

            Assert.AreEqual(1, d.Keys.Count);
        }

        [Test]
        public void TryGetValue()
        {
            ZObject o = new ZObject();
            o.Add("PropertyNameValue", new ZValue(1));
            Assert.AreEqual(1, o.Children().Count());

            ZToken t;
            Assert.AreEqual(false, o.TryGetValue("sdf", out t));
            Assert.AreEqual(null, t);

            Assert.AreEqual(false, o.TryGetValue(null, out t));
            Assert.AreEqual(null, t);

            Assert.AreEqual(true, o.TryGetValue("PropertyNameValue", out t));
            Assert.AreEqual(true, ZToken.DeepEquals(new ZValue(1), t));
        }

        [Test]
        public void DictionaryItemShouldSet()
        {
            ZObject o = new ZObject();
            o["PropertyNameValue"] = new ZValue(1);
            Assert.AreEqual(1, o.Children().Count());

            ZToken t;
            Assert.AreEqual(true, o.TryGetValue("PropertyNameValue", out t));
            Assert.AreEqual(true, ZToken.DeepEquals(new ZValue(1), t));

            o["PropertyNameValue"] = new ZValue(2);
            Assert.AreEqual(1, o.Children().Count());

            Assert.AreEqual(true, o.TryGetValue("PropertyNameValue", out t));
            Assert.AreEqual(true, ZToken.DeepEquals(new ZValue(2), t));

            o["PropertyNameValue"] = null;
            Assert.AreEqual(1, o.Children().Count());

            Assert.AreEqual(true, o.TryGetValue("PropertyNameValue", out t));
            Assert.AreEqual(true, ZToken.DeepEquals(ZValue.CreateNull(), t));
        }

        [Test]
        public void Remove()
        {
            ZObject o = new ZObject();
            o.Add("PropertyNameValue", new ZValue(1));
            Assert.AreEqual(1, o.Children().Count());

            Assert.AreEqual(false, o.Remove("sdf"));
            Assert.AreEqual(false, o.Remove(null));
            Assert.AreEqual(true, o.Remove("PropertyNameValue"));

            Assert.AreEqual(0, o.Children().Count());
        }

        [Test]
        public void GenericCollectionRemove()
        {
            ZValue v = new ZValue(1);
            ZObject o = new ZObject();
            o.Add("PropertyNameValue", v);
            Assert.AreEqual(1, o.Children().Count());

            Assert.AreEqual(false, ((ICollection<KeyValuePair<string, ZToken>>)o).Remove(new KeyValuePair<string, ZToken>("PropertyNameValue1", new ZValue(1))));
            Assert.AreEqual(false, ((ICollection<KeyValuePair<string, ZToken>>)o).Remove(new KeyValuePair<string, ZToken>("PropertyNameValue", new ZValue(2))));
            Assert.AreEqual(false, ((ICollection<KeyValuePair<string, ZToken>>)o).Remove(new KeyValuePair<string, ZToken>("PropertyNameValue", new ZValue(1))));
            Assert.AreEqual(true, ((ICollection<KeyValuePair<string, ZToken>>)o).Remove(new KeyValuePair<string, ZToken>("PropertyNameValue", v)));

            Assert.AreEqual(0, o.Children().Count());
        }

        [Test]
        public void DuplicatePropertyNameShouldThrow()
        {
            ExceptionAssert.Throws<ArgumentException>(
                "Can not add property PropertyNameValue to Newtonsoft.Json.Linq.ZObject. Property with the same name already exists on object.",
                () =>
                {
                    ZObject o = new ZObject();
                    o.Add("PropertyNameValue", null);
                    o.Add("PropertyNameValue", null);
                });
        }

        [Test]
        public void GenericDictionaryAdd()
        {
            ZObject o = new ZObject();

            o.Add("PropertyNameValue", new ZValue(1));
            Assert.AreEqual(1, (int)o["PropertyNameValue"]);

            o.Add("PropertyNameValue1", null);
            Assert.AreEqual(null, ((ZValue)o["PropertyNameValue1"]).Value);

            Assert.AreEqual(2, o.Children().Count());
        }

        [Test]
        public void GenericCollectionAdd()
        {
            ZObject o = new ZObject();
            ((ICollection<KeyValuePair<string, ZToken>>)o).Add(new KeyValuePair<string, ZToken>("PropertyNameValue", new ZValue(1)));

            Assert.AreEqual(1, (int)o["PropertyNameValue"]);
            Assert.AreEqual(1, o.Children().Count());
        }

        [Test]
        public void GenericCollectionClear()
        {
            ZObject o = new ZObject();
            o.Add("PropertyNameValue", new ZValue(1));
            Assert.AreEqual(1, o.Children().Count());

            ZProperty p = (ZProperty)o.Children().ElementAt(0);

            ((ICollection<KeyValuePair<string, ZToken>>)o).Clear();
            Assert.AreEqual(0, o.Children().Count());

            Assert.AreEqual(null, p.Parent);
        }

        [Test]
        public void GenericCollectionContains()
        {
            ZValue v = new ZValue(1);
            ZObject o = new ZObject();
            o.Add("PropertyNameValue", v);
            Assert.AreEqual(1, o.Children().Count());

            bool contains = ((ICollection<KeyValuePair<string, ZToken>>)o).Contains(new KeyValuePair<string, ZToken>("PropertyNameValue", new ZValue(1)));
            Assert.AreEqual(false, contains);

            contains = ((ICollection<KeyValuePair<string, ZToken>>)o).Contains(new KeyValuePair<string, ZToken>("PropertyNameValue", v));
            Assert.AreEqual(true, contains);

            contains = ((ICollection<KeyValuePair<string, ZToken>>)o).Contains(new KeyValuePair<string, ZToken>("PropertyNameValue", new ZValue(2)));
            Assert.AreEqual(false, contains);

            contains = ((ICollection<KeyValuePair<string, ZToken>>)o).Contains(new KeyValuePair<string, ZToken>("PropertyNameValue1", new ZValue(1)));
            Assert.AreEqual(false, contains);

            contains = ((ICollection<KeyValuePair<string, ZToken>>)o).Contains(default(KeyValuePair<string, ZToken>));
            Assert.AreEqual(false, contains);
        }

        [Test]
        public void GenericDictionaryContains()
        {
            ZObject o = new ZObject();
            o.Add("PropertyNameValue", new ZValue(1));
            Assert.AreEqual(1, o.Children().Count());

            bool contains = ((IDictionary<string, ZToken>)o).ContainsKey("PropertyNameValue");
            Assert.AreEqual(true, contains);
        }

        [Test]
        public void GenericCollectionCopyTo()
        {
            ZObject o = new ZObject();
            o.Add("PropertyNameValue", new ZValue(1));
            o.Add("PropertyNameValue2", new ZValue(2));
            o.Add("PropertyNameValue3", new ZValue(3));
            Assert.AreEqual(3, o.Children().Count());

            KeyValuePair<string, ZToken>[] a = new KeyValuePair<string, ZToken>[5];

            ((ICollection<KeyValuePair<string, ZToken>>)o).CopyTo(a, 1);

            Assert.AreEqual(default(KeyValuePair<string, ZToken>), a[0]);

            Assert.AreEqual("PropertyNameValue", a[1].Key);
            Assert.AreEqual(1, (int)a[1].Value);

            Assert.AreEqual("PropertyNameValue2", a[2].Key);
            Assert.AreEqual(2, (int)a[2].Value);

            Assert.AreEqual("PropertyNameValue3", a[3].Key);
            Assert.AreEqual(3, (int)a[3].Value);

            Assert.AreEqual(default(KeyValuePair<string, ZToken>), a[4]);
        }

        [Test]
        public void GenericCollectionCopyToNullArrayShouldThrow()
        {
            ExceptionAssert.Throws<ArgumentException>(
                @"Value cannot be null.
Parameter name: array",
                () =>
                {
                    ZObject o = new ZObject();
                    ((ICollection<KeyValuePair<string, ZToken>>)o).CopyTo(null, 0);
                });
        }

        [Test]
        public void GenericCollectionCopyToNegativeArrayIndexShouldThrow()
        {
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(
                @"arrayIndex is less than 0.
Parameter name: arrayIndex",
                () =>
                {
                    ZObject o = new ZObject();
                    ((ICollection<KeyValuePair<string, ZToken>>)o).CopyTo(new KeyValuePair<string, ZToken>[1], -1);
                });
        }

        [Test]
        public void GenericCollectionCopyToArrayIndexEqualGreaterToArrayLengthShouldThrow()
        {
            ExceptionAssert.Throws<ArgumentException>(
                @"arrayIndex is equal to or greater than the length of array.",
                () =>
                {
                    ZObject o = new ZObject();
                    ((ICollection<KeyValuePair<string, ZToken>>)o).CopyTo(new KeyValuePair<string, ZToken>[1], 1);
                });
        }

        [Test]
        public void GenericCollectionCopyToInsufficientArrayCapacity()
        {
            ExceptionAssert.Throws<ArgumentException>(
                @"The number of elements in the source ZObject is greater than the available space from arrayIndex to the end of the destination array.",
                () =>
                {
                    ZObject o = new ZObject();
                    o.Add("PropertyNameValue", new ZValue(1));
                    o.Add("PropertyNameValue2", new ZValue(2));
                    o.Add("PropertyNameValue3", new ZValue(3));

                    ((ICollection<KeyValuePair<string, ZToken>>)o).CopyTo(new KeyValuePair<string, ZToken>[3], 1);
                });
        }

        [Test]
        public void FromObjectRaw()
        {
            PersonRaw raw = new PersonRaw
            {
                FirstName = "FirstNameValue",
                RawContent = new JRaw("[1,2,3,4,5]"),
                LastName = "LastNameValue"
            };

            ZObject o = ZObject.FromObject(raw);

            Assert.AreEqual("FirstNameValue", (string)o["first_name"]);
            Assert.AreEqual(ZTokenType.Raw, ((ZValue)o["RawContent"]).Type);
            Assert.AreEqual("[1,2,3,4,5]", (string)o["RawContent"]);
            Assert.AreEqual("LastNameValue", (string)o["last_name"]);
        }

        [Test]
        public void ZTokenReader()
        {
            PersonRaw raw = new PersonRaw
            {
                FirstName = "FirstNameValue",
                RawContent = new JRaw("[1,2,3,4,5]"),
                LastName = "LastNameValue"
            };

            ZObject o = ZObject.FromObject(raw);

            JsonReader reader = new ZTokenReader(o);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Raw, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsFalse(reader.Read());
        }

        [Test]
        public void DeserializeFromRaw()
        {
            PersonRaw raw = new PersonRaw
            {
                FirstName = "FirstNameValue",
                RawContent = new JRaw("[1,2,3,4,5]"),
                LastName = "LastNameValue"
            };

            ZObject o = ZObject.FromObject(raw);

            JsonReader reader = new ZTokenReader(o);
            JsonSerializer serializer = new JsonSerializer();
            raw = (PersonRaw)serializer.Deserialize(reader, typeof(PersonRaw));

            Assert.AreEqual("FirstNameValue", raw.FirstName);
            Assert.AreEqual("LastNameValue", raw.LastName);
            Assert.AreEqual("[1,2,3,4,5]", raw.RawContent.Value);
        }

        [Test]
        public void Parse_ShouldThrowOnUnexpectedToken()
        {
            ExceptionAssert.Throws<JsonReaderException>("Error reading ZObject from JsonReader. Current JsonReader item is not an object: StartArray. Path '', line 1, position 1.",
                () =>
                {
                    string json = @"[""prop""]";
                    ZObject.Parse(json);
                });
        }

        [Test]
        public void ParseJavaScriptDate()
        {
            string json = @"[new Date(1207285200000)]";

            JArray a = (JArray)JsonConvert.DeserializeObject(json);
            ZValue v = (ZValue)a[0];

            Assert.AreEqual(DateTimeUtils.ConvertJavaScriptTicksToDateTime(1207285200000), (DateTime)v);
        }

        [Test]
        public void GenericValueCast()
        {
            string json = @"{""foo"":true}";
            ZObject o = (ZObject)JsonConvert.DeserializeObject(json);
            bool? value = o.Value<bool?>("foo");
            Assert.AreEqual(true, value);

            json = @"{""foo"":null}";
            o = (ZObject)JsonConvert.DeserializeObject(json);
            value = o.Value<bool?>("foo");
            Assert.AreEqual(null, value);
        }

        [Test]
        public void Blog()
        {
            ExceptionAssert.Throws<JsonReaderException>(
                "Invalid property identifier character: ]. Path 'name', line 3, position 5.",
                () => { ZObject.Parse(@"{
    ""name"": ""James"",
    ]!#$THIS IS: BAD JSON![{}}}}]
  }"); });
        }

        [Test]
        public void RawChildValues()
        {
            ZObject o = new ZObject();
            o["val1"] = new JRaw("1");
            o["val2"] = new JRaw("1");

            string json = o.ToString();

            Assert.AreEqual(@"{
  ""val1"": 1,
  ""val2"": 1
}", json);
        }
#endif


        [Test]
        public void Iterate()
        {
            ZObject o = new ZObject
            {
                { "PropertyNameValue1", new ZValue(1) },
                { "PropertyNameValue2", new ZValue(2) }
            };

            ZToken t = o;

            int i = 1;
            foreach (var zToken in t)
            {
                var property = (ZProperty) zToken;

                Assert.AreEqual("PropertyNameValue" + i, property.Name);
                Assert.AreEqual(i, (int)property.Value);

                i++;
            }
        }


        [Test]
        public void KeyValuePairIterate()
        {
            ZObject o = new ZObject();
            o.Add("PropertyNameValue1", new ZValue(1));
            o.Add("PropertyNameValue2", new ZValue(2));

            int i = 1;
            foreach (KeyValuePair<string, ZToken> pair in o)
            {
                Assert.AreEqual("PropertyNameValue" + i, pair.Key);
                Assert.AreEqual(i, (int)pair.Value);

                i++;
            }
        }


#if false
        [Test]
        public void WriteObjectNullStringValue()
        {
            string s = null;
            ZValue v = new ZValue(s);
            Assert.AreEqual(null, v.Value);
            Assert.AreEqual(ZTokenType.String, v.Type);

            ZObject o = new ZObject();
            o["title"] = v;

            string output = o.ToString();

            Assert.AreEqual(@"{
  ""title"": null
}", output);
        }

        [Test]
        public void Example()
        {
            string json = @"{
        ""Name"": ""Apple"",
        ""Expiry"": new Date(1230422400000),
        ""Price"": 3.99,
        ""Sizes"": [
          ""Small"",
          ""Medium"",
          ""Large""
        ]
      }";

            ZObject o = ZObject.Parse(json);

            string name = (string)o["Name"];
            // Apple

            JArray sizes = (JArray)o["Sizes"];

            string smallest = (string)sizes[0];
            // Small

            Console.WriteLine(name);
            Console.WriteLine(smallest);
        }

        [Test]
        public void DeserializeClassManually()
        {
            string jsonText = @"{
  ""short"":
  {
    ""original"":""http://www.foo.com/"",
    ""short"":""krehqk"",
    ""error"":
    {
      ""code"":0,
      ""msg"":""No action taken""
    }
  }
}";

            ZObject json = ZObject.Parse(jsonText);

            Shortie shortie = new Shortie
            {
                Original = (string)json["short"]["original"],
                Short = (string)json["short"]["short"],
                Error = new ShortieException
                {
                    Code = (int)json["short"]["error"]["code"],
                    ErrorMessage = (string)json["short"]["error"]["msg"]
                }
            };

            Console.WriteLine(shortie.Original);
            // http://www.foo.com/

            Console.WriteLine(shortie.Error.ErrorMessage);
            // No action taken

            Assert.AreEqual("http://www.foo.com/", shortie.Original);
            Assert.AreEqual("krehqk", shortie.Short);
            Assert.AreEqual(null, shortie.Shortened);
            Assert.AreEqual(0, shortie.Error.Code);
            Assert.AreEqual("No action taken", shortie.Error.ErrorMessage);
        }

        [Test]
        public void ZObjectContainingHtml()
        {
            ZObject o = new ZObject();
            o["rc"] = new ZValue(200);
            o["m"] = new ZValue("");
            o["o"] = new ZValue(@"<div class='s1'>
    <div class='avatar'>                    
        <a href='asdf'>asdf</a><br />
        <strong>0</strong>
    </div>
    <div class='sl'>
        <p>
            444444444
        </p>
    </div>
    <div class='clear'>
    </div>                        
</div>");

            Assert.AreEqual(@"{
  ""rc"": 200,
  ""m"": """",
  ""o"": ""<div class='s1'>\r\n    <div class='avatar'>                    \r\n        <a href='asdf'>asdf</a><br />\r\n        <strong>0</strong>\r\n    </div>\r\n    <div class='sl'>\r\n        <p>\r\n            444444444\r\n        </p>\r\n    </div>\r\n    <div class='clear'>\r\n    </div>                        \r\n</div>""
}", o.ToString());
        }

        [Test]
        public void ImplicitValueConversions()
        {
            ZObject moss = new ZObject();
            moss["FirstName"] = new ZValue("Maurice");
            moss["LastName"] = new ZValue("Moss");
            moss["BirthDate"] = new ZValue(new DateTime(1977, 12, 30));
            moss["Department"] = new ZValue("IT");
            moss["JobTitle"] = new ZValue("Support");

            Console.WriteLine(moss.ToString());
            //{
            //  "FirstName": "Maurice",
            //  "LastName": "Moss",
            //  "BirthDate": "\/Date(252241200000+1300)\/",
            //  "Department": "IT",
            //  "JobTitle": "Support"
            //}


            ZObject jen = new ZObject();
            jen["FirstName"] = "Jen";
            jen["LastName"] = "Barber";
            jen["BirthDate"] = new DateTime(1978, 3, 15);
            jen["Department"] = "IT";
            jen["JobTitle"] = "Manager";

            Console.WriteLine(jen.ToString());
            //{
            //  "FirstName": "Jen",
            //  "LastName": "Barber",
            //  "BirthDate": "\/Date(258721200000+1300)\/",
            //  "Department": "IT",
            //  "JobTitle": "Manager"
            //}
        }

        [Test]
        public void ReplaceZPropertyWithZPropertyWithSameName()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");

            ZObject o = new ZObject(p1, p2);
            IList l = o;
            Assert.AreEqual(p1, l[0]);
            Assert.AreEqual(p2, l[1]);

            ZProperty p3 = new ZProperty("Test1", "III");

            p1.Replace(p3);
            Assert.AreEqual(null, p1.Parent);
            Assert.AreEqual(l, p3.Parent);

            Assert.AreEqual(p3, l[0]);
            Assert.AreEqual(p2, l[1]);

            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, o.Properties().Count());

            ZProperty p4 = new ZProperty("Test4", "IV");

            p2.Replace(p4);
            Assert.AreEqual(null, p2.Parent);
            Assert.AreEqual(l, p4.Parent);

            Assert.AreEqual(p3, l[0]);
            Assert.AreEqual(p4, l[1]);
        }

#if !(NET20 || NETFX_CORE || PORTABLE || PORTABLE40)
        [Test]
        public void PropertyChanging()
        {
            object changing = null;
            object changed = null;
            int changingCount = 0;
            int changedCount = 0;

            ZObject o = new ZObject();
            o.PropertyChanging += (sender, args) =>
            {
                ZObject s = (ZObject)sender;
                changing = (s[args.PropertyName] != null) ? ((ZValue)s[args.PropertyName]).Value : null;
                changingCount++;
            };
            o.PropertyChanged += (sender, args) =>
            {
                ZObject s = (ZObject)sender;
                changed = (s[args.PropertyName] != null) ? ((ZValue)s[args.PropertyName]).Value : null;
                changedCount++;
            };

            o["StringValue"] = "value1";
            Assert.AreEqual(null, changing);
            Assert.AreEqual("value1", changed);
            Assert.AreEqual("value1", (string)o["StringValue"]);
            Assert.AreEqual(1, changingCount);
            Assert.AreEqual(1, changedCount);

            o["StringValue"] = "value1";
            Assert.AreEqual(1, changingCount);
            Assert.AreEqual(1, changedCount);

            o["StringValue"] = "value2";
            Assert.AreEqual("value1", changing);
            Assert.AreEqual("value2", changed);
            Assert.AreEqual("value2", (string)o["StringValue"]);
            Assert.AreEqual(2, changingCount);
            Assert.AreEqual(2, changedCount);

            o["StringValue"] = null;
            Assert.AreEqual("value2", changing);
            Assert.AreEqual(null, changed);
            Assert.AreEqual(null, (string)o["StringValue"]);
            Assert.AreEqual(3, changingCount);
            Assert.AreEqual(3, changedCount);

            o["NullValue"] = null;
            Assert.AreEqual(null, changing);
            Assert.AreEqual(null, changed);
            Assert.AreEqual(ZValue.CreateNull(), o["NullValue"]);
            Assert.AreEqual(4, changingCount);
            Assert.AreEqual(4, changedCount);

            o["NullValue"] = null;
            Assert.AreEqual(4, changingCount);
            Assert.AreEqual(4, changedCount);
        }
#endif

        [Test]
        public void PropertyChanged()
        {
            object changed = null;
            int changedCount = 0;

            ZObject o = new ZObject();
            o.PropertyChanged += (sender, args) =>
            {
                ZObject s = (ZObject)sender;
                changed = (s[args.PropertyName] != null) ? ((ZValue)s[args.PropertyName]).Value : null;
                changedCount++;
            };

            o["StringValue"] = "value1";
            Assert.AreEqual("value1", changed);
            Assert.AreEqual("value1", (string)o["StringValue"]);
            Assert.AreEqual(1, changedCount);

            o["StringValue"] = "value1";
            Assert.AreEqual(1, changedCount);

            o["StringValue"] = "value2";
            Assert.AreEqual("value2", changed);
            Assert.AreEqual("value2", (string)o["StringValue"]);
            Assert.AreEqual(2, changedCount);

            o["StringValue"] = null;
            Assert.AreEqual(null, changed);
            Assert.AreEqual(null, (string)o["StringValue"]);
            Assert.AreEqual(3, changedCount);

            o["NullValue"] = null;
            Assert.AreEqual(null, changed);
            Assert.AreEqual(ZValue.CreateNull(), o["NullValue"]);
            Assert.AreEqual(4, changedCount);

            o["NullValue"] = null;
            Assert.AreEqual(4, changedCount);
        }
#endif


        [Test]
        public void IListContains()
        {
            ZProperty p = new ZProperty("Test", 1);
            IList l = new ZObject(p);

            Assert.IsTrue(l.Contains(p));
            Assert.IsFalse(l.Contains(new ZProperty("Test", 1)));
        }


        [Test]
        public void IListIndexOf()
        {
            ZProperty p = new ZProperty("Test", 1);
            IList l = new ZObject(p);

            Assert.AreEqual(0, l.IndexOf(p));
            Assert.AreEqual(-1, l.IndexOf(new ZProperty("Test", 1)));
        }


        [Test]
        public void IListClear()
        {
            ZProperty p = new ZProperty("Test", 1);
            IList l = new ZObject(p);

            Assert.AreEqual(1, l.Count);

            l.Clear();

            Assert.AreEqual(0, l.Count);
        }


        [Test]
        public void IListCopyTo()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            object[] a = new object[l.Count];

            l.CopyTo(a, 0);

            Assert.AreEqual(p1, a[0]);
            Assert.AreEqual(p2, a[1]);
        }

        [Test]
        public void IListAdd()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            l.Add(p3);

            Assert.AreEqual(3, l.Count);
            Assert.AreEqual(p3, l[2]);
        }


        [Test]
        public void IListAddBadToken()
        {
            ExceptionAssert.Throws<ArgumentException>(
                "Can not add ZValue to ZObject.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList l = new ZObject(p1, p2);

                    l.Add(new ZValue("Bad!"));
                });
        }


        [Test]
        public void IListAddBadValue()
        {
            ExceptionAssert.Throws<ArgumentException>(
                "Argument is not a ZToken.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList l = new ZObject(p1, p2);

                    l.Add("Bad!");
                });
        }


        [Test]
        public void IListAddPropertyWithExistingName()
        {
            ExceptionAssert.Throws<ArgumentException>(
                "Can not add property Test2 to ZObject. Property with the same name already exists on object.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList l = new ZObject(p1, p2);

                    ZProperty p3 = new ZProperty("Test2", "II");

                    l.Add(p3);
                });
        }


#if false
        [Test]
        public void IListRemove()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            // won't do anything
            l.Remove(p3);
            Assert.AreEqual(2, l.Count);

            l.Remove(p1);
            Assert.AreEqual(1, l.Count);
            Assert.IsFalse(l.Contains(p1));
            Assert.IsTrue(l.Contains(p2));

            l.Remove(p2);
            Assert.AreEqual(0, l.Count);
            Assert.IsFalse(l.Contains(p2));
            Assert.AreEqual(null, p2.Parent);
        }

        [Test]
        public void IListRemoveAt()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            // won't do anything
            l.RemoveAt(0);

            l.Remove(p1);
            Assert.AreEqual(1, l.Count);

            l.Remove(p2);
            Assert.AreEqual(0, l.Count);
        }
#endif


        [Test]
        public void IListInsert()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            l.Insert(1, p3);
            Assert.AreEqual(l, p3.Parent);

            Assert.AreEqual(p1, l[0]);
            Assert.AreEqual(p3, l[1]);
            Assert.AreEqual(p2, l[2]);
        }


        [Test]
        public void IListIsReadOnly()
        {
            IList l = new ZObject();
            Assert.IsFalse(l.IsReadOnly);
        }

        [Test]
        public void IListIsFixedSize()
        {
            IList l = new ZObject();
            Assert.IsFalse(l.IsFixedSize);
        }


#if false
        [Test]
        public void IListSetItem()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            l[0] = p3;

            Assert.AreEqual(p3, l[0]);
            Assert.AreEqual(p2, l[1]);
        }

        [Test]
        public void IListSetItemAlreadyExists()
        {
            ExceptionAssert.Throws<ArgumentException>(
                "Can not add property Test3 to Newtonsoft.Json.Linq.ZObject. Property with the same name already exists on object.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList l = new ZObject(p1, p2);

                    ZProperty p3 = new ZProperty("Test3", "III");

                    l[0] = p3;
                    l[1] = p3;
                });
        }

        [Test]
        public void IListSetItemInvalid()
        {
            ExceptionAssert.Throws<ArgumentException>(
                @"Can not add Newtonsoft.Json.Linq.ZValue to Newtonsoft.Json.Linq.ZObject.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList l = new ZObject(p1, p2);

                    l[0] = new ZValue(true);
                });
        }
#endif


        [Test]
        public void IListSyncRoot()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            Assert.IsNotNull(l.SyncRoot);
        }

        [Test]
        public void IListIsSynchronized()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList l = new ZObject(p1, p2);

            Assert.IsFalse(l.IsSynchronized);
        }


#if false
        [Test]
        public void GenericListZTokenContains()
        {
            ZProperty p = new ZProperty("Test", 1);
            IList<ZToken> l = new ZObject(p);

            Assert.IsTrue(l.Contains(p));
            Assert.IsFalse(l.Contains(new ZProperty("Test", 1)));
        }

        [Test]
        public void GenericListZTokenIndexOf()
        {
            ZProperty p = new ZProperty("Test", 1);
            IList<ZToken> l = new ZObject(p);

            Assert.AreEqual(0, l.IndexOf(p));
            Assert.AreEqual(-1, l.IndexOf(new ZProperty("Test", 1)));
        }

        [Test]
        public void GenericListZTokenClear()
        {
            ZProperty p = new ZProperty("Test", 1);
            IList<ZToken> l = new ZObject(p);

            Assert.AreEqual(1, l.Count);

            l.Clear();

            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void GenericListZTokenCopyTo()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList<ZToken> l = new ZObject(p1, p2);

            ZToken[] a = new ZToken[l.Count];

            l.CopyTo(a, 0);

            Assert.AreEqual(p1, a[0]);
            Assert.AreEqual(p2, a[1]);
        }

        [Test]
        public void GenericListZTokenAdd()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList<ZToken> l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            l.Add(p3);

            Assert.AreEqual(3, l.Count);
            Assert.AreEqual(p3, l[2]);
        }

        [Test]
        public void GenericListZTokenAddBadToken()
        {
            ExceptionAssert.Throws<ArgumentException>("Can not add Newtonsoft.Json.Linq.ZValue to Newtonsoft.Json.Linq.ZObject.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList<ZToken> l = new ZObject(p1, p2);

                    l.Add(new ZValue("Bad!"));
                });
        }

        [Test]
        public void GenericListZTokenAddBadValue()
        {
            ExceptionAssert.Throws<ArgumentException>("Can not add Newtonsoft.Json.Linq.ZValue to Newtonsoft.Json.Linq.ZObject.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList<ZToken> l = new ZObject(p1, p2);

                    // string is implicitly converted to ZValue
                    l.Add("Bad!");
                });
        }

        [Test]
        public void GenericListZTokenAddPropertyWithExistingName()
        {
            ExceptionAssert.Throws<ArgumentException>("Can not add property Test2 to Newtonsoft.Json.Linq.ZObject. Property with the same name already exists on object.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList<ZToken> l = new ZObject(p1, p2);

                    ZProperty p3 = new ZProperty("Test2", "II");

                    l.Add(p3);
                });
        }

        [Test]
        public void GenericListZTokenRemove()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList<ZToken> l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            // won't do anything
            Assert.IsFalse(l.Remove(p3));
            Assert.AreEqual(2, l.Count);

            Assert.IsTrue(l.Remove(p1));
            Assert.AreEqual(1, l.Count);
            Assert.IsFalse(l.Contains(p1));
            Assert.IsTrue(l.Contains(p2));

            Assert.IsTrue(l.Remove(p2));
            Assert.AreEqual(0, l.Count);
            Assert.IsFalse(l.Contains(p2));
            Assert.AreEqual(null, p2.Parent);
        }

        [Test]
        public void GenericListZTokenRemoveAt()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList<ZToken> l = new ZObject(p1, p2);

            // won't do anything
            l.RemoveAt(0);

            l.Remove(p1);
            Assert.AreEqual(1, l.Count);

            l.Remove(p2);
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void GenericListZTokenInsert()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList<ZToken> l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            l.Insert(1, p3);
            Assert.AreEqual(l, p3.Parent);

            Assert.AreEqual(p1, l[0]);
            Assert.AreEqual(p3, l[1]);
            Assert.AreEqual(p2, l[2]);
        }

        [Test]
        public void GenericListZTokenIsReadOnly()
        {
            IList<ZToken> l = new ZObject();
            Assert.IsFalse(l.IsReadOnly);
        }

        [Test]
        public void GenericListZTokenSetItem()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            IList<ZToken> l = new ZObject(p1, p2);

            ZProperty p3 = new ZProperty("Test3", "III");

            l[0] = p3;

            Assert.AreEqual(p3, l[0]);
            Assert.AreEqual(p2, l[1]);
        }

        [Test]
        public void GenericListZTokenSetItemAlreadyExists()
        {
            ExceptionAssert.Throws<ArgumentException>("Can not add property Test3 to Newtonsoft.Json.Linq.ZObject. Property with the same name already exists on object.",
                () =>
                {
                    ZProperty p1 = new ZProperty("Test1", 1);
                    ZProperty p2 = new ZProperty("Test2", "Two");
                    IList<ZToken> l = new ZObject(p1, p2);

                    ZProperty p3 = new ZProperty("Test3", "III");

                    l[0] = p3;
                    l[1] = p3;
                });
        }

#if !(NETFX_CORE || PORTABLE || PORTABLE40)
        [Test]
        public void IBindingListSortDirection()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(ListSortDirection.Ascending, l.SortDirection);
        }

        [Test]
        public void IBindingListSortProperty()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(null, l.SortProperty);
        }

        [Test]
        public void IBindingListSupportsChangeNotification()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(true, l.SupportsChangeNotification);
        }

        [Test]
        public void IBindingListSupportsSearching()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(false, l.SupportsSearching);
        }

        [Test]
        public void IBindingListSupportsSorting()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(false, l.SupportsSorting);
        }

        [Test]
        public void IBindingListAllowEdit()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(true, l.AllowEdit);
        }

        [Test]
        public void IBindingListAllowNew()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(true, l.AllowNew);
        }

        [Test]
        public void IBindingListAllowRemove()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(true, l.AllowRemove);
        }

        [Test]
        public void IBindingListAddIndex()
        {
            IBindingList l = new ZObject();
            // do nothing
            l.AddIndex(null);
        }

        [Test]
        public void IBindingListApplySort()
        {
            ExceptionAssert.Throws<NotSupportedException>(
                "Specified method is not supported.",
                () =>
                {
                    IBindingList l = new ZObject();
                    l.ApplySort(null, ListSortDirection.Ascending);
                });
        }

        [Test]
        public void IBindingListRemoveSort()
        {
            ExceptionAssert.Throws<NotSupportedException>(
                "Specified method is not supported.",
                () =>
                {
                    IBindingList l = new ZObject();
                    l.RemoveSort();
                });
        }

        [Test]
        public void IBindingListRemoveIndex()
        {
            IBindingList l = new ZObject();
            // do nothing
            l.RemoveIndex(null);
        }

        [Test]
        public void IBindingListFind()
        {
            ExceptionAssert.Throws<NotSupportedException>(
                "Specified method is not supported.",
                () =>
                {
                    IBindingList l = new ZObject();
                    l.Find(null, null);
                });
        }

        [Test]
        public void IBindingListIsSorted()
        {
            IBindingList l = new ZObject();
            Assert.AreEqual(false, l.IsSorted);
        }

        [Test]
        public void IBindingListAddNew()
        {
            ExceptionAssert.Throws<JsonException>(
                "Could not determine new value to add to 'Newtonsoft.Json.Linq.ZObject'.",
                () =>
                {
                    IBindingList l = new ZObject();
                    l.AddNew();
                });
        }

        [Test]
        public void IBindingListAddNewWithEvent()
        {
            ZObject o = new ZObject();
            o._addingNew += (s, e) => e.NewObject = new ZProperty("Property!");

            IBindingList l = o;
            object newObject = l.AddNew();
            Assert.IsNotNull(newObject);

            ZProperty p = (ZProperty)newObject;
            Assert.AreEqual("Property!", p.Name);
            Assert.AreEqual(o, p.Parent);
        }

        [Test]
        public void ITypedListGetListName()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            ITypedList l = new ZObject(p1, p2);

            Assert.AreEqual(string.Empty, l.GetListName(null));
        }

        [Test]
        public void ITypedListGetItemProperties()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            ITypedList l = new ZObject(p1, p2);

            PropertyDescriptorCollection propertyDescriptors = l.GetItemProperties(null);
            Assert.IsNull(propertyDescriptors);
        }

        [Test]
        public void ListChanged()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            ZObject o = new ZObject(p1, p2);

            ListChangedType? changedType = null;
            int? index = null;

            o.ListChanged += (s, a) =>
            {
                changedType = a.ListChangedType;
                index = a.NewIndex;
            };

            ZProperty p3 = new ZProperty("Test3", "III");

            o.Add(p3);
            Assert.AreEqual(changedType, ListChangedType.ItemAdded);
            Assert.AreEqual(index, 2);
            Assert.AreEqual(p3, ((IList<ZToken>)o)[index.Value]);

            ZProperty p4 = new ZProperty("Test4", "IV");

            ((IList<ZToken>)o)[index.Value] = p4;
            Assert.AreEqual(changedType, ListChangedType.ItemChanged);
            Assert.AreEqual(index, 2);
            Assert.AreEqual(p4, ((IList<ZToken>)o)[index.Value]);
            Assert.IsFalse(((IList<ZToken>)o).Contains(p3));
            Assert.IsTrue(((IList<ZToken>)o).Contains(p4));

            o["Test1"] = 2;
            Assert.AreEqual(changedType, ListChangedType.ItemChanged);
            Assert.AreEqual(index, 0);
            Assert.AreEqual(2, (int)o["Test1"]);
        }
#endif

#if !(NET20 || NET35 || PORTABLE40)
        [Test]
        public void CollectionChanged()
        {
            ZProperty p1 = new ZProperty("Test1", 1);
            ZProperty p2 = new ZProperty("Test2", "Two");
            ZObject o = new ZObject(p1, p2);

            NotifyCollectionChangedAction? changedType = null;
            int? index = null;

            o._collectionChanged += (s, a) =>
            {
                changedType = a.Action;
                index = a.NewStartingIndex;
            };

            ZProperty p3 = new ZProperty("Test3", "III");

            o.Add(p3);
            Assert.AreEqual(changedType, NotifyCollectionChangedAction.Add);
            Assert.AreEqual(index, 2);
            Assert.AreEqual(p3, ((IList<ZToken>)o)[index.Value]);

            ZProperty p4 = new ZProperty("Test4", "IV");

            ((IList<ZToken>)o)[index.Value] = p4;
            Assert.AreEqual(changedType, NotifyCollectionChangedAction.Replace);
            Assert.AreEqual(index, 2);
            Assert.AreEqual(p4, ((IList<ZToken>)o)[index.Value]);
            Assert.IsFalse(((IList<ZToken>)o).Contains(p3));
            Assert.IsTrue(((IList<ZToken>)o).Contains(p4));

            o["Test1"] = 2;
            Assert.AreEqual(changedType, NotifyCollectionChangedAction.Replace);
            Assert.AreEqual(index, 0);
            Assert.AreEqual(2, (int)o["Test1"]);
        }
#endif

        [Test]
        public void GetGeocodeAddress()
        {
            string json = @"{
  ""name"": ""Address: 435 North Mulford Road Rockford, IL 61107"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""435 N Mulford Rd, Rockford, IL 61107, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""IL"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Rockford"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""61107""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""435 N Mulford Rd""
               }
            },
            ""SubAdministrativeAreaName"" : ""Winnebago""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 42.2753076,
        ""south"": 42.2690124,
        ""east"": -88.9964645,
        ""west"": -89.0027597
      }
    },
    ""Point"": {
      ""coordinates"": [ -88.9995886, 42.2721596, 0 ]
    }
  } ]
}";

            ZObject o = ZObject.Parse(json);

            string searchAddress = (string)o["Placemark"][0]["AddressDetails"]["Country"]["AdministrativeArea"]["SubAdministrativeArea"]["Locality"]["Thoroughfare"]["ThoroughfareName"];
            Assert.AreEqual("435 N Mulford Rd", searchAddress);
        }

        [Test]
        public void SetValueWithInvalidPropertyName()
        {
            ExceptionAssert.Throws<ArgumentException>("Set ZObject values with invalid key value: 0. Object property name expected.",
                () =>
                {
                    ZObject o = new ZObject();
                    o[0] = new ZValue(3);
                });
        }

        [Test]
        public void SetValue()
        {
            object key = "TestKey";

            ZObject o = new ZObject();
            o[key] = new ZValue(3);

            Assert.AreEqual(3, (int)o[key]);
        }

        [Test]
        public void ParseMultipleProperties()
        {
            string json = @"{
        ""Name"": ""Name1"",
        ""Name"": ""Name2""
      }";

            ZObject o = ZObject.Parse(json);
            string value = (string)o["Name"];

            Assert.AreEqual("Name2", value);
        }

#if !(NETFX_CORE || PORTABLE || PORTABLE40)
        [Test]
        public void WriteObjectNullDBNullValue()
        {
            DBNull dbNull = DBNull.Value;
            ZValue v = new ZValue(dbNull);
            Assert.AreEqual(DBNull.Value, v.Value);
            Assert.AreEqual(ZTokenType.Null, v.Type);

            ZObject o = new ZObject();
            o["title"] = v;

            string output = o.ToString();

            Assert.AreEqual(@"{
  ""title"": null
}", output);
        }
#endif

        [Test]
        public void InvalidValueCastExceptionMessage()
        {
            ExceptionAssert.Throws<ArgumentException>("Can not convert Object to String.",
                () =>
                {
                    string json = @"{
  ""responseData"": {}, 
  ""responseDetails"": null, 
  ""responseStatus"": 200
}";

                    ZObject o = ZObject.Parse(json);

                    string name = (string)o["responseData"];
                });
        }

        [Test]
        public void InvalidPropertyValueCastExceptionMessage()
        {
            ExceptionAssert.Throws<ArgumentException>("Can not convert Object to String.",
                () =>
                {
                    string json = @"{
  ""responseData"": {}, 
  ""responseDetails"": null, 
  ""responseStatus"": 200
}";

                    ZObject o = ZObject.Parse(json);

                    string name = (string)o.Property("responseData");
                });
        }

        [Test]
        public void ParseIncomplete()
        {
            ExceptionAssert.Throws<Exception>("Unexpected end of content while loading ZObject. Path 'foo', line 1, position 6.",
                () => { ZObject.Parse("{ foo:"); });
        }

        [Test]
        public void LoadFromNestedObject()
        {
            string jsonText = @"{
  ""short"":
  {
    ""error"":
    {
      ""code"":0,
      ""msg"":""No action taken""
    }
  }
}";

            JsonReader reader = new JsonTextReader(new StringReader(jsonText));
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();

            ZObject o = (ZObject)ZToken.ReadFrom(reader);
            Assert.IsNotNull(o);
            Assert.AreEqual(@"{
  ""code"": 0,
  ""msg"": ""No action taken""
}", o.ToString(Formatting.Indented));
        }

        [Test]
        public void LoadFromNestedObjectIncomplete()
        {
            ExceptionAssert.Throws<JsonReaderException>("Unexpected end of content while loading ZObject. Path 'short.error.code', line 6, position 15.",
                () =>
                {
                    string jsonText = @"{
  ""short"":
  {
    ""error"":
    {
      ""code"":0";

                    JsonReader reader = new JsonTextReader(new StringReader(jsonText));
                    reader.Read();
                    reader.Read();
                    reader.Read();
                    reader.Read();
                    reader.Read();

                    ZToken.ReadFrom(reader);
                });
        }

#if !(NETFX_CORE || PORTABLE || PORTABLE40)
        [Test]
        public void GetProperties()
        {
            ZObject o = ZObject.Parse("{'prop1':12,'prop2':'hi!','prop3':null,'prop4':[1,2,3]}");

            ICustomTypeDescriptor descriptor = o;

            PropertyDescriptorCollection properties = descriptor.GetProperties();
            Assert.AreEqual(4, properties.Count);

            PropertyDescriptor prop1 = properties[0];
            Assert.AreEqual("prop1", prop1.Name);
            Assert.AreEqual(typeof(object), prop1.PropertyType);
            Assert.AreEqual(typeof(ZObject), prop1.ComponentType);
            Assert.AreEqual(false, prop1.CanResetValue(o));
            Assert.AreEqual(false, prop1.ShouldSerializeValue(o));

            PropertyDescriptor prop2 = properties[1];
            Assert.AreEqual("prop2", prop2.Name);
            Assert.AreEqual(typeof(object), prop2.PropertyType);
            Assert.AreEqual(typeof(ZObject), prop2.ComponentType);
            Assert.AreEqual(false, prop2.CanResetValue(o));
            Assert.AreEqual(false, prop2.ShouldSerializeValue(o));

            PropertyDescriptor prop3 = properties[2];
            Assert.AreEqual("prop3", prop3.Name);
            Assert.AreEqual(typeof(object), prop3.PropertyType);
            Assert.AreEqual(typeof(ZObject), prop3.ComponentType);
            Assert.AreEqual(false, prop3.CanResetValue(o));
            Assert.AreEqual(false, prop3.ShouldSerializeValue(o));

            PropertyDescriptor prop4 = properties[3];
            Assert.AreEqual("prop4", prop4.Name);
            Assert.AreEqual(typeof(object), prop4.PropertyType);
            Assert.AreEqual(typeof(ZObject), prop4.ComponentType);
            Assert.AreEqual(false, prop4.CanResetValue(o));
            Assert.AreEqual(false, prop4.ShouldSerializeValue(o));
        }
#endif

        [Test]
        public void ParseEmptyObjectWithComment()
        {
            ZObject o = ZObject.Parse("{ /* A Comment */ }");
            Assert.AreEqual(0, o.Count);
        }

        [Test]
        public void FromObjectTimeSpan()
        {
            ZValue v = (ZValue)ZToken.FromObject(TimeSpan.FromDays(1));
            Assert.AreEqual(v.Value, TimeSpan.FromDays(1));

            Assert.AreEqual("1.00:00:00", v.ToString());
        }

        [Test]
        public void FromObjectUri()
        {
            ZValue v = (ZValue)ZToken.FromObject(new Uri("http://www.stuff.co.nz"));
            Assert.AreEqual(v.Value, new Uri("http://www.stuff.co.nz"));

            Assert.AreEqual("http://www.stuff.co.nz/", v.ToString());
        }

        [Test]
        public void FromObjectGuid()
        {
            ZValue v = (ZValue)ZToken.FromObject(new Guid("9065ACF3-C820-467D-BE50-8D4664BEAF35"));
            Assert.AreEqual(v.Value, new Guid("9065ACF3-C820-467D-BE50-8D4664BEAF35"));

            Assert.AreEqual("9065acf3-c820-467d-be50-8d4664beaf35", v.ToString());
        }

        [Test]
        public void ParseAdditionalContent()
        {
            ExceptionAssert.Throws<JsonReaderException>("Additional text encountered after finished reading JSON content: ,. Path '', line 10, position 2.",
                () =>
                {
                    string json = @"{
""Name"": ""Apple"",
""Expiry"": new Date(1230422400000),
""Price"": 3.99,
""Sizes"": [
""Small"",
""Medium"",
""Large""
]
}, 987987";

                    ZObject o = ZObject.Parse(json);
                });
        }

        [Test]
        public void DeepEqualsIgnoreOrder()
        {
            ZObject o1 = new ZObject(
                new ZProperty("null", null),
                new ZProperty("integer", 1),
                new ZProperty("string", "string!"),
                new ZProperty("decimal", 0.5m),
                new ZProperty("array", new JArray(1, 2)));

            Assert.IsTrue(o1.DeepEquals(o1));

            ZObject o2 = new ZObject(
                new ZProperty("null", null),
                new ZProperty("string", "string!"),
                new ZProperty("decimal", 0.5m),
                new ZProperty("integer", 1),
                new ZProperty("array", new JArray(1, 2)));

            Assert.IsTrue(o1.DeepEquals(o2));

            ZObject o3 = new ZObject(
                new ZProperty("null", null),
                new ZProperty("string", "string!"),
                new ZProperty("decimal", 0.5m),
                new ZProperty("integer", 2),
                new ZProperty("array", new JArray(1, 2)));

            Assert.IsFalse(o1.DeepEquals(o3));

            ZObject o4 = new ZObject(
                new ZProperty("null", null),
                new ZProperty("string", "string!"),
                new ZProperty("decimal", 0.5m),
                new ZProperty("integer", 1),
                new ZProperty("array", new JArray(2, 1)));

            Assert.IsFalse(o1.DeepEquals(o4));

            ZObject o5 = new ZObject(
                new ZProperty("null", null),
                new ZProperty("string", "string!"),
                new ZProperty("decimal", 0.5m),
                new ZProperty("integer", 1));

            Assert.IsFalse(o1.DeepEquals(o5));

            Assert.IsFalse(o1.DeepEquals(null));
        }

        [Test]
        public void ToListOnEmptyObject()
        {
            ZObject o = ZObject.Parse(@"{}");
            IList<ZToken> l1 = o.ToList<ZToken>();
            Assert.AreEqual(0, l1.Count);

            IList<KeyValuePair<string, ZToken>> l2 = o.ToList<KeyValuePair<string, ZToken>>();
            Assert.AreEqual(0, l2.Count);

            o = ZObject.Parse(@"{'hi':null}");

            l1 = o.ToList<ZToken>();
            Assert.AreEqual(1, l1.Count);

            l2 = o.ToList<KeyValuePair<string, ZToken>>();
            Assert.AreEqual(1, l2.Count);
        }

        [Test]
        public void EmptyObjectDeepEquals()
        {
            Assert.IsTrue(ZToken.DeepEquals(new ZObject(), new ZObject()));

            ZObject a = new ZObject();
            ZObject b = new ZObject();

            b.Add("hi", "bye");
            b.Remove("hi");

            Assert.IsTrue(ZToken.DeepEquals(a, b));
            Assert.IsTrue(ZToken.DeepEquals(b, a));
        }

        [Test]
        public void GetValueBlogExample()
        {
            ZObject o = ZObject.Parse(@"{
        'name': 'Lower',
        'NAME': 'Upper'
      }");

            string exactMatch = (string)o.GetValue("NAME", StringComparison.OrdinalIgnoreCase);
            // Upper

            string ignoreCase = (string)o.GetValue("Name", StringComparison.OrdinalIgnoreCase);
            // Lower

            Assert.AreEqual("Upper", exactMatch);
            Assert.AreEqual("Lower", ignoreCase);
        }

        [Test]
        public void GetValue()
        {
            ZObject a = new ZObject();
            a["Name"] = "Name!";
            a["name"] = "name!";
            a["title"] = "Title!";

            Assert.AreEqual(null, a.GetValue("NAME", StringComparison.Ordinal));
            Assert.AreEqual(null, a.GetValue("NAME"));
            Assert.AreEqual(null, a.GetValue("TITLE"));
            Assert.AreEqual("Name!", (string)a.GetValue("NAME", StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual("name!", (string)a.GetValue("name", StringComparison.Ordinal));
            Assert.AreEqual(null, a.GetValue(null, StringComparison.Ordinal));
            Assert.AreEqual(null, a.GetValue(null));

            ZToken v;
            Assert.IsFalse(a.TryGetValue("NAME", StringComparison.Ordinal, out v));
            Assert.AreEqual(null, v);

            Assert.IsFalse(a.TryGetValue("NAME", out v));
            Assert.IsFalse(a.TryGetValue("TITLE", out v));

            Assert.IsTrue(a.TryGetValue("NAME", StringComparison.OrdinalIgnoreCase, out v));
            Assert.AreEqual("Name!", (string)v);

            Assert.IsTrue(a.TryGetValue("name", StringComparison.Ordinal, out v));
            Assert.AreEqual("name!", (string)v);

            Assert.IsFalse(a.TryGetValue(null, StringComparison.Ordinal, out v));
        }

        public class FooJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var token = ZToken.FromObject(value, new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                if (token.Type == ZTokenType.Object)
                {
                    var o = (ZObject)token;
                    o.AddFirst(new ZProperty("foo", "bar"));
                    o.WriteTo(writer);
                }
                else
                    token.WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotSupportedException("This custom converter only supportes serialization and not deserialization.");
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanConvert(Type objectType)
            {
                return true;
            }
        }

        [Test]
        public void FromObjectInsideConverterWithCustomSerializer()
        {
            var p = new Person
            {
                Name = "Daniel Wertheim",
            };

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new FooJsonConverter() },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(p, settings);

            Assert.AreEqual(@"{""foo"":""bar"",""name"":""Daniel Wertheim"",""birthDate"":""0001-01-01T00:00:00"",""lastModified"":""0001-01-01T00:00:00""}", json);
        }
#endif

    }
}
