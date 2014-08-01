
using System;
using System.Collections.Generic;
using Difftaculous.Paths;


namespace Difftaculous.Adapters
{
    internal class ZArray : IArray
    {
        private readonly List<IToken> _tokens = new List<IToken>();


        public DiffPath Path { get { throw new NotImplementedException(); } }

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
