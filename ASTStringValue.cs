namespace VWLang
{

    public class ASTStringValue : ASTValue
    {
        public override Type type => Type.String;
        public override object value { get; }

        public ASTStringValue(string value)
        {
            this.value = value;
        }
    }
}