namespace sharplox
{
    public class Token
    {
        // A Token is a lexeme bundled with other data
        // edit: private if not used publicly
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly Object? Literal;
        public readonly int Line;

        public Token(TokenType type, string lexeme, Object? literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        override public string ToString()
        {
            return Type + " " + Lexeme + " " + Literal;
        }
    }

    public enum TokenType
    {
        // Single-character tokens.
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        // One or two character tokens.
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals.
        IDENTIFIER, STRING, NUMBER,

        // Keywords.
        AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
        PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

        EOF
    }
}
