

using System;
using System.Collections.Generic;
using Difftaculous.ZModel;

namespace Difftaculous.Paths
{
    internal class ArrayIndexFilter : PathFilter
    {
        public int? Index { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current)
        {
            foreach (ZToken t in current)
            {
                if (Index != null)
                {
                    ZToken v = GetTokenIndex(t, Index.Value);

                    if (v != null)
                    {
                        yield return v;
                    }
                }
                else
                {
#if false
                    if (t is JArray || t is JConstructor)
                    {
                        foreach (JToken v in t)
                        {
                            yield return v;
                        }
                    }
                    else
                    {
                        if (errorWhenNoMatch)
                            throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
                    }
#endif
                    throw new NotImplementedException();
                }
            }
        }



        public override string AsJsonPath
        {
            get
            {
                return Index.HasValue ? string.Format("[{0}]", Index.Value) : "[*]";
            }
        }
    }
}
