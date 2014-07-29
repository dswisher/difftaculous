

namespace Difftaculous.Paths
{
    public interface IDiffPath
    {
        IDiffPath Extend(string name);

        IDiffPath ArrayExtend(string key);
        IDiffPath ArrayExtend(int index);

        bool Matches(IDiffPath path);

        string AsJsonPath { get; }
    }



    public class DiffPath : IDiffPath
    {
        private static readonly DiffPath _root = FromJsonPath("$");


        private DiffPath()
        {
        }


        public static DiffPath Root { get { return _root; } }


        public static DiffPath FromJsonPath(string path)
        {
            return new DiffPath { AsJsonPath = path };
        }



        // TODO - we need a MUCH better way of indicating what is being extended
        public IDiffPath Extend(string name)
        {
            return FromJsonPath(AsJsonPath + "." + name);
        }


        public IDiffPath ArrayExtend(int index)
        {
            return FromJsonPath(AsJsonPath + "[" + index + "]");
        }


        public IDiffPath ArrayExtend(string key)
        {
            return FromJsonPath(AsJsonPath + "['" + key + "']");
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


        public bool Matches(IDiffPath path)
        {
            // TODO - this is way too simplistic!  Need to do wildcard matches and
            // whatnot...matching is NOT the same as equality...

            return Equals(path);
        }
    }
}
