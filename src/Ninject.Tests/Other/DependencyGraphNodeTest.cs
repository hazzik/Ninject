using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Other;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Other
{
    public class DependencyGraphNodeTest
    {
        [Fact]
        public void DependencyGraphNode_satisfies_request_for_service()
        {
            var sut = new DependencyGraphNode();
            sut.InterfaceType = typeof(Sword);

            Assert.True(sut.SatisfiesDependencyOn(typeof(Sword)));
        }

        [Fact]
        public void DependencyGraphNode_doesnt_satisfy_other_services()
        {
            var sut = new DependencyGraphNode();
            sut.InterfaceType = typeof(Sword);

            Assert.False(sut.SatisfiesDependencyOn(typeof(Shuriken)));
        }

        [Fact]
        public void DependencyGraphNode_satisfies_request_for_enumerable_of_service()
        {
            var sut = new DependencyGraphNode();
            sut.InterfaceType = typeof(Sword);

            Assert.True(sut.SatisfiesDependencyOn(typeof(IEnumerable<Sword>)));
        }

        [Fact]
        public void DependencyGraphNode_doesnt_satisfy_request_for_enumerable_of_other_service()
        {
            var sut = new DependencyGraphNode();
            sut.InterfaceType = typeof(Sword);

            Assert.False(sut.SatisfiesDependencyOn(typeof(IList<Shuriken>)));
        }

        [Fact]
        public void DependencyGraphNode_satisfies_request_for_factory_of_service()
        {
            var sut = new DependencyGraphNode();
            sut.InterfaceType = typeof(Sword);

            Assert.True(sut.SatisfiesDependencyOn(typeof(Func<Sword>)));
        }

        [Fact]
        public void GetBoundServices_includes_Dependencies()
        {
            var expectedNode = new DependencyGraphNode()
            {
                InterfaceType = typeof(IWeapon)
            };
            
            var sut = new DependencyGraphNode();
            sut.Dependencies = new[] { typeof(IWeapon) };

            var result = sut.GetBoundServices(new DependencyGraphNode[] {
                expectedNode,
                sut
            });

            result.ShouldContain(expectedNode);
        }

        [Fact]
        public void GetMissingDependencies_includes_missing_dependencies()
        {
            var expectedNode = new DependencyGraphNode()
            {
                InterfaceType = typeof(IWeapon)
            };

            var sut = new DependencyGraphNode();
            sut.Dependencies = new[] { typeof(IWeapon), typeof(IWarrior) };

            var result = sut.GetMissingDependencies(new DependencyGraphNode[] {
                expectedNode,
                sut
            });

            result.ShouldContain(typeof(IWarrior));
        }
    }
}
