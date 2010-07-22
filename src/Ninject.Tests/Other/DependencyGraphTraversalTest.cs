using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Other;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Other
{
    public class DependencyGraphTraversalTest : DependencyGraphingKernelContext
    {
        [Fact]
        public void print_dependency_graph()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<Juggler>().ToSelf();
            kernel.Bind<ArmsMaster>().ToSelf();

            kernel.Bind<IWeapon>()
                    .To<Shuriken>()
                    .WhenTargetHas<InjectAttribute>();
            kernel.Bind<Ninja>().ToSelf();

            Console.Out.WriteLine();

            DependencyGraphTraversal.VisitGraph(
                kernel.LoadSupportedServices(),
                new PrintingDependencyGraphVisitor(Console.Out));
        }
    }
}
