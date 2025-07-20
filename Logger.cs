namespace VWLang
{
    public class Logger
    {
        public static void Error(Object obj, string message = "Unknown Error", bool doQuit = true) // graceful
        {
            if (obj is Lexer)
            {
                FancyPrint("Lexer Error\n" + message);
            }
            else if (obj is Parser)
            {
                FancyPrint("Parser Error\n" + message);
            }
            else
            {
                 FancyPrint(message);
            }

            if (doQuit)
            {
                Environment.Exit(-1);
            }
        }
        private static void FancyPrint(string message, bool centered = true)
        {
            Console.WriteLine(new string('-', Console.WindowWidth));
            foreach (var v in message.Split('\n'))
            {
                Console.WriteLine(Pad(v) + v);
            }
            Console.WriteLine(new string('-', Console.WindowWidth));
        }
        private static string Pad(string text)
        {
            return new string(' ', Console.WindowWidth / 2 - text.Length / 2);
        }
    }
}