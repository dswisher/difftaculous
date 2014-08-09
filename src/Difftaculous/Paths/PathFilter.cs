
using System.Collections.Generic;
using System.Text;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal abstract class PathFilter
    {
        public abstract IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current, bool errorWhenNoMatch);
        public abstract void AddJsonPath(StringBuilder sb);

        protected static ZToken GetTokenIndex(ZToken t, bool errorWhenNoMatch, int index)
        {
            ZArray a = t as ZArray;
            if (a != null)
            {
                if (a.Count <= index)
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonPathException(string.Format("Index {0} outside the bounds of ZArray.", index));
                    }

                    return null;
                }

                return a[index];
            }

            if (errorWhenNoMatch)
            {
                throw new JsonPathException(string.Format("Index {0} not valid on {1}.", index, t.GetType().Name));
            }

            return null;
        }
    }
}
