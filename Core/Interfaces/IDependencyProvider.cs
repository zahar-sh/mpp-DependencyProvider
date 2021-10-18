using System;

namespace DependencyInjectionContainer
{
    public interface IDependencyProvider
    {
        void Register<TDependency>(Lifetime lifetime = Lifetime.Instance) where TDependency : class;
        void Register(Type serviceType, Lifetime lifetime = Lifetime.Instance);
        void Register<TDependency, TImplementation>(Lifetime lifetime = Lifetime.Instance)
            where TDependency : class
            where TImplementation : class, TDependency;
        void Register(Type serviceType, Type implementationType, Lifetime lifetime = Lifetime.Instance);

        void Register<TDependency, TImplementation>(Func<Type, TImplementation> implementationFactory, Lifetime lifetime = Lifetime.Instance)
            where TDependency : class
            where TImplementation : class, TDependency;
        void Register<TDependency>(Func<Type, TDependency> implementationFactory, Lifetime lifetime = Lifetime.Instance) where TDependency : class;
        void Register(Type serviceType, Func<Type, object> implementationFactory, Lifetime lifetime = Lifetime.Instance);

        void RegisterSingleton<TDependency>(TDependency implementationInstance) where TDependency : class;
        void RegisterSingleton(Type serviceType, object implementationInstance);

        public TDependency Resolve<TDependency>();
        public object Resolve(Type serviceType);
    }
}
