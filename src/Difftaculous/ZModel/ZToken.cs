
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Paths;


namespace Difftaculous.ZModel
{
    internal abstract class ZToken : IZEnumerable<ZToken> // , IJsonLineInfo, ICloneable, IDynamicMetaObjectProvider
    {
        private ZContainer _parent;
        private ZToken _previous;
        private ZToken _next;


#if false
        private static JTokenEqualityComparer _equalityComparer;

        private int? _lineNumber;
        private int? _linePosition;

        private static readonly JTokenType[] BooleanTypes = new[] { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean };
#endif

        private static readonly TokenType[] NumberTypes = { TokenType.Integer, TokenType.Value }; // new[] { TokenType.Float, TokenType.String, TokenType.Comment, TokenType.Raw, TokenType.Boolean };

#if false
#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
        private static readonly JTokenType[] BigIntegerTypes = new[] { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean, JTokenType.Bytes };
#endif
        private static readonly JTokenType[] StringTypes = new[] { JTokenType.Date, JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean, JTokenType.Bytes, JTokenType.Guid, JTokenType.TimeSpan, JTokenType.Uri };
        private static readonly JTokenType[] GuidTypes = new[] { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Guid, JTokenType.Bytes };
        private static readonly JTokenType[] TimeSpanTypes = new[] { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.TimeSpan };
        private static readonly JTokenType[] UriTypes = new[] { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Uri };
        private static readonly JTokenType[] CharTypes = new[] { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw };
        private static readonly JTokenType[] DateTimeTypes = new[] { JTokenType.Date, JTokenType.String, JTokenType.Comment, JTokenType.Raw };
        private static readonly JTokenType[] BytesTypes = new[] { JTokenType.Bytes, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Integer };

        /// <summary>
        /// Gets a comparer that can compare two tokens for value equality.
        /// </summary>
        /// <value>A <see cref="JTokenEqualityComparer"/> that can compare two nodes for value equality.</value>
        public static JTokenEqualityComparer EqualityComparer
        {
            get
            {
                if (_equalityComparer == null)
                    _equalityComparer = new JTokenEqualityComparer();

                return _equalityComparer;
            }
        }

#endif


        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public ZContainer Parent
        {
            // [DebuggerStepThrough]
            get { return _parent; }
            internal set { _parent = value; }
        }


        /// <summary>
        /// Gets the root <see cref="ZToken"/> of this <see cref="ZToken"/>.
        /// </summary>
        /// <value>The root <see cref="ZToken"/> of this <see cref="ZToken"/>.</value>
        public ZToken Root
        {
            get
            {
                ZContainer parent = Parent;
                if (parent == null)
                    return this;

                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                return parent;
            }
        }



#if false
        internal abstract JToken CloneToken();
        internal abstract bool DeepEquals(JToken node);
#endif 


        /// <summary>
        /// Gets the node type for this <see cref="ZToken"/>.
        /// </summary>
        /// <value>The type.</value>
        public abstract TokenType Type { get; }


        /// <summary>
        /// Gets a value indicating whether this token has child tokens.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this token has child values; otherwise, <c>false</c>.
        /// </value>
        public abstract bool HasValues { get; }


#if false
        /// <summary>
        /// Compares the values of two tokens, including the values of all descendant tokens.
        /// </summary>
        /// <param name="t1">The first <see cref="JToken"/> to compare.</param>
        /// <param name="t2">The second <see cref="JToken"/> to compare.</param>
        /// <returns>true if the tokens are equal; otherwise false.</returns>
        public static bool DeepEquals(JToken t1, JToken t2)
        {
            return (t1 == t2 || (t1 != null && t2 != null && t1.DeepEquals(t2)));
        }
#endif


        /// <summary>
        /// Gets the next sibling token of this node.
        /// </summary>
        /// <value>The <see cref="ZToken"/> that contains the next sibling token.</value>
        public ZToken Next
        {
            get { return _next; }
            internal set { _next = value; }
        }


