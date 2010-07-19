using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration
{
    public class MultiDependencyContext
    {
        protected readonly StandardKernel kernel;

        public MultiDependencyContext()
        {
            kernel = new StandardKernel();
        }
    }

    public class MultiDependency : MultiDependencyContext
    {
        [Fact]
        public void can_get_multiple()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>();
            kernel.Bind<ArmsMaster>().ToSelf();

            var results = kernel.GetAll<IWeapon>(m => true);

            results.Count().ShouldBe(2);

            var includer = kernel.Get<ArmsMaster>();

            includer.Weapons.Count().ShouldBe(2);
        }
    }
}
