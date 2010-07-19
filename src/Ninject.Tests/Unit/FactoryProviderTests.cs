using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit
{
    public class FactoryProviderContext
	{
		protected readonly Mock<IContext> contextMock;
        protected readonly StandardKernel kernel;

        public FactoryProviderContext()
		{
			contextMock = new Mock<IContext>();
            kernel = new StandardKernel();

            contextMock.SetupGet(c => c.Kernel).Returns(kernel);
		}
	}

    class FactoryProviderTests : FactoryProviderContext
    {
        [Fact]
        public void can_produce_factory()
        {
            FactoryProvider<IWeapon> factoryProvider = new FactoryProvider<IWeapon>();

            object result = factoryProvider.Create(contextMock.Object);

            Assert.IsAssignableFrom(typeof(Func<IWeapon>), result);
        }

        [Fact]
        public void factory_uses_kernel()
        {
            var expectedWeapon = new Sword();

            kernel.Bind<IWeapon>().ToConstant(expectedWeapon);

            FactoryProvider<IWeapon> factoryProvider = new FactoryProvider<IWeapon>();

            var result = (Func<IWeapon>)factoryProvider.Create(contextMock.Object);

            Assert.Equal(expectedWeapon, result());
        }

        [Fact]
        public void factory_uses_kernel_for_each()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            FactoryProvider<IWeapon> factoryProvider = new FactoryProvider<IWeapon>();

            var result = (Func<IWeapon>)factoryProvider.Create(contextMock.Object);

            Assert.NotEqual(result(), result());
        }
    }
}
