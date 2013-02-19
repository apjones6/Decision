using System;

namespace Decision
{
    class Program
    {
        static DecisionProvider decisionProvider;

        static void Main(string[] args)
        {
            decisionProvider = new DecisionProvider("Decisions.xml");

            // Execute
            Do(new DecisionContext("X", "Alpha Beta"));
            Do(new DecisionContext("X", "Alpha"));
            Do(new DecisionContext("X", "Alpha Gamma"));
            Do(new DecisionContext("X", "Alpha Beta Gamma"));
            Do(new DecisionContext("X", "Beta Gamma"));
            Console.WriteLine();

            Do(new DecisionContext("Y", "Alpha Beta"));
            Do(new DecisionContext("Y", "Alpha Beta Gamma"));
            Do(new DecisionContext("Y", "Alpha Gamma"));
            Do(new DecisionContext("Y", "Beta Gamma"));
            Do(new DecisionContext("Y", "Alpha"));
            Console.WriteLine();

            Do(new DecisionContext("Z", "Alpha"));
            Do(new DecisionContext("Z", "Beta"));
            Do(new DecisionContext("Z", "Gamma"));
            Console.WriteLine();

            // Wait
            Console.ReadKey();
        }

        static void Do(DecisionContext context)
        {
            Console.Write(context.Role);
            Console.Write(" - \"");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(context.Content);
            Console.ResetColor();
            Console.Write("\": ");

            var result = decisionProvider.Decide(context);
            Console.ForegroundColor = result ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(result);
            Console.ResetColor();
        }
    }
}
