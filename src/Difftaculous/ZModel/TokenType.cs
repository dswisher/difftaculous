
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

        Boolean,
        Integer,
        Float,

        Date,
        TimeSpan,
        Guid,
        Uri,

        String,

        Comment,

        Null,
        Undefined
    }
}
