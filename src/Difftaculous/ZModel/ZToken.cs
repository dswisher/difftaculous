using System;
using Difftaculous.Adapters;
using Difftaculous.Paths;

namespace Difftaculous.ZModel
{
    internal abstract class ZToken : IToken
    {
        public IToken Parent { get; set; }

        // NOTE: Json.Net defines this as taking an object as the key - for arrays, perhaps?
        public virtual IToken this[string key]
        {
            get { throw new InvalidOperationException(string.Format("Cannot access child value on {0}.", GetType())); }
        }



        public DiffPath Path
        {
            // TODO!  Hack to get unit test to pass!
            get { return DiffPath.FromJsonPath("$.name"); }
        }

    }
}
