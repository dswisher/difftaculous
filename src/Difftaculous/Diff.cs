
using System.Collections.Generic;

using Difftaculous.Adapters;
using Difftaculous.Caveats;
using Difftaculous.Hints;
using Difftaculous.Paths;
using Difftaculous.Results;


namespace Difftaculous
{
    public static class Diff
    {

        // TODO - remove the JsonAdapter-based overloads
        public static IDiffResult Compare(JsonAdapter a, JsonAdapter b, IEnumerable<ICaveat> caveats, IEnumerable<IHint> hints)
        {
            DiffEngine engine = new DiffEngine(caveats, hints);

            return engine.Diff(a.Content, b.Content, DiffPath.Root);
        }



        public static IDiffResult Compare(JsonAdapter a, JsonAdapter b, IEnumerable<ICaveat> caveats)
        {
            return Compare(a, b, caveats, null);
        }



        public static IDiffResult Compare(JsonAdapter a, JsonAdapter b, IEnumerable<IHint> hints)
        {
            return Compare(a, b, null, hints);
        }



        public static IDiffResult Compare(JsonAdapter a, JsonAdapter b)
        {
            return Compare(a, b, null, null);
        }



        public static IDiffResult Compare(IAdapter a, IAdapter b)
        {
            // TODO - accept hints and caveats!
            DiffEngine engine = new DiffEngine(null, null);

            return engine.Diff(a.Content, b.Content);
        }
    }
}
