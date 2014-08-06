
using System.Collections.Generic;
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
                        ZToken v = (ZToken)o[Name];

                        if (v != null)
                        {
                            yield return v;
                        }
                    }
                    else
                    {
                        foreach (var p in o.Properties)
                        {
                            yield return (ZToken)p.Value;
                        }
                    }
                }
            }
        }
    }
}
