namespace VWLang
{
    public class AST
    {
        public ASTBlock Root = new ASTBlock();

        public void Print()
        {
            Root.Print();
        }
    }
}
