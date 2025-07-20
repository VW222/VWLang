namespace VWLang
{
    public class ASTBlock : ASTExpression
    {
        // children are executable expressions
        public override string ToString() => "Block";
    }
}
