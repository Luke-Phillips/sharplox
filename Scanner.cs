namespace sharplox
{
    public class Scanner
    {
        private static readonly IDictionary<string, TokenType> _keywords;

        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();

        private int _start = 0; // first char in lexeme
        private int _current = 0; // current char in lexeme
        private int _line = 1; // source line

        static Scanner()
        {
            _keywords = new Dictionary<string, TokenType>();
            _keywords["and"] = TokenType.AND;
            _keywords["class"] = TokenType.CLASS;
            _keywords["else"] = TokenType.ELSE;
            _keywords["false"] = TokenType.FALSE;
            _keywords["for"] = TokenType.FOR;
            _keywords["fun"] = TokenType.FUN;
            _keywords["if"] = TokenType.IF;
            _keywords["nil"] = TokenType.NIL;
            _keywords["or"] = TokenType.OR;
            _keywords["print"] = TokenType.PRINT;
            _keywords["return"] = TokenType.RETURN;
            _keywords["super"] = TokenType.SUPER;
            _keywords["this"] = TokenType.THIS;
            _keywords["true"] = TokenType.TRUE;
            _keywords["var"] = TokenType.VAR;
            _keywords["while"] = TokenType.WHILE;
        }

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL :TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Peek() == '/')
                    {
                        Advance();
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else if (Match('*'))
                    {
                        // A block comment goes until */
                        while ((Peek() != '*' || PeekNext() != '/') && !IsAtEnd())
                        {
                            if (Peek() == '\n') ++_line;
                            Advance();
                        }
                        Advance(); Advance();

                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;
                case '\n':
                    _line++;
                    break;

                case '"': StringLiteral(); break;

                default:
                    if (IsDigit(c))
                    {
                        NumberLiteral();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(_line, "Unexpected character.");
                    }
                    
                    break;
            }
        }
        private bool IsAtEnd()
        {
            //return _current < _source.Length; // intellisensed wtf
            return _current >= _source.Length;
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }
        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, Object? literal)
        {
            string text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private void StringLiteral()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            string value = _source.Substring(_start + 1, _current - 1); // if we supported escape seqs we'd do that here
            AddToken(TokenType.STRING, value);
        }

        private void NumberLiteral()
        {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, Double.Parse(_source.Substring(_start, _current - _start)));
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = _source.Substring(_start, _current - _start);
            TokenType type;
            if (!_keywords.TryGetValue(text, out type)) type = TokenType.IDENTIFIER;
            AddToken((TokenType) type);
        }
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'A' && c <= 'Z') ||
                   (c >= 'a' && c <= 'z') ||
                    c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}
