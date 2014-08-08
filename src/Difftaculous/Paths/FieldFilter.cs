
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Difftaculous.ZModel;

namespace Difftaculous.Paths
{
    internal class FieldFilter : PathFilter
    {
        public string Name { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current)
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
                    }
                    else
                    {
                        foreach (KeyValuePair<string, ZToken> p in o)
                        {
                            yield return p.Value;
                        }
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
