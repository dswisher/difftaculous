
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Difftaculous.ZModel;

namespace Difftaculous.Paths
{
    internal class FieldFilter : PathFilter
    {
        public string Name { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current, bool errorWhenNoMatch)
        {
            foreach (ZToken t in current)
            {
                ZObject o = t as ZObject;
                if (o != null)
                {
                    if (Name != null)
                    {
                        ZToken v = o[Name];

                        if (v != null)
                        {
                            yield return v;
                        }
                        else if (errorWhenNoMatch)
                        {
                            throw new JsonPathException(string.Format("Property '{0}' does not exist on ZObject.", Name));
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, ZToken> p in o)
                        {
                            yield return p.Value;
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonPathException(string.Format("Property '{0}' not valid on {1}.", Name ?? "*", t.GetType().Name));
                    }
                }
            }
        }



        public override void AddJsonPath(StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                sb.Append(".");
            }

            if (!string.IsNullOrEmpty(Name))
            {
                if (Name.Any(c => !char.IsLetterOrDigit(c) && (c != '$')))
                {
                    sb.AppendFormat("['{0}']", Name);
                }
                else
                {
                    sb.Append(Name);
                }
            }
            else
            {
                sb.Append("*");
            }
        }
    }
}
