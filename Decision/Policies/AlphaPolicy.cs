namespace Decision.Policies
{
    public class AlphaPolicy : IPolicy
    {
        public bool Decide(DecisionContext context)
        {
            return context.Content.Contains("Alpha");
        }
    }
}
