using System;
using System.Linq;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Integration
{
    public class FactoryBindingContext
    {
        protected readonly StandardKernel kernel;

        public FactoryBindingContext()
        {
            kernel = new StandardKernel();
        }
    }

    class FactoryBindingTests : FactoryBindingContext
    {
        [Fact]
        public void can_bind_factory()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<Juggler>().ToSelf();

            var result = kernel.Get<Juggler>();

            Assert.NotNull(result);
        }

        [Fact]
        public void factory_is_used()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<Juggler>().ToSelf();

            var result = kernel.Get<Juggler>();

            Assert.NotNull(result.Weapon);
            Assert.NotEqual(result.Weapon, result.Weapon);
        }
    }
}
