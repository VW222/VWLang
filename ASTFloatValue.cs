namespace VWLang
{
    public class ASTFloatValue : ASTValue
    {
        public override Type type => Type.Float;
        public override object value { get; }

        public ASTFloatValue(float value)
        {
            this.value = value;
        }
    }

}