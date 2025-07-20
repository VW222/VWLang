namespace VWLang
{
    public class Language
    {
        public required Lexer lexer;
        public required Parser parser;
        public required Runner runner;

        public AST? parseOut;
        public void Parse(string input)
        {
            parseOut = parser.Parse(lexer.Lex(input));
        }
        public void Run(string input)
        {
            Parse(input);
            Run();
        }
        public void Run()
        {
            runner.Run(parseOut);
        }
    }
}
