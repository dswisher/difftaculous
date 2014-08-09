
using System.Collections.Generic;
using System.Text;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal class ArrayIndexFilter : PathFilter
    {
        public int? Index { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current, bool errorWhenNoMatch)
        {
            foreach (ZToken t in current)
            {
                if (Index != null)
                {
                    ZToken v = GetTokenIndex(t, errorWhenNoMatch, Index.Value);

                    if (v != null)
                    {
                        yield return v;
                    }
                }
                else
                {
                    if (t is ZArray)
                    {
                        foreach (ZToken v in (ZArray)t)
                        {
                            yield return v;
                        }
                    }
                    else
                    {
                        if (errorWhenNoMatch)
                        {
                            throw new JsonPathException(string.Format("Index * not valid on {0}.", t.GetType().Name));
                        }
                    }
                }
            }
        }



        public override void AddJsonPath(StringBuilder sb)
        {
            sb.Append(Index.HasValue ? string.Format("[{0}]", Index.Value) : "[*]");
        }
    }
}
