using System;

namespace Core
{
    internal class SingletonDependency : Dependency
    {
        private object _instance;

        public SingletonDependency(DependencyProvider provider, Type type, object instance = null)
            : base(provider, type)
        {
            _instance = instance;
        }
        public override object GetInstance()
        {
            lock (this)
            {
                return _instance ??= base.GetInstance();
            }
        }
    }
}
