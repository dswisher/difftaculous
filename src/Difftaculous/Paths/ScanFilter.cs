

using System;
using System.Collections.Generic;
using Difftaculous.ZModel;

namespace Difftaculous.Paths
{
    internal class ScanFilter : PathFilter
    {
        public string Name { get; set; }

        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current)
        {
#if false
            foreach (JToken root in current)
            {
                if (Name == null)
                    yield return root;

                JToken value = root;
                JToken container = root;

                while (true)
                {
                    if (container != null && container.HasValues)
                    {
                        value = container.First;
                    }
                    else
                    {
                        while (value != null && value != root && value == value.Parent.Last)
                        {
                            value = value.Parent;
                        }

                        if (value == null || value == root)
                            break;

                        value = value.Next;
                    }

                    JProperty e = value as JProperty;
                    if (e != null)
                    {
                        if (e.Name == Name)
                            yield return e.Value;
                    }
                    else
                    {
                        if (Name == null)
                            yield return value;
                    }

                    container = value as JContainer;
                }
            }
#endif

            throw new NotImplementedException();
        }
    }
}
