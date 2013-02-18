namespace Decision.Policies
{
    public class BetaPolicy : IPolicy
    {
        public bool Decide(DecisionContext context)
        {
            return context.Content.Contains("Beta");
        }
    }
}
