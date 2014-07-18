
using Difftaculous.Adapters;


namespace Difftaculous
{
    public static class Diff
    {

        // TODO - this should take some sort of non-JSON-specific adapter interface
        public static IDiffResult Compare(JsonAdapter a, JsonAdapter b)
        {
            DiffEngine engine = new DiffEngine();

            return engine.Diff(a.Content, b.Content, DiffPath.Root);
        }

    }
}
