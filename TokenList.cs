namespace VWLang
{
    public class TokenList
    {
        public List<Token> TList = [];
        public void Add(Token token)
        {
            TList.Add(token);
        }
        public TokenList(List<Token> tList)
        {
            TList = tList;
        }

        public TokenList()
        {
        }

        public void Print()
        {
            foreach (Token token in TList)
            {
                Console.WriteLine(token);
            }
        }
        public void RepRemove(int at, int count)
        {
            for (int i = 0; i < count; i++)
            {
                TList.RemoveAt(at);
            }
        }
    }
}
