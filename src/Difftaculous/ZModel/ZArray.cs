
using System.Collections;
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


        public int IndexOf(IToken item)
        {
            // TODO - do we need an equality comparer?
            return _tokens.IndexOf(item);
        }


        public void Add(IToken token)
        {
            _tokens.Add(token);
            ((ZToken)token).Parent = this;
        }

        public IEnumerator<IToken> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