        /// <summary>
        /// Gets the previous sibling token of this node.
        /// </summary>
        /// <value>The <see cref="ZToken"/> that contains the previous sibling token.</value>
        public ZToken Previous
        {
            get { return _previous; }
            internal set { _previous = value; }
        }


        /// <summary>
        /// Gets the path of the token. 
        /// </summary>
        public DiffPath Path
        {
            get
            {
                // TODO - this should really be building up an expression directly so we don't have to parse the JsonPath at the end

                // Based on Json.Net...see JToken.cs, line 181
                var ancestors = Ancestors.Reverse().ToList();
                ancestors.Add(this);

                StringBuilder sb = new StringBuilder("$");

                for (int i = 0; i < ancestors.Count; i++)
                {
                    ZToken current = ancestors[i];
                    ZToken next = null;

                    if (i + 1 < ancestors.Count)
                    {
                        next = ancestors[i + 1];
                    }
                    else if (ancestors[i].Type == TokenType.Property)
                    {
                        next = ancestors[i];
                    }

                    if (next != null)
                    {
                        switch (current.Type)
                        {
                            case TokenType.Property:
                                sb.Append(".");
                                sb.Append(((ZProperty)current).Name);
                                break;

                            case TokenType.Array:
                                ZArray array = (ZArray)current;
                                int index = array.IndexOf(next);

                                sb.Append("[");
                                sb.Append(index);
                                sb.Append("]");
                                break;
                        }
                    }
                }

                return DiffPath.FromJsonPath(sb.ToString());
            }
        }


        internal ZToken()
        {
        }


#if false
        /// <summary>
        /// Adds the specified content immediately after this token.
        /// </summary>
        /// <param name="content">A content object that contains simple content or a collection of content objects to be added after this token.</param>
        public void AddAfterSelf(object content)
        {
            if (_parent == null)
                throw new InvalidOperationException("The parent is missing.");

            int index = _parent.IndexOfItem(this);
            _parent.AddInternal(index + 1, content, false);
        }

        /// <summary>
        /// Adds the specified content immediately before this token.
        /// </summary>
        /// <param name="content">A content object that contains simple content or a collection of content objects to be added before this token.</param>
        public void AddBeforeSelf(object content)
        {
            if (_parent == null)
                throw new InvalidOperationException("The parent is missing.");

            int index = _parent.IndexOfItem(this);
            _parent.AddInternal(index, content, false);
        }
#endif



        /// <summary>
        /// Returns a collection of the ancestor tokens of this token.
        /// </summary>
        /// <returns>A collection of the ancestor tokens of this token.</returns>
        private IEnumerable<ZToken> Ancestors
        {
            // NOTE: Json.Net defines this as a function rather than a property.
            get
            {
                for (ZToken parent = Parent; parent != null; parent = parent.Parent)
                {
                    yield return parent;
                }
            }
        }



#if false
        /// <summary>
        /// Returns a collection of the sibling tokens after this token, in document order.
        /// </summary>
        /// <returns>A collection of the sibling tokens after this tokens, in document order.</returns>
        public IEnumerable<JToken> AfterSelf()
        {
            if (Parent == null)
                yield break;

            for (JToken o = Next; o != null; o = o.Next)
            {
                yield return o;
            }
        }

        /// <summary>
        /// Returns a collection of the sibling tokens before this token, in document order.
        /// </summary>
        /// <returns>A collection of the sibling tokens before this token, in document order.</returns>
        public IEnumerable<JToken> BeforeSelf()
        {
            for (JToken o = Parent.First; o != this; o = o.Next)
            {
                yield return o;
            }
        }
#endif


        /// <summary>
        /// Gets the <see cref="ZToken"/> with the specified key.
        /// </summary>
        /// <value>The <see cref="ZToken"/> with the specified key.</value>
        public virtual ZToken this[object key]
        {
            get { throw new InvalidOperationException(string.Format("Cannot access child value on {0}.", GetType())); }
            set { throw new InvalidOperationException(string.Format("Cannot set child value on {0}.", GetType())); }
        }



#if false
        /// <summary>
        /// Gets the <see cref="ZToken"/> with the specified key converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to convert the token to.</typeparam>
        /// <param name="key">The token key.</param>
        /// <returns>The converted token value.</returns>
        public virtual T Value<T>(object key)
        {
            ZToken token = this[key];

            // null check to fix MonoTouch issue - https://github.com/dolbz/Newtonsoft.Json/commit/a24e3062846b30ee505f3271ac08862bb471b822
            return token == null ? default(T) : Extensions.Convert<ZToken, T>(token);
        }
#endif


