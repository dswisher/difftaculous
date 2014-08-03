using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Difftaculous.Paths
{
    internal class PathExpression
    {
        private readonly List<PathTerm> _terms = new List<PathTerm>();

        public IList<PathTerm> Terms { get { return _terms; } }
    }
}
