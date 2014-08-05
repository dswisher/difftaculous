
using System.Collections.Generic;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal abstract class PathFilter
    {
        public abstract IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current);
    }
}
