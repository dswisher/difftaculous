
using System.Collections.Generic;


namespace Difftaculous.Adapters
{
    public interface IObject : IToken
    {
        IEnumerable<IProperty> Properties { get; }
    }
}
