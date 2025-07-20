namespace VWLang
{
    public class ASTAssignation : ASTExpression
    {
        public string Name;
        public ASTExpression Expression;
        public override string ToString() => $"Assign variable {Name} to be:";

        public override void Print(int depth)
        {
            Console.WriteLine(new String('\t', depth) + ToString());
            Expression.Print(depth + 1);
        }
    }
}