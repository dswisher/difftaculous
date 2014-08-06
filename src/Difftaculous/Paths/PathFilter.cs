
using System.Collections.Generic;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal abstract class PathFilter
    {
        public abstract IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current);
        public abstract string AsJsonPath { get; }

        protected static ZToken GetTokenIndex(ZToken t, int index)
        {
            ZArray a = t as ZArray;
            if (a != null)
            {
                if (a.Count <= index)
                {
                    return null;
                }

                return (ZToken)(a[index]);
            }
            else
            {
                return null;
            }

#if false
            JArray a = t as JArray;
            JConstructor c = t as JConstructor;

            if (a != null)
            {
                if (a.Count <= index)
                {
                    if (errorWhenNoMatch)
                        throw new JsonException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, index));

                    return null;
                }

                return a[index];
            }
            else if (c != null)
            {
                if (c.Count <= index)
                {
                    if (errorWhenNoMatch)
                        throw new JsonException("Index {0} outside the bounds of JConstructor.".FormatWith(CultureInfo.InvariantCulture, index));

                    return null;
                }

                return c[index];
            }
            else
            {
                if (errorWhenNoMatch)
                    throw new JsonException("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, index, t.GetType().Name));

                return null;
            }
#endif
        }
    }
}
