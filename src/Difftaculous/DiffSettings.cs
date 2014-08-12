
using System.Collections.Generic;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Paths;


namespace Difftaculous
{
    /// <summary>
    /// Settings that alter or enhance the behavior of the diff process.
    /// </summary>
    public class DiffSettings
    {

        /// <summary>
        /// Define settings for an array
        /// </summary>
        /// <param name="path">The path that defines the array</param>
        /// <returns>A settings object</returns>
        public static DiffSettings Array(DiffPath path)
        {
            return new DiffSettings(path);
        }


        /// <summary>
        /// Define settings for an item
        /// </summary>
        /// <param name="path">The path that defines the item</param>
        /// <returns>A settings object</returns>
        public static DiffSettings Item(DiffPath path)
        {
            return new DiffSettings(path);
        }


        private readonly List<IHint> _hints = new List<IHint>();
        private readonly List<ICaveat> _caveats = new 
            List<ICaveat>();

        private DiffPath _currentPath;

        internal DiffSettings()
        {
        }


        internal DiffSettings(DiffPath path)
        {
            _currentPath = path;
        }


        /// <summary>
        /// For the current array, set the diff mode to keyed and define the key.
        /// </summary>
        /// <param name="name">The key</param>
        /// <returns>The settings</returns>
        public DiffSettings KeyedBy(string name)
        {
            _hints.Add(new ArrayDiffHint(_currentPath, name));

            return this;
        }


        /// <summary>
        /// For the current item, define the amount by which it can vary and still be considered the same
        /// </summary>
        /// <param name="amount">The amount of allowed variance</param>
        /// <returns>The settings</returns>
        public DiffSettings CanVaryBy(double amount)
        {
            _caveats.Add(new VarianceCaveat(_currentPath, amount));

            return this;
        }


        internal IEnumerable<IHint> Hints { get { return _hints; } }
        internal IEnumerable<ICaveat> Caveats { get { return _caveats; } }
    }
}
