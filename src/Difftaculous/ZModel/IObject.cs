
using System.Collections.Generic;


namespace Difftaculous.ZModel
{
    public interface IObject : IToken
    {
        IEnumerable<IProperty> Properties { get; }
    }
}
