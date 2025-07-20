namespace VWLang
{
    public class ASTMathOp : ASTExpression
    {
        public enum Operation
        {
            Nop = 0,
            Add = 1,
            Subtract,
            Multiply,
            Divide,
            GreaterThan,
            LessThan,
            Equals
        }
        public Operation operation;
        public ASTMathOp(Token t)
        {
            operation = t.name switch
            {
                TokenType.Addition => Operation.Add,
                TokenType.Subtraction => Operation.Subtract,
                TokenType.Multiplication => Operation.Multiply,
                TokenType.Division => Operation.Divide,
                TokenType.GreaterThan => Operation.GreaterThan,
                TokenType.LessThan => Operation.LessThan,
                TokenType.Equals => Operation.Equals,
                _ => Operation.Nop,
            };
        }
        public override string ToString()
        {
            return operation.ToString();
        }
    }
}
