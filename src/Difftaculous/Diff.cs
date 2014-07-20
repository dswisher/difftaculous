
using System.Collections.Generic;
using Difftaculous.Adapters;
using Difftaculous.Caveats;


namespace Difftaculous
{
    public static class Diff
    {

        // TODO - this should take some sort of non-JSON-specific adapter interface
        public static IDiffResult Compare(JsonAdapter a, JsonAdapter b, IEnumerable<ICaveat> caveats = null)
        {
            DiffEngine engine = new DiffEngine(caveats);

            return engine.Diff(a.Content, b.Content, DiffPath.Root);
        }

    }
}
