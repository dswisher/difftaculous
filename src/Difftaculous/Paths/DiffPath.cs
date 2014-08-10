
using System.Collections.Generic;
using System.Text;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    /// <summary>
    /// A path to one or more items within the abstract difference model.
    /// </summary>
    public class DiffPath
    {

        private DiffPath()
        {
            Filters = new List<PathFilter>();
        }


        internal List<PathFilter> Filters { get; private set; }



        /// <summary>
        /// Construct a path from a JsonPath string.
        /// </summary>
        /// <param name="path">The JsonPath string.</param>
        /// <returns>A DiffPath for the string.</returns>
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



        /// <summary>
        /// Return a string representation of the DiffPath using JsonPath notation.
        /// </summary>
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



        /// <summary>
        /// Return a string representation of this DiffPath.
        /// </summary>
        /// <returns>The string representation.</returns>
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
