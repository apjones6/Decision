﻿using System;

namespace Decision
{
    class Program
    {
        static DecisionProvider decisionProvider;

        static void Main(string[] args)
        {
            decisionProvider = new DecisionProvider("Decisions.xml");

            // Execute
            T(new DecisionContext("X", "Alpha Beta"));
            F(new DecisionContext("X", "Alpha"));
            F(new DecisionContext("X", "Alpha Gamma"));
            T(new DecisionContext("X", "Alpha Beta Gamma"));
            T(new DecisionContext("X", "Beta Gamma"));
            Console.WriteLine();

            T(new DecisionContext("Y", "Alpha Beta"));
            T(new DecisionContext("Y", "Alpha Beta Gamma"));
            T(new DecisionContext("Y", "Alpha Gamma"));
            F(new DecisionContext("Y", "Beta Gamma"));
            F(new DecisionContext("Y", "Alpha"));
            Console.WriteLine();

            F(new DecisionContext("Z", "Alpha"));
            T(new DecisionContext("Z", "Beta"));
            F(new DecisionContext("Z", "Gamma"));
            Console.WriteLine();

            // Wait
            Console.ReadKey();
        }

        static void Test(DecisionContext context, bool expected)
        {
            Console.Write(context.Role);
            Console.Write(" - \"");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(context.Content);
            Console.ResetColor();
            Console.Write("\": ");

            var result = decisionProvider.Decide(context);
            Console.ForegroundColor = result == expected ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(result);
            Console.ResetColor();
        }

        static void T(DecisionContext context)
        {
            Test(context, true);
        }

        static void F(DecisionContext context)
        {
            Test(context, false);
        }
    }
}