        /// <summary>
        /// Get the first child token of this token.
        /// </summary>
        /// <value>A <see cref="ZToken"/> containing the first child token of the <see cref="ZToken"/>.</value>
        public virtual ZToken First
        {
            get { throw new InvalidOperationException(string.Format("Cannot access child value on {0}.", GetType())); }
        }


        /// <summary>
        /// Get the last child token of this token.
        /// </summary>
        /// <value>A <see cref="ZToken"/> containing the last child token of the <see cref="ZToken"/>.</value>
        public virtual ZToken Last
        {
            get { throw new InvalidOperationException(string.Format("Cannot access child value on {0}.", GetType())); }
        }



        /// <summary>
        /// Returns a collection of the child tokens of this token, in document order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ZToken"/> containing the child tokens of this <see cref="ZToken"/>, in document order.</returns>
        public virtual ZEnumerable<ZToken> Children()
        {
            return ZEnumerable<ZToken>.Empty;
        }


#if false
        /// <summary>
        /// Returns a collection of the child tokens of this token, in document order, filtered by the specified type.
        /// </summary>
        /// <typeparam name="T">The type to filter the child tokens on.</typeparam>
        /// <returns>A <see cref="JEnumerable{T}"/> containing the child tokens of this <see cref="JToken"/>, in document order.</returns>
        public JEnumerable<T> Children<T>() where T : JToken
        {
            return new JEnumerable<T>(Children().OfType<T>());
        }
#endif



        /// <summary>
        /// Returns a collection of the child values of this token, in document order.
        /// </summary>
        /// <typeparam name="T">The type to convert the values to.</typeparam>
        /// <returns>A <see cref="IEnumerable{T}"/> containing the child values of this <see cref="ZToken"/>, in document order.</returns>
        public virtual IEnumerable<T> Values<T>()
        {
            throw new InvalidOperationException(string.Format("Cannot access child value on {0}.", GetType()));
        }



#if false
        /// <summary>
        /// Removes this token from its parent.
        /// </summary>
        public void Remove()
        {
            if (_parent == null)
                throw new InvalidOperationException("The parent is missing.");

            _parent.RemoveItem(this);
        }

        /// <summary>
        /// Replaces this token with the specified token.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Replace(JToken value)
        {
            if (_parent == null)
                throw new InvalidOperationException("The parent is missing.");

            _parent.ReplaceItem(this, value);
        }

        /// <summary>
        /// Writes this token to a <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the token.</param>
        public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

        /// <summary>
        /// Returns the indented JSON for this token.
        /// </summary>
        /// <returns>
        /// The indented JSON for this token.
        /// </returns>
        public override string ToString()
        {
            return ToString(Formatting.Indented);
        }

        /// <summary>
        /// Returns the JSON for this token using the given formatting and converters.
        /// </summary>
        /// <param name="formatting">Indicates how the output is formatted.</param>
        /// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the token.</param>
        /// <returns>The JSON for this token using the given formatting and converters.</returns>
        public string ToString(Formatting formatting, params JsonConverter[] converters)
        {
            using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonTextWriter jw = new JsonTextWriter(sw);
                jw.Formatting = formatting;

                WriteTo(jw, converters);

                return sw.ToString();
            }
        }
#endif


        private static ZValue EnsureValue(ZToken value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value is ZProperty)
            {
                value = ((ZProperty)value).Value;
            }

            ZValue v = value as ZValue;

            return v;
        }



        private static string GetType(ZToken token)
        {
            ValidationUtils.ArgumentNotNull(token, "token");

            if (token is ZProperty)
            {
                token = ((ZProperty)token).Value;
            }

            return token.Type.ToString();
        }



        private static bool ValidateToken(ZToken o, TokenType[] validTypes, bool nullable)
        {
            return (Array.IndexOf(validTypes, o.Type) != -1) || (nullable && (o.Type == TokenType.Null || o.Type == TokenType.Undefined));
        }



        #region Cast from operators
