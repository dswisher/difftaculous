
using System;
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
        private readonly List<IHint> _hints = new List<IHint>();
        private readonly List<ICaveat> _caveats = new List<ICaveat>();



        /// <summary>
        /// For the current array, set the diff mode to keyed and define the key.
        /// </summary>
        /// <param name="path">The path of the array</param>
        /// <param name="name">The key</param>
        /// <returns>The settings</returns>
        public DiffSettings KeyedBy(DiffPath path, string name)
        {
            _hints.Add(new ArrayDiffHint(path, name));

            return this;
        }


        /// <summary>
        /// For the current item, define the amount by which it can vary and still be considered the same
        /// </summary>
        /// <param name="path">The path of the item</param>
        /// <param name="amount">The amount of allowed variance</param>
        /// <returns>The settings</returns>
        public DiffSettings CanVaryBy(DiffPath path, double amount)
        {
            _caveats.Add(new VarianceCaveat(path, amount));

            return this;
        }


        internal IEnumerable<IHint> Hints { get { return _hints; } }
        internal IEnumerable<ICaveat> Caveats { get { return _caveats; } }
    }
}
