using System;
using System.Collections.Generic;
using Decision.Policies;

namespace Decision
{
    class Program
    {
        static DecisionProvider decisionProvider;

        static readonly IDictionary<string, Type> POLICIES = new Dictionary<string, Type>
            {
                { "A", typeof(AlphaPolicy) },
                { "B", typeof(BetaPolicy) },
                { "C", typeof(GammaPolicy) }
            };

        static readonly IDictionary<string, string> EXPRESSIONS = new Dictionary<string, string>
            {
                { "X", "(A|C)&B" },
                { "Y", "(A&C)|(A&B)|(A&B&C)" },
                { "Z", "B" }
            };

        static void Main(string[] args)
        {
            // Initialize
            var policyProvider = new PolicyProvider(POLICIES);
            var expressionProvider = new ExpressionProvider(EXPRESSIONS, policyProvider);
            decisionProvider = new DecisionProvider(expressionProvider);

            // Execute
            Console.WriteLine("X");
            Do(new DecisionContext("X", "Alpha Beta"));
            Do(new DecisionContext("X", "Alpha"));
            Do(new DecisionContext("X", "Alpha Gamma"));
            Do(new DecisionContext("X", "Alpha Beta Gamma"));
            Do(new DecisionContext("X", "Beta Gamma"));

            Console.WriteLine("Y");
            Do(new DecisionContext("Y", "Alpha Beta"));
            Do(new DecisionContext("Y", "Alpha Beta Gamma"));
            Do(new DecisionContext("Y", "Alpha Gamma"));
            Do(new DecisionContext("Y", "Beta Gamma"));
            Do(new DecisionContext("Y", "Alpha"));

            Console.WriteLine("Z");
            Do(new DecisionContext("Z", "Alpha"));
            Do(new DecisionContext("Z", "Beta"));
            Do(new DecisionContext("Z", "Gamma"));

            // Wait
            Console.ReadKey();
        }

        static void Do(DecisionContext context)
        {
            Console.Write(EXPRESSIONS[context.Role]);
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
