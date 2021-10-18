using System;

namespace DependencyInjectionContainer
{
    public class InstanceHolder
    {
        private readonly Type _serviceType;
        private readonly Func<Type, object> _implementationFactory;
        private object _instance;

        public InstanceHolder(Type serviceType, Func<Type, object> implementationFactory)
        {
            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _implementationFactory = implementationFactory ?? throw new ArgumentNullException(nameof(implementationFactory));
        }

        public object GetInstance()
        {
            lock (_implementationFactory)
            {
                return _instance ??= _implementationFactory.Invoke(_serviceType);
            }
        }
    }
}
