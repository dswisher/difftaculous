

namespace Difftaculous.ZModel
{
    internal enum TokenType
    {
        Object,
        Property,
        Array,

        Boolean,
        Integer,
        Float,

        Date,
        TimeSpan,
        Guid,
        Uri,

        String,

        Raw,
        Bytes,
        Comment,

        Null,
        Undefined
    }
}
