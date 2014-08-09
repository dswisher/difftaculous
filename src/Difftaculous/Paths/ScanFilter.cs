

using System;
using System.Collections.Generic;
using System.Text;
using Difftaculous.ZModel;

namespace Difftaculous.Paths
{
    internal class ScanFilter : PathFilter
    {
        public string Name { get; set; }

        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current)
        {
            foreach (ZToken root in current)
            {
                if (Name == null)
                    yield return root;

                ZToken value = root;
                ZToken container = root;

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

                    ZProperty e = value as ZProperty;

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

                    container = value as ZContainer;
                }
            }
        }



        public override void AddJsonPath(StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }
}
