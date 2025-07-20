
namespace VWLang
{
    public class LangEnvironment
    {
        private Dictionary<string, ASTValue> variables = new Dictionary<string, ASTValue>();
        public void Declare(string name, ASTValue value)
        {
            variables.Add(name, value);
        }
        public void Assign(string name, ASTValue value)
        {
            variables[name] = value;
        }
        public ASTValue Get(string name)
        {
            return variables[name];
        }
    }
}