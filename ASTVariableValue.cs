namespace VWLang
{
    public class ASTVariableValue : ASTValue
    {
        public override Type type => Type.Variable;
        public override object value { get; }

        public ASTVariableValue(string value)
        {
            this.value = value;
        }
    }
}
