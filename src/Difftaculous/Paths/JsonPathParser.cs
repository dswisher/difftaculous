
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace Difftaculous.Paths
{
    internal class JsonPathParser
    {
        private readonly string _expression;
        private int _currentIndex;



        public static List<PathFilter> Parse(string jsonPath)
        {
            var parser = new JsonPathParser(jsonPath);

            return parser.Filters;
        }



        private JsonPathParser(string expression)
        {
            _expression = expression;
            Filters = new List<PathFilter>();

            ParseMain();
        }


        public List<PathFilter> Filters { get; private set; }


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
                        if (_currentIndex > currentPartStartIndex)
                        {
                            string member = _expression.Substring(currentPartStartIndex, _currentIndex - currentPartStartIndex);
                            PathFilter filter = scan ? (PathFilter)new ScanFilter { Name = member } : new FieldFilter { Name = member };
                            Filters.Add(filter);
                            scan = false;
                        }

                        Filters.Add(ParseIndexer(currentChar));
                        _currentIndex++;
                        currentPartStartIndex = _currentIndex;
                        followingIndexer = true;
                        followingDot = false;
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
                            PathFilter filter = scan ? (PathFilter)new ScanFilter { Name = member } : new FieldFilter { Name = member };
                            Filters.Add(filter);
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
                PathFilter filter = scan ? (PathFilter)new ScanFilter { Name = member } : new FieldFilter { Name = member };
                Filters.Add(filter);
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



        private PathFilter ParseIndexer(char indexerOpenChar)
        {
            _currentIndex++;

            char indexerCloseChar = (indexerOpenChar == '[') ? ']' : ')';

            EnsureLength("Path ended with open indexer.");

            EatWhitespace();

            if (_expression[_currentIndex] == '\'')
            {
                return ParseQuotedField(indexerCloseChar);
            }
            else if (_expression[_currentIndex] == '?')
            {
                return ParseQuery(indexerCloseChar);
            }
            else
            {
                return ParseArrayIndexer(indexerCloseChar);
            }
        }



        private PathFilter ParseArrayIndexer(char indexerCloseChar)
        {
            int start = _currentIndex;
            int? end = null;
            List<int> indexes = null;
            int colonCount = 0;
            int? startIndex = null;
            int? endIndex = null;
            int? step = null;

            while (_currentIndex < _expression.Length)
            {
                char currentCharacter = _expression[_currentIndex];

                if (currentCharacter == ' ')
                {
                    end = _currentIndex;
                    EatWhitespace();
                    continue;
                }

                if (currentCharacter == indexerCloseChar)
                {
                    int length = (end ?? _currentIndex) - start;

                    if (indexes != null)
                    {
                        if (length == 0)
                        {
                            throw new JsonPathException("Array index expected.");
                        }

                        string indexer = _expression.Substring(start, length);
                        int index = Convert.ToInt32(indexer, CultureInfo.InvariantCulture);

                        indexes.Add(index);
                        return new ArrayMultipleIndexFilter { Indexes = indexes };
                    }
                    else if (colonCount > 0)
                    {
                        if (length > 0)
                        {
                            string indexer = _expression.Substring(start, length);
                            int index = Convert.ToInt32(indexer, CultureInfo.InvariantCulture);

                            if (colonCount == 1)
                                endIndex = index;
                            else
                                step = index;
                        }

                        return new ArraySliceFilter { Start = startIndex, End = endIndex, Step = step };
                    }
                    else
                    {
                        if (length == 0)
                        {
                            throw new JsonPathException("Array index expected.");
                        }

                        string indexer = _expression.Substring(start, length);
                        int index = Convert.ToInt32(indexer, CultureInfo.InvariantCulture);

                        return new ArrayIndexFilter { Index = index };
                    }
                }
                else if (currentCharacter == ',')
                {
                    int length = (end ?? _currentIndex) - start;

                    if (length == 0)
                    {
                        throw new JsonPathException("Array index expected.");
                    }

                    if (indexes == null)
                    {
                        indexes = new List<int>();
                    }

                    string indexer = _expression.Substring(start, length);
                    indexes.Add(Convert.ToInt32(indexer, CultureInfo.InvariantCulture));

                    _currentIndex++;

                    EatWhitespace();

                    start = _currentIndex;
                    end = null;
                }
                else if (currentCharacter == '*')
                {
                    _currentIndex++;
                    EnsureLength("Path ended with open indexer.");
                    EatWhitespace();

                    if (_expression[_currentIndex] != indexerCloseChar)
                    {
                        throw new JsonPathException("Unexpected character while parsing path indexer: " + currentCharacter);
                    }

                    return new ArrayIndexFilter();
                }
                else if (currentCharacter == ':')
                {
                    int length = (end ?? _currentIndex) - start;

                    if (length > 0)
                    {
                        string indexer = _expression.Substring(start, length);
                        int index = Convert.ToInt32(indexer, CultureInfo.InvariantCulture);

                        if (colonCount == 0)
                            startIndex = index;
                        else if (colonCount == 1)
                            endIndex = index;
                        else
                            step = index;
                    }

                    colonCount++;

                    _currentIndex++;

                    EatWhitespace();

                    start = _currentIndex;
                    end = null;
                }
                else if (!char.IsDigit(currentCharacter) && currentCharacter != '-')
                {
                    throw new JsonPathException("Unexpected character while parsing path indexer: " + currentCharacter);
                }
                else
                {
                    if (end != null)
                    {
                        throw new JsonPathException("Unexpected character while parsing path indexer: " + currentCharacter);
                    }

                    _currentIndex++;
                }
            }

            throw new JsonPathException("Path ended with open indexer.");
        }



        private PathFilter ParseQuery(char indexerCloseChar)
        {
#if false
            _currentIndex++;
            EnsureLength("Path ended with open indexer.");

            if (_expression[_currentIndex] != '(')
                throw new JsonException("Unexpected character while parsing path indexer: " + _expression[_currentIndex]);

            _currentIndex++;

            QueryExpression expression = ParseExpression();

            _currentIndex++;
            EnsureLength("Path ended with open indexer.");
            EatWhitespace();

            if (_expression[_currentIndex] != indexerCloseChar)
                throw new JsonException("Unexpected character while parsing path indexer: " + _expression[_currentIndex]);

            return new QueryFilter
            {
                Expression = expression
            };
#endif

            throw new NotImplementedException();
        }



        private PathFilter ParseQuotedField(char indexerCloseChar)
        {
            List<string> fields = null;

            while (_currentIndex < _expression.Length)
            {
                string field = ReadQuotedString();

                EatWhitespace();
                EnsureLength("Path ended with open indexer.");

                if (_expression[_currentIndex] == indexerCloseChar)
                {
                    _currentIndex++;

                    if (fields != null)
                    {
                        fields.Add(field);
                        return new FieldMultipleFilter { Names = fields };
                    }
                    else
                    {
                        return new FieldFilter { Name = field };
                    }
                }
                else if (_expression[_currentIndex] == ',')
                {
                    _currentIndex++;
                    EatWhitespace();

                    if (fields == null)
                        fields = new List<string>();

                    fields.Add(field);
                }
                else
                {
                    throw new JsonPathException("Unexpected character while parsing path indexer: " + _expression[_currentIndex]);
                }
            }

            throw new JsonPathException("Path ended with open indexer.");
        }



        private string ReadQuotedString()
        {
            StringBuilder sb = new StringBuilder();

            _currentIndex++;
            while (_currentIndex < _expression.Length)
            {
                char currentChar = _expression[_currentIndex];
                if (currentChar == '\\' && _currentIndex + 1 < _expression.Length)
                {
                    _currentIndex++;

                    if (_expression[_currentIndex] == '\'')
                        sb.Append('\'');
                    else if (_expression[_currentIndex] == '\\')
                        sb.Append('\\');
                    else
                        throw new JsonPathException(@"Unknown escape chracter: \" + _expression[_currentIndex]);

                    _currentIndex++;
                }
                else if (currentChar == '\'')
                {
                    _currentIndex++;
                    {
                        return sb.ToString();
                    }
                }
                else
                {
                    _currentIndex++;
                    sb.Append(currentChar);
                }
            }

            throw new JsonPathException("Path ended with an open string.");
        }



        private void EnsureLength(string message)
        {
            if (_currentIndex >= _expression.Length)
            {
                throw new JsonPathException(message);
            }
        }
    }
}
