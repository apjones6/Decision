using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Decision
{
    public class ExpressionProvider
    {
        private readonly IDictionary<string, Predicate<DecisionContext>> built = new Dictionary<string, Predicate<DecisionContext>>();
        private readonly IDictionary<string, string> expressions;
        private readonly ParameterExpression contextParameter;
        private readonly PolicyProvider provider;

        public ExpressionProvider(IDictionary<string, string> expressions, PolicyProvider provider)
        {
            this.expressions = expressions;
            this.contextParameter = Expression.Parameter(typeof(DecisionContext), "context");
            this.provider = provider;
        }

        public Predicate<DecisionContext> Inflate(DecisionContext context)
        {
            if (built.ContainsKey(context.Role) == false)
            {
                var expression = Expression.Lambda<Predicate<DecisionContext>>(Build(expressions[context.Role]), contextParameter);
                built[context.Role] = expression.Compile();
            }

            return built[context.Role];
        }

        private Expression Build(string input)
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
                            expression = Append(expression, Build(variable), operation);
                            variable = string.Empty;
                        }

                        break;

                    case '&':
                    case '|':
                        if (depth == 0)
                        {
                            if (variable.Length > 0)
                            {
                                expression = Append(expression, Call(variable), operation);
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
                expression = Append(expression, Call(variable), operation);
                variable = string.Empty;
            }

            return expression;
        }

        private Expression Append(Expression lhs, Expression rhs, char operation)
        {
            if (lhs == null)
            {
                return rhs;
            }
            else if (operation == '&')
            {
                return Expression.AndAlso(lhs, rhs);
            }
            else
            {
                return Expression.OrElse(lhs, rhs);
            }
        }

        private Expression Call(string alias)
        {
            return Expression.Call(Expression.Constant(provider.Get(alias)), typeof(IPolicy).GetMethod("Decide", new Type[] { typeof(DecisionContext) }), contextParameter);
        }
    }
}
