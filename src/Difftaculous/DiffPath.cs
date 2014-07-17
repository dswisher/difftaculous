

namespace Difftaculous
{
    public interface IDiffPath
    {
    }



    public class DiffPath : IDiffPath
    {
        private static readonly DiffPath _root = new DiffPath();


        private DiffPath()
        {
        }


        public static DiffPath Root { get { return _root; } }

    }
}
