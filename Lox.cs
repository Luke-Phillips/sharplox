namespace sharplox
{
    public static class Lox
    {
        static bool HadError = false;

        public static void Run(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Scanning file");
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            var sourceReader = new StreamReader(path);
            string source = sourceReader.ReadToEnd();
            RunSource(source);
            if (HadError) Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            string? sourceLine;
            while (true)
            {
                Console.WriteLine("> ");
                sourceLine = Console.ReadLine();
                if (sourceLine == null) break;
                RunSource(sourceLine);
                HadError = false;
            }
        }

        private static void RunSource(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            
            // debug
            // foreach (Token token in tokens)
            // {
            //     Console.WriteLine(
            //         token.Type.ToString() + " " + 
            //         token.Lexeme + " " + 
            //         token.Literal + " " + 
            //         token.Line
            //     );
            // }

            // For now, just print the tokens.
            /* for (Token token : tokens)
             {
                 System.out.println(token);
             }*/
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine(
                "[line " + line + "] Error" + where + ": " + message);
            HadError = true;
        }
    }
}