#if false
        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator bool(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BooleanTypes, false))
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return Convert.ToBoolean((int)(BigInteger)v.Value);
#endif

            return Convert.ToBoolean(v.Value, CultureInfo.InvariantCulture);
        }

#if !NET20
        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.DateTimeOffset"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTimeOffset(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, false))
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value is DateTimeOffset)
                return (DateTimeOffset)v.Value;
            if (v.Value is string)
                return DateTimeOffset.Parse((string)v.Value, CultureInfo.InvariantCulture);
            return new DateTimeOffset(Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture));
        }
#endif

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Boolean}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator bool?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BooleanTypes, true))
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return Convert.ToBoolean((int)(BigInteger)v.Value);
#endif

            return (v.Value != null) ? (bool?)Convert.ToBoolean(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator long(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (long)(BigInteger)v.Value;
#endif

            return Convert.ToInt64(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{DateTime}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTime?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, true))
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !NET20
            if (v.Value is DateTimeOffset)
                return ((DateTimeOffset)v.Value).DateTime;
#endif

            return (v.Value != null) ? (DateTime?)Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture) : null;
        }

#if !NET20
        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{DateTimeOffset}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTimeOffset?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, true))
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value == null)
                return null;
            if (v.Value is DateTimeOffset)
                return (DateTimeOffset?)v.Value;
            if (v.Value is string)
                return DateTimeOffset.Parse((string)v.Value, CultureInfo.InvariantCulture);
            return new DateTimeOffset(Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture));
        }
#endif

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Decimal}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator decimal?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (decimal?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (decimal?)Convert.ToDecimal(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Double}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (double?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (double?)Convert.ToDouble(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Char}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator char?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, CharTypes, true))
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (char?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (char?)Convert.ToChar(v.Value, CultureInfo.InvariantCulture) : null;
        }
#endif


        /// <summary>
        /// Performs an explicit conversion from <see cref="ZToken"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int(ZToken value)
        {
            ZValue v = EnsureValue(value);

            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException(string.Format("Can not convert {0} to Int32.", GetType(value)));
            }

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (int)(BigInteger)v.Value;
#endif

            return Convert.ToInt32(v.Value, CultureInfo.InvariantCulture);
        }


