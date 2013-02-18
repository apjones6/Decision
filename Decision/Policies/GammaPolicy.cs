namespace Decision.Policies
{
    public class GammaPolicy : IPolicy
    {
        public bool Decide(DecisionContext context)
        {
            return context.Content.Contains("Gamma");
        }
    }
}
