
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    public interface IDiffPath
    {
        // TODO - obsolete
        bool Matches(IToken token);

        string AsJsonPath { get; }
    }



    public class DiffPath : IDiffPath
    {
        private static readonly DiffPath _root = FromJsonPath("$");


        private DiffPath()
        {
        }


        // TODO - this may be obsolete once new IToken-based DiffEngine is done
        internal static DiffPath Root { get { return _root; } }


        public static DiffPath FromJsonPath(string path)
        {
            return new DiffPath { AsJsonPath = path };
        }


        public static DiffPath FromToken(IToken token)
        {
            return token.Path;
        }



        public string AsJsonPath { get; private set; }


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



        // TODO - Get rid of matches - should use SelectTokens instead!
        public bool Matches(IToken token)
        {
            // TODO - implement this the right way!

            return AsJsonPath.Equals(token.Path.AsJsonPath);
        }
    }
}
