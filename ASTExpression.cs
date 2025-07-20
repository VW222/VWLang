namespace VWLang
{
    public abstract class ASTExpression : ASTNode
    {
        // doesnt really need anything, does it? the purpose of this class is purely organization
        // its so much nicer if every expression inherits from this
        public override string ToString() => "Expression";
    }
}
