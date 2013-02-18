namespace Decision
{
    public class DecisionContext
    {
        private readonly string content;
        private readonly string role;

        public DecisionContext(string role, string content)
        {
            this.content = content;
            this.role = role;
        }

        public string Content
        {
            get { return content; }
        }

        public string Role
        {
            get { return role; }
        }
    }
}
