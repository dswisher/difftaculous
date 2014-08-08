
using System;

namespace Difftaculous.ZModel
{
    public enum TokenType
    {
        Object,
        Property,
        Array,

        [Obsolete]
        Value,

        Integer,

        String,

        Null,
        Undefined
    }
}
