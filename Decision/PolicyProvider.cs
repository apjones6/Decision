using System;
using System.Collections.Generic;
using System.Linq;

namespace Decision
{
    public class PolicyProvider
    {
        private readonly IDictionary<string, IPolicy> instances = new Dictionary<string, IPolicy>();
        private readonly IDictionary<string, Type> policies;

        public PolicyProvider(IDictionary<string, Type> policies)
        {
            if (policies.Any(x => x.Value.GetInterface(typeof(IPolicy).FullName) == null))
            {
                throw new ArgumentException("All policies must implement the Decision.IPolicy interface.", "policies");
            }

            this.policies = policies;
        }

        public IPolicy Get(string alias)
        {
            if (instances.ContainsKey(alias))
            {
                return instances[alias];
            }

            if (policies.ContainsKey(alias))
            {
                var policy = (IPolicy)Activator.CreateInstance(policies[alias]);
                instances[alias] = policy;
                return policy;
            }

            throw new ArgumentException(string.Format("No policy for alias '{0}' registered.", alias), "alias");
        }
    }
}
