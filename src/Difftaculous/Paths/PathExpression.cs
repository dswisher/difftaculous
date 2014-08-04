
using System.Collections.Generic;


namespace Difftaculous.Paths
{
    internal class PathExpression
    {
        private readonly List<PathTerm> _terms = new List<PathTerm>();

        public IList<PathTerm> Terms { get { return _terms; } }
    }
}
