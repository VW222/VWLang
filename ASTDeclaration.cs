namespace VWLang
{
    public class ASTDeclaration : ASTExpression
    {
        public string Type;
        public string Name;
        public ASTExpression Expression;
        public override string ToString() => $"Declare {Name} to be {Type} with value:";
        public override void Print(int depth)
        {
            Console.WriteLine(new String('\t', depth) + ToString());
            Expression.Print(depth + 1);
        }
    }
}