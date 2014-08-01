
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Difftaculous.Paths;


namespace Difftaculous.ZModel
{
    internal abstract class ZToken : IToken
    {
        public IToken Parent { get; set; }

        // NOTE: Json.Net defines this as taking an object as the key - for arrays, perhaps?
        public virtual IToken this[string key]
        {
            get { throw new InvalidOperationException(string.Format("Cannot access child value on {0}.", GetType())); }
        }


        public abstract TokenType Type { get; }


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
                    IToken current = ancestors[i];
                    IToken next = null;

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
                                sb.Append(((IProperty)current).Name);
                                break;

                            case TokenType.Array:
                                IArray array = (IArray)current;
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



        private IEnumerable<IToken> Ancestors
        {
            get
            {
                for (IToken parent = Parent; parent != null; parent = parent.Parent)
                {
                    yield return parent;
                }
            }
        }
    }
}
