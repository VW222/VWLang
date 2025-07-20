namespace VWLang
{
    public abstract class ASTNode
    {
        public List<ASTNode> Children { get; private set; } = [];

        public void AddChild(ASTNode node)
        {
            Children.Add(node);
        }
        public virtual void Print(int depth = 0)
        {
            Console.WriteLine(new String('\t', depth) + ToString());
            foreach (var child in Children)
            {
                child.Print(depth + 1);
            }
        }
        public void ReverseChildren()
        {
            Children.Reverse();
        }
    }
}