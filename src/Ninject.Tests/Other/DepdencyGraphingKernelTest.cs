using System;
using System.Linq;
using Ninject.Other;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Other
{
    public class DependencyGraphingKernelContext
    {
        protected readonly DependencyGraphingKernel kernel;

        public DependencyGraphingKernelContext()
        {
            kernel = new DependencyGraphingKernel();
        }
    }


    public class DependencyGraphingKernelTest : DependencyGraphingKernelContext
    {
        [Fact]
        public void preloads_missing_dependencies()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<Juggler>().ToSelf();

            Assert.Equal(0, kernel.GetBindings(typeof(Func<IWeapon>)).Count());

            kernel.LoadSupportedServices();

            Assert.Equal(1, kernel.GetBindings(typeof(Func<IWeapon>)).Count());
        }

        [Fact]
        public void type_output_includes_InterfaceType_when_bound_to_a_type()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var result = kernel.LoadSupportedServices();

            Assert.Equal(typeof(IWeapon), result.First().InterfaceType);
        }

        [Fact]
        public void type_output_includes_InterfaceType_when_bound_otherwise()
        {
            kernel.Bind<IWeapon>().ToConstant(new Sword());

            var result = kernel.LoadSupportedServices();

            Assert.Equal(typeof(IWeapon), result.First().InterfaceType);
        }

        [Fact]
        public void output_includes_ImplementationName_when_bound_to_type()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var result = kernel.LoadSupportedServices();

            Assert.Equal("Sword", result.First().ImplementationName);
        }

        [Fact]
        public void output_includes_ImplementationName_when_bound_otherwise()
        {
            kernel.Bind<IWeapon>().ToConstant(new Sword());

            var result = kernel.LoadSupportedServices();

            Assert.Equal("ConstantProvider{IWeapon}", result.First().ImplementationName);
        }

        [Fact]
        public void output_includes_dependencies_when_bound_to_types()
        {
            kernel.Bind<IWarrior>().To<Ninja>();

            var result = kernel.LoadSupportedServices();

            Assert.True(result.First().Dependencies.Contains(typeof(IWeapon)));
        }

        [Fact]
        public void output_has_no_dependencies_when_bound_otherwise()
        {
            kernel.Bind<IWeapon>().ToConstant(new Sword());

            var result = kernel.LoadSupportedServices();

            Assert.Equal(0, result.First().Dependencies.Count());
        }
    }
}
