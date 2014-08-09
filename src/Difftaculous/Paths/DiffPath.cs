
using System.Collections.Generic;
using System.Text;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    public class DiffPath
    {

        private DiffPath()
        {
            Filters = new List<PathFilter>();
        }


        internal List<PathFilter> Filters { get; private set; }



        public static DiffPath FromJsonPath(string path)
        {
            return new DiffPath
            {
                Filters = JsonPathParser.Parse(path)
            };
        }


        internal static DiffPath FromToken(ZToken token)
        {
            return token.Path;
        }



        public string AsJsonPath
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (var filter in Filters)
                {
                    filter.AddJsonPath(builder);
                }
                return builder.ToString();
            }
        }


        // TODO - remove this?  Where is it used?
        public override bool Equals(object obj)
        {
            DiffPath other = obj as DiffPath;

            if (other == null)
            {
                return false;
            }

            return AsJsonPath == other.AsJsonPath;
        }


        public override int GetHashCode()
        {
            return AsJsonPath.GetHashCode();
        }


        public override string ToString()
        {
            return AsJsonPath;
        }



        internal IEnumerable<ZToken> Evaluate(ZToken t, bool errorWhenNoMatch)
        {
            return Evaluate(Filters, t, errorWhenNoMatch);
        }



        internal static IEnumerable<ZToken> Evaluate(List<PathFilter> filters, ZToken t, bool errorWhenNoMatch)
        {
            IEnumerable<ZToken> current = new[] { t };
            foreach (PathFilter filter in filters)
            {
                current = filter.ExecuteFilter(current, errorWhenNoMatch);
            }

            return current;
        }
    }
}
