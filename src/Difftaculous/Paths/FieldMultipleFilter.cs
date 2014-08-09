
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal class FieldMultipleFilter : PathFilter
    {
        public List<string> Names { get; set; }

        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current, bool errorWhenNoMatch)
        {
            foreach (ZToken t in current)
            {
                ZObject o = t as ZObject;
                if (o != null)
                {
                    foreach (string name in Names)
                    {
                        ZToken v = o[name];

                        if (v != null)
                            yield return v;

                        if (errorWhenNoMatch)
                            throw new JsonPathException(string.Format("Property '{0}' does not exist on ZObject.", name));
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                        throw new JsonPathException(string.Format("Properties {0} not valid on {1}.", string.Join(", ", Names.Select(n => "'" + n + "'").ToArray()), t.GetType().Name));
                }
            }
        }



        public override void AddJsonPath(StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }
}
