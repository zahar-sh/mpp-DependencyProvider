using DependencyInjectionContainer;
using NUnit.Framework;
using System;

namespace CoreTests
{
    [TestFixture]
    public class DependencyProviderTests
    {
        interface ISomeObj
        {
            int Value { get; }
        }

        interface IAgregateObj
        {
            int Value { get; }
            ISomeObj Obj { get; }
        }

        interface IGenericObj<T>
        {
            int Value { get; }
            T TObj { get; }
        }

        class GenericImpl1 : IGenericObj<ISomeObj>
        {
            private int _value = new Random().Next();
            public int Value => _value;

            private ISomeObj _obj;
            public ISomeObj TObj => _obj;
        }

        class GenericImpl2 : IGenericObj<ISomeObj>
        {
            private int _Value = new Random().Next();
            public int Value => _Value;

            private ISomeObj _obj;
            public ISomeObj TObj => _obj;
        }

        class ObjImpl : ISomeObj
        {
            private int _Value = new Random().Next();
            public int Value => _Value;
        }

        class AgregateObj : IAgregateObj
        {
            private int _Value = new Random().Next();
            public int Value => _Value;


            private ISomeObj _obj;
            public ISomeObj Obj => _obj;

            public AgregateObj(ISomeObj obj)
            {
                _obj = obj;
            }
        }

        [Test]
        public void Resolve_DifferentValues_Instance()
        {
            var provider = new DependencyProvider();
            provider.Register<ISomeObj, ObjImpl>(Lifetime.Instance);
            int inst1 = provider.Resolve<ISomeObj>().Value;
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int inst2 = provider.Resolve<ISomeObj>().Value;
                    Assert.AreNotEqual(inst1, inst2);
                }
            });
        }

        [Test]
        public void Resolve_SameValues_Singleton()
        {
            var provider = new DependencyProvider();
            provider.Register<ISomeObj, ObjImpl>(Lifetime.Singleton);
            int inst1 = provider.Resolve<ISomeObj>().Value;
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int inst2 = provider.Resolve<ISomeObj>().Value;
                    Assert.AreEqual(inst1, inst2);
                }
            });
        }

        [Test]
        public void Resolve_Instance_Instance()
        {
            var provider = new DependencyProvider();
            provider.Register<ISomeObj, ObjImpl>(Lifetime.Instance);
            provider.Register<IAgregateObj, AgregateObj>(Lifetime.Instance);

            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                    Assert.AreNotEqual(inst1.Value, inst2.Value);
                    Assert.AreNotEqual(inst1.Obj.Value, inst2.Obj.Value);
                }
            });
        }

        [Test]
        public void Resolve_Instance_Singleton()
        {
            var provider = new DependencyProvider();
            provider.Register<ISomeObj, ObjImpl>(Lifetime.Instance);
            provider.Register<IAgregateObj, AgregateObj>(Lifetime.Singleton);
            int val1 = provider.Resolve<ISomeObj>().Value;
            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int val2 = provider.Resolve<ISomeObj>().Value;
                    IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                    Assert.AreNotEqual(val1, val2);
                    Assert.AreEqual(inst1.Value, inst2.Value);
                    Assert.AreEqual(inst1.Obj.Value, inst2.Obj.Value);
                }
            });
        }

        [Test]
        public void Resolve_Singleton_Instance()
        {
            var provider = new DependencyProvider();
            provider.Register<ISomeObj, ObjImpl>(Lifetime.Singleton);
            provider.Register<IAgregateObj, AgregateObj>(Lifetime.Instance);
            int val1 = provider.Resolve<ISomeObj>().Value;
            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int val2 = provider.Resolve<ISomeObj>().Value;
                    IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                    Assert.AreEqual(val1, val2);
                    Assert.AreNotEqual(inst1.Value, inst2.Value);
                    Assert.AreEqual(inst1.Obj.Value, inst2.Obj.Value);
                }
            });
        }

        [Test]
        public void Resolve_Singleton_Singleton()
        {
            var provider = new DependencyProvider();

            provider.Register<ISomeObj, ObjImpl>(Lifetime.Singleton);
            provider.Register<IAgregateObj, AgregateObj>(Lifetime.Singleton);
            int val1 = provider.Resolve<ISomeObj>().Value;
            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int val2 = provider.Resolve<ISomeObj>().Value;
                    IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                    Assert.AreEqual(val1, val2);
                    Assert.AreEqual(inst1.Value, inst2.Value);
                    Assert.AreEqual(inst1.Obj.Value, inst2.Obj.Value);
                }
            });
        }
    }
}