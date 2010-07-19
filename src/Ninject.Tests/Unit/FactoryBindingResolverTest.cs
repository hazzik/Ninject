using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit
{
    public class FactoryBindingResolverContext
    {
        protected readonly Mock<IRequest> request = new Mock<IRequest>();

    }

    public class FactoryBindingResolverTest : FactoryBindingResolverContext
    {
        [Fact]
        public void doesnt_resolve_other_types()
        {
            request.SetupGet(r => r.Service).Returns(typeof(Sword));

            var resolver = new FactoryBindingResolver();

            var result = resolver.Resolve(new Multimap<Type, IBinding>(), request.Object);

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void doesnt_resolve_for_factory_of_nonexisting_service()
        {
            request.SetupGet(r => r.Service).Returns(typeof(Func<Sword>));

            var resolver = new FactoryBindingResolver();

            var result = resolver.Resolve(new Multimap<Type, IBinding>(), request.Object);

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public IBinding resolves_for_factory_of_existing_service()
        {
            request.SetupGet(r => r.Service).Returns(typeof(Func<Sword>));

            var resolver = new FactoryBindingResolver();

            var maps = new Multimap<Type, IBinding>();
            maps.Add(typeof(Sword), new Binding(typeof(Sword)));

            IBinding result = resolver.Resolve(maps, request.Object).Single();

            return result;
        }

        [Fact]
        public void resolves_for_factory_of_existing_service_as_FactoryProvider()
        {
            var binding = resolves_for_factory_of_existing_service();

            Assert.IsType<FactoryProvider<Sword>>(binding.ProviderCallback(null));
            Assert.Equal(BindingTarget.Factory, binding.Target);
        }
    }
}
