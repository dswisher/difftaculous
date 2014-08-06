
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


        public static DiffPath FromToken(IToken token)
        {
            return token.Path;
        }



        public string AsJsonPathEx
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

            return AsJsonPathEx == other.AsJsonPathEx;
        }


        public override int GetHashCode()
        {
            return AsJsonPathEx.GetHashCode();
        }


        public override string ToString()
        {
            return AsJsonPathEx;
        }



        internal IEnumerable<ZToken> Evaluate(ZToken t)
        {
            return Evaluate(Filters, t);
        }

        internal static IEnumerable<ZToken> Evaluate(List<PathFilter> filters, ZToken t)
        {
            IEnumerable<ZToken> current = new[] { t };
            foreach (PathFilter filter in filters)
            {
                current = filter.ExecuteFilter(current);
            }

            return current;
        }
    }
}
