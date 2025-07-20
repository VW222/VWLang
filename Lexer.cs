using System.Text.RegularExpressions;

namespace VWLang
{
    public class Lexer
    {
        public Regex Split = new Regex(@"\""[^\""]*\""|\d+\.\d*f?d?|\/\/|\+=|-=|==|:=|[+-/*=(){};\n,<>]|\w+");
        public Regex IsInt = new Regex(@"\d+");
        public Regex IsFloat = new Regex(@"\d+(?:\.\d*[fd]?|[fd])");
        public Regex IsOp = new Regex(@"\+=|-=|==|:=|[+-/*=;(){}\n,<>]");
        public Regex IsKeyword = new Regex(@"(let|if|else)");
        public Regex IsString = new Regex(@"\"".*\""");

        private TokenList result = new TokenList();
        public TokenList Lex(string input)
        {
            foreach (var stTok in Split.Matches(input).Cast<Match>().Select(m => m.Value).ToArray())
            {
                result.Add(Determine(stTok));
            }

            return result;
        }
        private Token Determine(string inputDet) // determine token type..
        {
            TokenType type = TokenType.Reserve;

            if (inputDet == "//") type = TokenType.Comment;
            else if (Char.IsDigit(inputDet[0]))
            {
                if (IsFloat.IsMatch(inputDet)) type = TokenType.Float;
                else type = TokenType.Int;
            }
            else if (IsString.IsMatch(inputDet)) type = TokenType.String;
            else if (IsOp.IsMatch(inputDet))
            {
                type = inputDet switch
                {
                    "\n" => TokenType.NewLine,
                    ";" => TokenType.Semicolon,
                    "+" => TokenType.Addition,
                    "-" => TokenType.Subtraction,
                    "*" => TokenType.Multiplication,
                    "/" => TokenType.Division,
                    ":=" => TokenType.Declaration,
                    "(" => TokenType.LeftParen,
                    ")" => TokenType.RightParen,
                    "," => TokenType.Comma,
                    "=" => TokenType.Assign,
                    "{" => TokenType.LeftCurlyBrace,
                    "}" => TokenType.RightCurlyBrace,
                    ">" => TokenType.GreaterThan,
                    "<" => TokenType.LessThan,
                    "==" => TokenType.Equals,
                    _ => throw new Exception("Unknown Operator"),
                };
            }
            else if (IsKeyword.IsMatch(inputDet)) type = TokenType.Keyword;
            else type = TokenType.Name;

            return new Token(type, inputDet);
        }
    }
}