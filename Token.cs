namespace VWLang
{
    public enum TokenType
    {
        Reserve = -1,
        Root,
        Int,
        Float,
        LeftParen,
        RightParen,
        Semicolon,
        Name,
        Keyword,
        Comment,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Declaration,
        NewLine,
        Function,
        Comma,
        String,
        Assign,
        LeftCurlyBrace,
        RightCurlyBrace,
        GreaterThan,
        LessThan,
        Equals
    }

    public class Token
    {
        public TokenType name;
        public string? value;

        public Token(TokenType name, string value)
        {
            this.name = name;
            this.value = value;
        }
        public Token(TokenType name)
        {
            this.name = name;
        }
        public override string ToString() => ("T:" + name + ("\tV:" + this?.value));
        public static int GetPrecedence(TokenType tokenType)
        {
            Dictionary<TokenType, int> conv = new Dictionary<TokenType, int>()
            {
                { TokenType.Assign, -1 },
                { TokenType.GreaterThan, 0 },
                { TokenType.LessThan, 0 },
                { TokenType.Equals, 0 },
                { TokenType.Subtraction, 1 },
                { TokenType.Addition, 1 },
                { TokenType.Multiplication, 2 },
                { TokenType.Division, 2 },
            };

            return conv[tokenType];
        }
        public static bool IsLeftAssociative(TokenType tokenType) => tokenType != TokenType.Assign;
        public static bool IsOp(TokenType type) =>
              (type is TokenType.Addition
                    or TokenType.Subtraction
                    or TokenType.Multiplication
                    or TokenType.Division
                    or TokenType.GreaterThan
                    or TokenType.LessThan
                    or TokenType.Equals);
    }

}