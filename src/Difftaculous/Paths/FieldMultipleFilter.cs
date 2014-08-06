
using System;
using System.Collections.Generic;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal class FieldMultipleFilter : PathFilter
    {
        public List<string> Names { get; set; }

        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current)
        {
#if false
            foreach (JToken t in current)
            {
                JObject o = t as JObject;
                if (o != null)
                {
                    foreach (string name in Names)
                    {
                        JToken v = o[name];

                        if (v != null)
                            yield return v;

                        if (errorWhenNoMatch)
                            throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, name));
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                        throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", Names.Select(n => "'" + n + "'").ToArray()), t.GetType().Name));
                }
            }
#endif

            throw new NotImplementedException();
        }



        public override string AsJsonPath
        {
            get { throw new NotImplementedException(); }
        }
    }
}
