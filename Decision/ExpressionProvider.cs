using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Decision
{
    public class ExpressionProvider
    {
        private readonly IDictionary<string, Predicate<DecisionContext>> built = new Dictionary<string, Predicate<DecisionContext>>();
        private readonly IDictionary<string, string> expressions;
        private readonly ParameterExpression contextParameter = Expression.Parameter(typeof(DecisionContext), "context");
        private readonly PolicyProvider provider;

        public ExpressionProvider(IDictionary<string, string> expressions, PolicyProvider provider)
        {
            this.expressions = expressions.ToDictionary(x => x.Key, x => Reduce(x.Value));
            this.provider = provider;
        }

        public ExpressionProvider(XElement settings, PolicyProvider provider)
        {
            expressions = new Dictionary<string, string>();
            this.provider = provider;

            foreach (var item in settings.Elements("item"))
            {
                var key = item.Attribute("key");
                if (key == null || string.IsNullOrWhiteSpace((string)key))
                {
                    throw new ConfigurationErrorsException("All expressions must specify a unique 'key'.", item.ToXmlNode());
                }

                var value = item.Attribute("value");
                if (value == null || string.IsNullOrWhiteSpace((string)value))
                {
                    throw new ConfigurationErrorsException("All expressions must specify a 'value'.", item.ToXmlNode());
                }

                expressions[(string)key] = Reduce((string)value);
            }
        }

        public Predicate<DecisionContext> Inflate(DecisionContext context)
        {
            if (built.ContainsKey(context.Role) == false)
            {
                var expression = Expression.Lambda<Predicate<DecisionContext>>(Parse(expressions[context.Role]), contextParameter);
                built[context.Role] = expression.Compile();
            }

            return built[context.Role];
        }

        private string Reduce(string input)
        {
            var output = input;

            output = Regex.Replace(output, @"(?<=\W)AND(?=\W)", "&", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<=\W)OR(?=\W)", "|", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"\.", "&");
            output = Regex.Replace(output, @"\+", "|");
            output = Regex.Replace(output, @"\s", "");

            return output;
        }

        private Expression Parse(string input)
        {
            Expression expression = null;
            var variable = string.Empty;
            var operation = '&';
            var depth = 0;

            foreach (var c in input)
            {
                switch (c)
                {
                    case '(':
                        depth++;
                        break;

                    case ')':
                        depth--;
                        if (depth == 0)
                        {
                            expression = Combine(expression, Parse(variable), operation);
                            variable = string.Empty;
                        }

                        break;

                    case '&':
                    case '|':
                        if (depth == 0)
                        {
                            if (variable.Length > 0)
                            {
                                expression = Combine(expression, Call(variable), operation);
                                variable = string.Empty;
                            }

                            operation = c;
                        }
                        else
                        {
                            variable += c;
                        }

                        break;

                    default:
                        variable += c;
                        break;
                }
            }

            // Ensure we don't drop the last variable
            if (variable.Length > 0)
            {
                expression = Combine(expression, Call(variable), operation);
                variable = string.Empty;
            }

            return expression;
        }

        private Expression Combine(Expression lhs, Expression rhs, char operation)
        {
            if (lhs == null)
            {
                return rhs;
            }

            switch (operation)
            {
                case '&':
                    return Expression.AndAlso(lhs, rhs);

                case '|':
                    return Expression.OrElse(lhs, rhs);

                default:
                    throw new InvalidOperationException(string.Format("The operation '{0}' is not recognized", operation));
            }
        }

        private Expression Call(string alias)
        {
            return Expression.Call(Expression.Constant(provider.Get(alias)), typeof(IPolicy).GetMethod("Decide", new Type[] { typeof(DecisionContext) }), contextParameter);
        }
    }
}
