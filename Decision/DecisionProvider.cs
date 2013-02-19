using System.IO;
using System.Xml.Linq;

namespace Decision
{
    public class DecisionProvider
    {
        private readonly ExpressionProvider expressionProvider;
        
        public DecisionProvider(ExpressionProvider expressionProvider)
        {
            this.expressionProvider = expressionProvider;
        }

        public DecisionProvider(string configuration)
        {
            var settings = XElement.Load(Path.GetFullPath(configuration));
            var policyProvider = new PolicyProvider(settings.Element("policies"));
            expressionProvider = new ExpressionProvider(settings.Element("expressions"), policyProvider);
        }

        public bool Decide(DecisionContext context)
        {
            var expression = expressionProvider.Inflate(context);
            return expression(context);
        }
    }
}
