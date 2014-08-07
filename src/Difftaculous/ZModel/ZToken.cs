
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Paths;


namespace Difftaculous.ZModel
{
    internal abstract class ZToken
    {
        private readonly List<IHint> _hints = new List<IHint>();
        private readonly List<ICaveat> _caveats = new List<ICaveat>();


        public ZToken Parent { get; set; }
        public IEnumerable<IHint> Hints { get { return _hints; } }
        public IEnumerable<ICaveat> Caveats { get { return _caveats; } }

        private static readonly TokenType[] NumberTypes = { TokenType.Value }; // new[] { TokenType.Integer, TokenType.Float, TokenType.String, TokenType.Comment, TokenType.Raw, TokenType.Boolean };


        // NOTE: Json.Net defines this as taking an object as the key - for arrays, perhaps?
        public virtual ZToken this[string key]
        {
            get { throw new InvalidOperationException(string.Format("Cannot access child value on {0}.", GetType())); }
        }


        public void AddHint(IHint hint)
        {
            _hints.Add(hint);
        }


        public void AddCaveat(ICaveat caveat)
        {
            _caveats.Add(caveat);
        }


        public abstract TokenType Type { get; }



        public ZToken SelectToken(DiffPath path)
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
            return path.Evaluate(this);
        }



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



        private IEnumerable<ZToken> Ancestors
        {
            get
            {
                for (ZToken parent = Parent; parent != null; parent = parent.Parent)
                {
                    yield return parent;
                }
            }
        }



        public static explicit operator int(ZToken value)
        {
            ZValue v = EnsureValue(value);

            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException(string.Format("Can not convert {0} to Int32.", GetType(value)));
            }

            return Convert.ToInt32(v.Value, CultureInfo.InvariantCulture);
        }


        private static string GetType(ZToken token)
        {
            // TODO - put this in!
            // ValidationUtils.ArgumentNotNull(token, "token");

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

    }
}
