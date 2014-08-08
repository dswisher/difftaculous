
using System.Collections;
using System.Collections.Generic;


namespace Difftaculous.ZModel
{
    internal class ZArray : ZContainer, IEnumerable<ZToken> // , IList<JToken>
    {
        private readonly List<ZToken> _tokens = new List<ZToken>();


        public ZArray()
        {
        }


        public ZArray(params object[] content)
        {
            foreach (var item in content)
            {
                Add(new ZValue(item));
            }
        }



        public override TokenType Type { get { return TokenType.Array; } }

        public int Count { get { return _tokens.Count; } }


        public ZToken this[int index]
        {
            get { return _tokens[index]; }
        }


        public int IndexOf(ZToken item)
        {
            // TODO - do we need an equality comparer?
            return _tokens.IndexOf(item);
        }


        public void Add(ZToken token)
        {
            _tokens.Add(token);
            token.Parent = this;
        }

        public IEnumerator<ZToken> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
