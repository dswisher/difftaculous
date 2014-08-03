using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Difftaculous.Paths
{
    internal class JsonPathParser
    {
        private readonly string _expression;
        private readonly PathExpression _pathExpression = new PathExpression();
        private int _currentIndex;


        public JsonPathParser(string expression)
        {
            _expression = expression;

            ParseMain();
        }


        public PathExpression Expression { get { return _pathExpression; } }


        private void ParseMain()
        {
            int currentPartStartIndex = _currentIndex;

            EatWhitespace();

            if (_expression.Length == _currentIndex)
            {
                return;
            }

            if (_expression[_currentIndex] == '$')
            {
                if (_expression.Length == 1)
                {
                    return;
                }

                // only increment position for "$." or "$["
                // otherwise assume property that starts with $
                char c = _expression[_currentIndex + 1];
                if (c == '.' || c == '[')
                {
                    _currentIndex++;
                    currentPartStartIndex = _currentIndex;
                }
            }

            if (!ParsePath(currentPartStartIndex, false))
            {
                int lastCharacterIndex = _currentIndex;

                EatWhitespace();

                if (_currentIndex < _expression.Length)
                {
                    throw new JsonPathException("Unexpected character while parsing path: " + _expression[lastCharacterIndex]);                
                }
            }
        }



        private bool ParsePath(int currentPartStartIndex, bool query)
        {
            bool scan = false;
            bool followingIndexer = false;
            bool followingDot = false;

            bool ended = false;
            while (_currentIndex < _expression.Length && !ended)
            {
                char currentChar = _expression[_currentIndex];

                switch (currentChar)
                {
                    case '[':
                    case '(':
                        // TODO

#if false
                        if (_currentIndex > currentPartStartIndex)
                        {
                            string member = _expression.Substring(currentPartStartIndex, _currentIndex - currentPartStartIndex);
                            PathFilter filter = (scan) ? (PathFilter)new ScanFilter() { Name = member } : new FieldFilter() { Name = member };
                            filters.Add(filter);
                            scan = false;
                        }

                        filters.Add(ParseIndexer(currentChar));
                        _currentIndex++;
                        currentPartStartIndex = _currentIndex;
                        followingIndexer = true;
                        followingDot = false;
#endif
                        break;

                    case ']':
                    case ')':
                        ended = true;
                        break;

                    case ' ':
                        if (_currentIndex < _expression.Length)
                            ended = true;
                        break;

                    case '.':
                        if (_currentIndex > currentPartStartIndex)
                        {
                            string member = _expression.Substring(currentPartStartIndex, _currentIndex - currentPartStartIndex);
                            if (member == "*")
                            {
                                member = null;
                            }
                            // TODO!
                            Console.WriteLine("PathFilter or ScanFilter");
                            //PathFilter filter = (scan) ? (PathFilter)new ScanFilter() { Name = member } : new FieldFilter() { Name = member };
                            //filters.Add(filter);
                            scan = false;
                        }
                        if (_currentIndex + 1 < _expression.Length && _expression[_currentIndex + 1] == '.')
                        {
                            scan = true;
                            _currentIndex++;
                        }
                        _currentIndex++;
                        currentPartStartIndex = _currentIndex;
                        followingIndexer = false;
                        followingDot = true;
                        break;

                    default:
                        if (query && (currentChar == '=' || currentChar == '<' || currentChar == '!' || currentChar == '>' || currentChar == '|' || currentChar == '&'))
                        {
                            ended = true;
                        }
                        else
                        {
                            if (followingIndexer)
                            {
                                throw new JsonPathException("Unexpected character following indexer: " + currentChar);
                            }

                            _currentIndex++;
                        }
                        break;

                }
            }

            bool atPathEnd = (_currentIndex == _expression.Length);

            if (_currentIndex > currentPartStartIndex)
            {
                string member = _expression.Substring(currentPartStartIndex, _currentIndex - currentPartStartIndex).TrimEnd();
                if (member == "*")
                {
                    member = null;
                }
                if (scan)
                {
                    //PathFilter filter = (scan) ? (PathFilter)new ScanFilter() { Name = member } : new FieldFilter() { Name = member };
                    // filters.Add(filter);
                    Console.WriteLine("PathFilter");
                }
                else
                {
                    var term = new FieldTerm() { Name = member };
                    _pathExpression.Terms.Add(term);
                }
            }
            else
            {
                // no field name following dot in path and at end of base path/query
                if (followingDot && (atPathEnd || query))
                {
                    throw new JsonPathException("Unexpected end while parsing path.");
                }
            }

            return atPathEnd;
        }


        private void EatWhitespace()
        {
            while (_currentIndex < _expression.Length)
            {
                if (_expression[_currentIndex] != ' ')
                    break;

                _currentIndex++;
            }
        }

    }
}
