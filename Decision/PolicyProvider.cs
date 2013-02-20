using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

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

        public PolicyProvider(XElement settings)
        {
            policies = new Dictionary<string, Type>();
            foreach (var item in settings.Elements("item"))
            {
                var key = item.Attribute("key");
                if (key == null || string.IsNullOrWhiteSpace((string)key))
                {
                    throw new ConfigurationErrorsException("All policies must specify a unique 'key'.", item.ToXmlNode());
                }

                var value = item.Attribute("value");
                if (value == null || string.IsNullOrWhiteSpace((string)value))
                {
                    throw new ConfigurationErrorsException("All policies must specify an IPolicy policy type 'value'.", item.ToXmlNode());
                }

                var type = Type.GetType((string)value, false);
                if (type == null || type.GetInterface(typeof(IPolicy).FullName) == null)
                {
                    throw new ConfigurationErrorsException("All policies must implement 'Decision.IPolicy'.", item.ToXmlNode());
                }

                policies[(string)key] = type;
            }
        }

        public Type GetType(string alias)
        {
            if (policies.ContainsKey(alias))
            {
                return policies[alias];
            }

            throw new ArgumentException(string.Format("No policy for alias '{0}' registered.", alias), "alias");
        }

        public IPolicy GetPolicy(string alias)
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
