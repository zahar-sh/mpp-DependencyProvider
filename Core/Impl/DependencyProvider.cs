using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionContainer
{
    public class DependencyProvider : IDependencyProvider
    {
        private static bool IsBaseTypeOrInterface(Type type, Type parent)
        {
            return Equals(type.BaseType, parent) ||
                type.GetInterfaces()
                .Any(t => t.Equals(parent));
        }
        private static void Validate(Type serviceType, Type implementationType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (!IsBaseTypeOrInterface(implementationType, serviceType))
            {
                throw new ArgumentException($"Implementation type: '{implementationType}" +
                    $"' not implements service type: '{serviceType}'");
            }
        }

        private readonly IDictionary<Type, Func<object>> _serviceGeneratorsDictionary
            = new ConcurrentDictionary<Type, Func<object>>();

        public void Register<TDependency>(Lifetime lifetime) where TDependency : class
        {
            Register(typeof(TDependency), typeof(TDependency), lifetime);
        }
        public void Register(Type serviceType, Lifetime lifetime)
        {
            Register(serviceType, serviceType, lifetime);
        }

        public void Register<TDependency, TImplementation>(Lifetime lifetime)
            where TDependency : class
            where TImplementation : class, TDependency
        {
            Register(typeof(TDependency), typeof(TImplementation), lifetime);
        }
        public void Register(Type serviceType, Type implementationType, Lifetime lifetime)
        {
            Validate(serviceType, implementationType);
            switch (lifetime)
            {
                case Lifetime.Instance:
                    _serviceGeneratorsDictionary.Add(serviceType, () => CreateInstanceFromType(implementationType));
                    break;
                case Lifetime.Singleton:
                    var holder = new InstanceHolder(serviceType, CreateInstanceFromType);
                    _serviceGeneratorsDictionary.Add(serviceType, () => holder.GetInstance());
                    break;
            }
        }

        public void Register<TDependency, TImplementation>(Func<Type, TImplementation> implementationFactory, Lifetime lifetime)
            where TDependency : class
            where TImplementation : class, TDependency
        {
            Register(typeof(TDependency), implementationFactory, lifetime);
        }
        public void Register<TDependency>(Func<Type, TDependency> implementationFactory, Lifetime lifetime) where TDependency : class
        {
            Register(typeof(TDependency), implementationFactory, lifetime);
        }
        public void Register(Type serviceType, Func<Type, object> implementationFactory, Lifetime lifetime)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }

            switch (lifetime)
            {
                case Lifetime.Instance:
                    _serviceGeneratorsDictionary.Add(serviceType, () => implementationFactory.Invoke(serviceType));
                    break;
                case Lifetime.Singleton:
                    var holder = new InstanceHolder(serviceType, implementationFactory);
                    _serviceGeneratorsDictionary.Add(serviceType, () => holder.GetInstance());
                    break;
            }
        }

        public void RegisterSingleton<TDependency>(TDependency implementationInstance) where TDependency : class
        {
            if (implementationInstance is null)
            {
                throw new ArgumentNullException(nameof(implementationInstance));
            }

            _serviceGeneratorsDictionary.Add(typeof(TDependency), () => implementationInstance);
        }
        public void RegisterSingleton(Type serviceType, object implementationInstance)
        {
            if (implementationInstance is null)
            {
                throw new ArgumentNullException(nameof(implementationInstance));
            }

            Validate(serviceType, implementationInstance.GetType());

            _serviceGeneratorsDictionary.Add(serviceType, () => implementationInstance);
        }

        public TDependency Resolve<TDependency>()
        {
            return (TDependency)Resolve(typeof(TDependency));
        }
        public object Resolve(Type type)
        {
            if (_serviceGeneratorsDictionary.TryGetValue(type, out var generator))
            {
                return generator.Invoke();
            }
            return null;
        }

        private object CreateInstanceFromType(Type type)
        {
            foreach (var constructor in type.GetConstructors(System.Reflection.BindingFlags.Public))
            {
                try
                {
                    object[] args = constructor
                        .GetParameters()
                        .Select(param => param.ParameterType)
                        .Select(Resolve).ToArray();
                    return constructor.Invoke(args);
                }
                catch
                {
                }
            }
            return Activator.CreateInstance(type);
        }
    }
}
