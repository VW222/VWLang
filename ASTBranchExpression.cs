namespace VWLang
{
    public class ASTBranchExpression : ASTExpression
    {
        public ASTExpression Condition;
        public ASTExpression TrueBranch;
        public ASTExpression? ElseBranch;
        public override string ToString() => "If statement";
        public override void Print(int depth = 0)
        {
            Console.WriteLine(new String('\t', depth) + ToString());
            Condition.Print(depth + 1);
            TrueBranch.Print(depth + 1);
            ElseBranch?.Print(depth + 1);
        }
    }
}