namespace Decision
{
    public class DecisionProvider
    {
        private readonly ExpressionProvider expressionProvider;

        public DecisionProvider(ExpressionProvider expressionProvider)
        {
            this.expressionProvider = expressionProvider;
        }

        public bool Decide(DecisionContext context)
        {
            var expression = expressionProvider.Inflate(context);
            return expression(context);
        }
    }
}
