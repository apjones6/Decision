namespace Decision.Policies
{
    public class DeltaPolicy : IPolicy
    {
        private readonly int occurrences;

        public DeltaPolicy(int occurrences)
        {
            this.occurrences = occurrences;
        }

        public bool Decide(DecisionContext context)
        {
            var skip = 0;
            var count = 0;

            while (count < occurrences)
            {
                if (skip >= context.Content.Length)
                {
                    return false;
                }

                var index = context.Content.IndexOf("Delta", skip);
                if (index == -1)
                {
                    return false;
                }

                skip = index + 5;
                count++;
            }

            return true;
        }
    }
}
