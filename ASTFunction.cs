namespace VWLang
{
    public class ASTFunction : ASTExpression
    {
        public string name;

        public override string ToString() => name;
    }
}
