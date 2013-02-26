using System;
using System.Xml.Linq;

namespace Decision
{
    class Program
    {
        static DecisionProvider decisionProvider;

        static void Main(string[] args)
        {
            decisionProvider = new DecisionProvider("Decisions.xml");

            var configuration = XElement.Load("Tests.xml");
            string lastRole = null;

            foreach (var test in configuration.Elements("test"))
            {
                var content = (string)test.Attribute("content");
                var expect = bool.Parse(test.Attribute("expect").Value);
                var role = test.Attribute("role").Value;

                if (lastRole != role && lastRole != null)
                {
                    Console.WriteLine();
                }

                Test(new DecisionContext(role, content), expect);
                lastRole = role;
            }

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
    }
}
