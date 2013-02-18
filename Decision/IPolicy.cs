namespace Decision
{
    public interface IPolicy
    {
        bool Decide(DecisionContext context);
    }
}
