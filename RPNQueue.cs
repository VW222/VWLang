namespace VWLang
{
    public class RPNQueue
    {
        Queue<Token> body = new Queue<Token>();
        public int Count { get { return body.Count; } }
        public void Enqueue(Token token)
        {
            body.Enqueue(token);
        }
        public Token Dequeue()
        {
            return body.Dequeue();
        }

        public void Print()
        {
            foreach (Token token in body)
            {
                Console.WriteLine(token);
            }
        }
    }
}
