

using System;
using System.Collections;

namespace Difftaculous.ZModel
{
    internal class ZProperty : ZToken, IProperty
    {
        public ZProperty(string name, object content)
        {
            Name = name;

            if (IsMultiContent(content))
            {
                throw new NotImplementedException("Array construction is TBD.");
            }

            Value = CreateFromContent(content);

            //Value = IsMultiContent(content)
            //    ? new ZArray(content)
            //    : CreateFromContent(content);

            ((ZToken)Value).Parent = this;
        }

        public override TokenType Type { get { return TokenType.Property; } }

        public string Name { get; private set; }
        public IToken Value { get; private set; }



        internal ZToken CreateFromContent(object content)
        {
            // NOTE: In Json.Net, this is in JContainer
            if (content is ZToken)
            {
                return (ZToken)content;
            }

            return new ZValue(content);
        }


        internal bool IsMultiContent(object content)
        {
            // NOTE: In Json.Net, this is in JContainer
            return (content is IEnumerable && !(content is string) && !(content is ZToken) && !(content is byte[]));
        }
    }
}
