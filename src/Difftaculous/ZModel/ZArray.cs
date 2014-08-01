
using System.Collections.Generic;


namespace Difftaculous.ZModel
{
    internal class ZArray : ZToken, IArray
    {
        private readonly List<IToken> _tokens = new List<IToken>();

        public override TokenType Type { get { return TokenType.Array; } }

        public int Count { get { return _tokens.Count; } }

        public IToken this[int index]
        {
            get { return _tokens[index]; }
        }


        public void Add(IToken token)
        {
            _tokens.Add(token);
        }
    }
}
