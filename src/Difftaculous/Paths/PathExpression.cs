
using System.Collections.Generic;


namespace Difftaculous.Paths
{
    internal class PathExpression
    {
        private readonly List<PathFilter> _terms = new List<PathFilter>();

        public IList<PathFilter> Terms { get { return _terms; } }
    }
}
