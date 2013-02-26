using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Decision
{
    public class PolicyProvider
    {
        // internal windsor container, so we don't need to worry about scope issues with components being added to the same
        // container as any other code in the system. These policies should not really be available to access by other parts
        // of the application anyway.
        private static IWindsorContainer container = new WindsorContainer();

        public PolicyProvider(XElement settings)
        {
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

                container.Register(Component.For(type).Forward<IPolicy>().Named(key.Value).DependsOn(Dependencies(type, item).ToArray()));
            }
        }

        public IPolicy GetPolicy(string alias)
        {
            return container.Resolve<IPolicy>(alias);
        }

        private static IEnumerable<Dependency> Dependencies(Type type, XElement item)
        {
            // Efficiency block to avoid doing reflection work if there are no more attributes than the bare minimum (key and value)
            var attributes = item.Attributes();
            if (attributes.Count() > 2)
            {
                var constructors = type.GetConstructors();
                var parameters = constructors
                    .SelectMany(x => x.GetParameters())
                    .Where(x => x.ParameterType.IsPrimitive)
                    .ToArray();

                foreach (var attribute in attributes)
                {
                    // Ignore if key, value, not primitive or simply not a parameter
                    var name = attribute.Name.LocalName;
                    var info = parameters.FirstOrDefault(x => x.Name == name);
                    if (name == "key" || name == "value" || info == null)
                    {
                        continue;
                    }

                    // Convert type
                    var value = Convert.ChangeType(attribute.Value, info.ParameterType);
                    yield return Dependency.OnValue(name, value);
                }
            }
        }
    }
}
