using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Difftaculous.Paths;

namespace Difftaculous.Adapters
{
    internal class ZObject : IObject
    {
        public DiffPath Path
        {
            // TODO - implement path - need to navigate up to parents
            get { throw new NotImplementedException(); }
        }
    }
}
