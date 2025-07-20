namespace VWLang
{
    public class ASTIntValue : ASTValue
    {
        public override Type type => Type.Int;
        public override object value { get; }

        public ASTIntValue(int value)
        {
            this.value = value;
        }

        public ASTIntValue(bool value)
        {
            this.value = value ? 1 : 0;
        }
    }
}
