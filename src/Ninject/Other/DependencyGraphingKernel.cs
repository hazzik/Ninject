using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Activation;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

namespace Ninject.Other
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDependencyGraphingKernel : IKernel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<DependencyGraphNode> LoadSupportedServices();
    }

    /// <summary>
    /// 
    /// </summary>
    public class DependencyGraphingKernel : StandardKernel, IDependencyGraphingKernel
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void AddComponents()
        {
            base.AddComponents();

            Components.RemoveAll<IMissingBindingResolver>();
            Components.Add<IMissingBindingResolver, FactoryBindingResolver>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DependencyGraphNode> LoadSupportedServices()
        {
            var allServices = Bindings.Keys.Distinct().ToArray();

            List<Type> unresolveableTypes = new List<Type>(allServices.Length);

            foreach (var service in allServices)
            {
                var request = CreateRequest(service, m => true, new IParameter[] { }, false, false);

                if (!CanResolve(request) && !HandleMissingBinding(request))
                {
                    unresolveableTypes.Add(service);
                }
            }

            List<DependencyGraphNode> result = new List<DependencyGraphNode>();

            foreach (KeyValuePair<Type, ICollection<IBinding>> bindings in Bindings)
            {
                foreach (var binding in bindings.Value)
                {
                    switch (binding.Target)
                    {
                    case BindingTarget.Self:
                    case BindingTarget.Type:
                        result.Add(GetNodeForTypeBinding(binding));
                        break;

                    case BindingTarget.Factory: // factory references are collapsed to be a reference on the type created.
                        //implementedBy = binding.Service;
                        //dependencies.Add(FactoryBindingResolver.TypeIsFactoryMethodFor(binding.Service));
                        break;
                    case BindingTarget.Constant:
                    case BindingTarget.Method:
                    case BindingTarget.Provider:
                        result.Add(new DependencyGraphNode()
                        {
                            ImplementationName =  GetBindingProvider(binding).GetType().Format(),
                            InterfaceType = binding.Service,
                            Dependencies = new Type[] {}
                        });
                        break;
                       
                    }
                }
            }

            return result;
        }

        private DependencyGraphNode GetNodeForTypeBinding(IBinding binding)
        {
            Type implementedBy = null;
            Type _ignored = null;
            var dependencies = GetDependenciesOfStandardProvider(binding, ref implementedBy, ref _ignored);

            return new DependencyGraphNode()
            {
                ImplementationName = implementedBy.Name,
                InterfaceType = binding.Service,
                Dependencies = dependencies
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="implementedBy"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public IEnumerable<Type> GetDependenciesOfStandardProvider(IBinding binding, ref Type implementedBy, ref Type providerType)
        {
            IProvider bindingProvider = GetBindingProvider(binding);

            providerType = bindingProvider.GetType();

            var standardProvider = bindingProvider as Activation.Providers.StandardProvider;

            if (standardProvider == null)
                return new Type[] { };

            implementedBy = standardProvider.Type;

            var constructors = standardProvider.Selector.SelectConstructorsForInjection(implementedBy);
            var methods = standardProvider.Selector.SelectMethodsForInjection(implementedBy);
            var properties = standardProvider.Selector.SelectPropertiesForInjection(implementedBy);

            var constructorParameters = constructors.SelectMany(c => c.GetParameters());
            var methodParameters = methods.SelectMany(m => m.GetParameters());
            var propertyParameters = properties.SelectMany(p => p.GetSetMethod().GetParameters());

            return constructorParameters.Concat(methodParameters).Concat(propertyParameters)
                .Select(p => p.ParameterType).Distinct();
        }

        private IProvider GetBindingProvider(IBinding binding)
        {
            var request = CreateRequest(binding.Service, m => true, new IParameter[] { }, false, false);
            var context = CreateContext(request, binding);

            return binding.ProviderCallback(context);
        }
    }
}