#if false
        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator short(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (short)(BigInteger)v.Value;
#endif

            return Convert.ToInt16(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.UInt16"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator ushort(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (ushort)(BigInteger)v.Value;
#endif

            return Convert.ToUInt16(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Char"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator char(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, CharTypes, false))
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (char)(BigInteger)v.Value;
#endif

            return Convert.ToChar(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator byte(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (byte)(BigInteger)v.Value;
#endif

            return Convert.ToByte(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.SByte"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator sbyte(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (sbyte)(BigInteger)v.Value;
#endif

            return Convert.ToSByte(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Int32}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (int?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (int?)Convert.ToInt32(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Int16}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator short?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (short?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (short?)Convert.ToInt16(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{UInt16}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator ushort?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (ushort?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (ushort?)Convert.ToUInt16(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Byte}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator byte?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (byte?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (byte?)Convert.ToByte(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{SByte}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator sbyte?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (sbyte?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (sbyte?)Convert.ToByte(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTime(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, false))
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !NET20
            if (v.Value is DateTimeOffset)
                return ((DateTimeOffset)v.Value).DateTime;
#endif

            return Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Int64}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator long?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (long?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (long?)Convert.ToInt64(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{Single}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator float?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (float?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (float?)Convert.ToSingle(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Decimal"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator decimal(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (decimal)(BigInteger)v.Value;
#endif

            return Convert.ToDecimal(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{UInt32}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator uint?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (uint?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (uint?)Convert.ToUInt32(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="Nullable{UInt64}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator ulong?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (ulong?)(BigInteger)v.Value;
#endif

            return (v.Value != null) ? (ulong?)Convert.ToUInt64(v.Value, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (double)(BigInteger)v.Value;
#endif

            return Convert.ToDouble(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator float(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (float)(BigInteger)v.Value;
#endif

            return Convert.ToSingle(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator string(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, StringTypes, true))
                throw new ArgumentException("Can not convert {0} to String.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value == null)
                return null;
            if (v.Value is byte[])
                return Convert.ToBase64String((byte[])v.Value);
#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return ((BigInteger)v.Value).ToString(CultureInfo.InvariantCulture);
#endif

            return Convert.ToString(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.UInt32"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator uint(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (uint)(BigInteger)v.Value;
#endif

            return Convert.ToUInt32(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.UInt64"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [CLSCompliant(false)]
        public static explicit operator ulong(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return (ulong)(BigInteger)v.Value;
#endif

            return Convert.ToUInt64(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="T:System.Byte[]"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator byte[](JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BytesTypes, false))
                throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value is string)
                return Convert.FromBase64String(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
            if (v.Value is BigInteger)
                return ((BigInteger)v.Value).ToByteArray();
#endif

            if (v.Value is byte[])
                return (byte[])v.Value;

            throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Guid(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, GuidTypes, false))
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value is byte[])
                return new Guid((byte[])v.Value);

            return (v.Value is Guid) ? (Guid)v.Value : new Guid(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Guid?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, GuidTypes, true))
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value == null)
                return null;

            if (v.Value is byte[])
                return new Guid((byte[])v.Value);

            return (v.Value is Guid) ? (Guid)v.Value : new Guid(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.TimeSpan"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator TimeSpan(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, TimeSpanTypes, false))
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            return (v.Value is TimeSpan) ? (TimeSpan)v.Value : ConvertUtils.ParseTimeSpan(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.TimeSpan"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator TimeSpan?(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, TimeSpanTypes, true))
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value == null)
                return null;

            return (v.Value is TimeSpan) ? (TimeSpan)v.Value : ConvertUtils.ParseTimeSpan(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Newtonsoft.Json.Linq.JToken"/> to <see cref="System.Uri"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Uri(JToken value)
        {
            if (value == null)
                return null;

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, UriTypes, true))
                throw new ArgumentException("Can not convert {0} to Uri.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value == null)
                return null;

            return (v.Value is Uri) ? (Uri)v.Value : new Uri(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
        private static BigInteger ToBigInteger(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BigIntegerTypes, false))
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            return ConvertUtils.ToBigInteger(v.Value);
        }

        private static BigInteger? ToBigIntegerNullable(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BigIntegerTypes, true))
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));

            if (v.Value == null)
                return null;

            return ConvertUtils.ToBigInteger(v.Value);
        }
#endif
#endif
        #endregion


#if false
        #region Cast to operators
        /// <summary>
        /// Performs an implicit conversion from <see cref="Boolean"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(bool value)
        {
            return new JValue(value);
        }

#if !NET20
        /// <summary>
        /// Performs an implicit conversion from <see cref="DateTimeOffset"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTimeOffset value)
        {
            return new JValue(value);
        }
#endif

        /// <summary>
        /// Performs an implicit conversion from <see cref="Byte"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(byte value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Byte}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(byte? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SByte"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(sbyte value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{SByte}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(sbyte? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Boolean}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(bool? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Int64}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(long value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{DateTime}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTime? value)
        {
            return new JValue(value);
        }

#if !NET20
        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{DateTimeOffset}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTimeOffset? value)
        {
            return new JValue(value);
        }
#endif

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Decimal}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(decimal? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Double}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(double? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Int16"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(short value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UInt16"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(ushort value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Int32"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(int value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Int32}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(int? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DateTime"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTime value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Int64}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(long? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Single}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(float? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Decimal"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(decimal value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Int16}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(short? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{UInt16}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(ushort? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{UInt32}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(uint? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{UInt64}"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(ulong? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Double"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(double value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Single"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(float value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="String"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(string value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UInt32"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(uint value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UInt64"/> to <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        [CLSCompliant(false)]
        public static implicit operator JToken(ulong value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Byte[]"/> to <see cref="Newtonsoft.Json.Linq.JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(byte[] value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Uri"/> to <see cref="Newtonsoft.Json.Linq.JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(Uri value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.TimeSpan"/> to <see cref="Newtonsoft.Json.Linq.JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(TimeSpan value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{TimeSpan}"/> to <see cref="Newtonsoft.Json.Linq.JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(TimeSpan? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Guid"/> to <see cref="Newtonsoft.Json.Linq.JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(Guid value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Nullable{Guid}"/> to <see cref="Newtonsoft.Json.Linq.JToken"/>.
        /// </summary>
        /// <param name="value">The value to create a <see cref="JValue"/> from.</param>
        /// <returns>The <see cref="JValue"/> initialized with the specified value.</returns>
        public static implicit operator JToken(Guid? value)
        {
            return new JValue(value);
        }
        #endregion
#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ZToken>)this).GetEnumerator();
        }


        IEnumerator<ZToken> IEnumerable<ZToken>.GetEnumerator()
        {
            return Children().GetEnumerator();
        }


#if false
        internal abstract int GetDeepHashCode();
#endif


        IZEnumerable<ZToken> IZEnumerable<ZToken>.this[object key]
        {
            get { return this[key]; }
        }


#if false
        /// <summary>
        /// Creates an <see cref="JsonReader"/> for this token.
        /// </summary>
        /// <returns>An <see cref="JsonReader"/> that can be used to read this token and its descendants.</returns>
        public JsonReader CreateReader()
        {
            return new JTokenReader(this, Path);
        }

        internal static JToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");

            JToken token;
            using (JTokenWriter jsonWriter = new JTokenWriter())
            {
                jsonSerializer.Serialize(jsonWriter, o);
                token = jsonWriter.Token;
            }

            return token;
        }

        /// <summary>
        /// Creates a <see cref="JToken"/> from an object.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="JToken"/>.</param>
        /// <returns>A <see cref="JToken"/> with the value of the specified object</returns>
        public static JToken FromObject(object o)
        {
            return FromObjectInternal(o, JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Creates a <see cref="JToken"/> from an object using the specified <see cref="JsonSerializer"/>.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="JToken"/>.</param>
        /// <param name="jsonSerializer">The <see cref="JsonSerializer"/> that will be used when reading the object.</param>
        /// <returns>A <see cref="JToken"/> with the value of the specified object</returns>
        public static JToken FromObject(object o, JsonSerializer jsonSerializer)
        {
            return FromObjectInternal(o, jsonSerializer);
        }

        /// <summary>
        /// Creates the specified .NET type from the <see cref="JToken"/>.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <returns>The new object created from the JSON value.</returns>
        public T ToObject<T>()
        {
            return (T)ToObject(typeof(T));
        }

        /// <summary>
        /// Creates the specified .NET type from the <see cref="JToken"/>.
        /// </summary>
        /// <param name="objectType">The object type that the token will be deserialized to.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public object ToObject(Type objectType)
        {
            if (JsonConvert.DefaultSettings == null)
            {
                PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(objectType);

                switch (typeCode)
                {
                    case PrimitiveTypeCode.BooleanNullable:
                        return (bool?)this;
                    case PrimitiveTypeCode.Boolean:
                        return (bool)this;
                    case PrimitiveTypeCode.CharNullable:
                        return (char?)this;
                    case PrimitiveTypeCode.Char:
                        return (char)this;
                    case PrimitiveTypeCode.SByte:
                        return (sbyte?)this;
                    case PrimitiveTypeCode.SByteNullable:
                        return (sbyte)this;
                    case PrimitiveTypeCode.ByteNullable:
                        return (byte?)this;
                    case PrimitiveTypeCode.Byte:
                        return (byte)this;
                    case PrimitiveTypeCode.Int16Nullable:
                        return (short?)this;
                    case PrimitiveTypeCode.Int16:
                        return (short)this;
                    case PrimitiveTypeCode.UInt16Nullable:
                        return (ushort?)this;
                    case PrimitiveTypeCode.UInt16:
                        return (ushort)this;
                    case PrimitiveTypeCode.Int32Nullable:
                        return (int?)this;
                    case PrimitiveTypeCode.Int32:
                        return (int)this;
                    case PrimitiveTypeCode.UInt32Nullable:
                        return (uint?)this;
                    case PrimitiveTypeCode.UInt32:
                        return (uint)this;
                    case PrimitiveTypeCode.Int64Nullable:
                        return (long?)this;
                    case PrimitiveTypeCode.Int64:
                        return (long)this;
                    case PrimitiveTypeCode.UInt64Nullable:
                        return (ulong?)this;
                    case PrimitiveTypeCode.UInt64:
                        return (ulong)this;
                    case PrimitiveTypeCode.SingleNullable:
                        return (float?)this;
                    case PrimitiveTypeCode.Single:
                        return (float)this;
                    case PrimitiveTypeCode.DoubleNullable:
                        return (double?)this;
                    case PrimitiveTypeCode.Double:
                        return (double)this;
                    case PrimitiveTypeCode.DecimalNullable:
                        return (decimal?)this;
                    case PrimitiveTypeCode.Decimal:
                        return (decimal)this;
                    case PrimitiveTypeCode.DateTimeNullable:
                        return (DateTime?)this;
                    case PrimitiveTypeCode.DateTime:
                        return (DateTime)this;
#if !NET20
                    case PrimitiveTypeCode.DateTimeOffsetNullable:
                        return (DateTimeOffset?)this;
                    case PrimitiveTypeCode.DateTimeOffset:
                        return (DateTimeOffset)this;
#endif
                    case PrimitiveTypeCode.String:
                        return (string)this;
                    case PrimitiveTypeCode.GuidNullable:
                        return (Guid?)this;
                    case PrimitiveTypeCode.Guid:
                        return (Guid)this;
                    case PrimitiveTypeCode.Uri:
                        return (Uri)this;
                    case PrimitiveTypeCode.TimeSpanNullable:
                        return (TimeSpan?)this;
                    case PrimitiveTypeCode.TimeSpan:
                        return (TimeSpan)this;
#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
                    case PrimitiveTypeCode.BigIntegerNullable:
                        return ToBigIntegerNullable(this);
                    case PrimitiveTypeCode.BigInteger:
                        return ToBigInteger(this);
#endif
                }
            }

            return ToObject(objectType, JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Creates the specified .NET type from the <see cref="JToken"/> using the specified <see cref="JsonSerializer"/>.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <param name="jsonSerializer">The <see cref="JsonSerializer"/> that will be used when creating the object.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public T ToObject<T>(JsonSerializer jsonSerializer)
        {
            return (T)ToObject(typeof(T), jsonSerializer);
        }

        /// <summary>
        /// Creates the specified .NET type from the <see cref="JToken"/> using the specified <see cref="JsonSerializer"/>.
        /// </summary>
        /// <param name="objectType">The object type that the token will be deserialized to.</param>
        /// <param name="jsonSerializer">The <see cref="JsonSerializer"/> that will be used when creating the object.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public object ToObject(Type objectType, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");

            using (JTokenReader jsonReader = new JTokenReader(this))
            {
                return jsonSerializer.Deserialize(jsonReader, objectType);
            }
        }

        /// <summary>
        /// Creates a <see cref="JToken"/> from a <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="JsonReader"/> positioned at the token to read into this <see cref="JToken"/>.</param>
        /// <returns>
        /// An <see cref="JToken"/> that contains the token and its descendant tokens
        /// that were read from the reader. The runtime type of the token is determined
        /// by the token type of the first token encountered in the reader.
        /// </returns>
        public static JToken ReadFrom(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");

            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                    throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
            }

            IJsonLineInfo lineInfo = reader as IJsonLineInfo;

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return JObject.Load(reader);
                case JsonToken.StartArray:
                    return JArray.Load(reader);
                case JsonToken.StartConstructor:
                    return JConstructor.Load(reader);
                case JsonToken.PropertyName:
                    return JProperty.Load(reader);
                case JsonToken.String:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Date:
                case JsonToken.Boolean:
                case JsonToken.Bytes:
                    JValue v = new JValue(reader.Value);
                    v.SetLineInfo(lineInfo);
                    return v;
                case JsonToken.Comment:
                    v = JValue.CreateComment(reader.Value.ToString());
                    v.SetLineInfo(lineInfo);
                    return v;
                case JsonToken.Null:
                    v = JValue.CreateNull();
                    v.SetLineInfo(lineInfo);
                    return v;
                case JsonToken.Undefined:
                    v = JValue.CreateUndefined();
                    v.SetLineInfo(lineInfo);
                    return v;
                default:
                    throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
        }

        /// <summary>
        /// Load a <see cref="JToken"/> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="String"/> that contains JSON.</param>
        /// <returns>A <see cref="JToken"/> populated from the string that contains JSON.</returns>
        public static JToken Parse(string json)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                JToken t = Load(reader);

                if (reader.Read() && reader.TokenType != JsonToken.Comment)
                    throw JsonReaderException.Create(reader, "Additional text found in JSON string after parsing content.");

                return t;
            }
        }

        /// <summary>
        /// Creates a <see cref="JToken"/> from a <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="JsonReader"/> positioned at the token to read into this <see cref="JToken"/>.</param>
        /// <returns>
        /// An <see cref="JToken"/> that contains the token and its descendant tokens
        /// that were read from the reader. The runtime type of the token is determined
        /// by the token type of the first token encountered in the reader.
        /// </returns>
        public static JToken Load(JsonReader reader)
        {
            return ReadFrom(reader);
        }

        internal void SetLineInfo(IJsonLineInfo lineInfo)
        {
            if (lineInfo == null || !lineInfo.HasLineInfo())
                return;

            SetLineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
        }

        internal void SetLineInfo(int lineNumber, int linePosition)
        {
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            return (_lineNumber != null && _linePosition != null);
        }

        int IJsonLineInfo.LineNumber
        {
            get { return _lineNumber ?? 0; }
        }

        int IJsonLineInfo.LinePosition
        {
            get { return _linePosition ?? 0; }
        }
#endif


        public ZToken SelectToken(DiffPath path)
        {
            return SelectToken(path, false);
        }


        public ZToken SelectToken(DiffPath path, bool errorWhenNoMatch)
        {
            ZToken token = null;
            foreach (ZToken t in path.Evaluate(this))
            {
                if (token != null)
                {
                    throw new JsonPathException("Path returned multiple tokens");
                }

                token = t;
            }

            return token;
        }


        public IEnumerable<ZToken> SelectTokens(DiffPath path)
        {
            return SelectTokens(path, false);
        }


        public IEnumerable<ZToken> SelectTokens(DiffPath path, bool errorWhenNoMatch)
        {
            return path.Evaluate(this);
        }



#if false
#if !(NET35 || NET20 || PORTABLE40)
        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        protected virtual DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicProxyMetaObject<JToken>(parameter, this, new DynamicProxy<JToken>(), true);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return GetMetaObject(parameter);
        }
#endif

#if !(NETFX_CORE || PORTABLE || PORTABLE40)
        object ICloneable.Clone()
        {
            return DeepClone();
        }
#endif

        /// <summary>
        /// Creates a new instance of the <see cref="JToken"/>. All child tokens are recursively cloned.
        /// </summary>
        /// <returns>A new instance of the <see cref="JToken"/>.</returns>
        public JToken DeepClone()
        {
            return CloneToken();
        }
#endif




        // ---------------------------------------------------------------
        // Difftaculous-specific stuff below!
        // ---------------------------------------------------------------


        private readonly List<IHint> _hints = new List<IHint>();
        private readonly List<ICaveat> _caveats = new List<ICaveat>();

        public IEnumerable<IHint> Hints { get { return _hints; } }
        public IEnumerable<ICaveat> Caveats { get { return _caveats; } }

        public void AddHint(IHint hint)
        {
            _hints.Add(hint);
        }


        public void AddCaveat(ICaveat caveat)
        {
            _caveats.Add(caveat);
        }

    }
}
