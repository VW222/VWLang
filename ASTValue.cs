namespace VWLang
{
    public abstract class ASTValue : ASTExpression
    {
        public enum Type
        {
            Int = 1,
            Float,
            String,
            Variable,
            Custom // future structs
        }

        public abstract Type type { get; }
        public abstract Object value { get;  }

        public override string ToString() => type.ToString() + " " + value;
        public static Type GetType(TokenType tokenType)
        {
            Dictionary<TokenType, Type> conv = new Dictionary<TokenType, Type> {
                { TokenType.Int, Type.Int },
                { TokenType.Float, Type.Float },
                { TokenType.Name, Type.Variable },
                { TokenType.String, Type.String }
            };
            return conv[tokenType];
        }
    }
}
