using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Components;
using Ninject.Infrastructure;

namespace Ninject.Planning.Bindings.Resolvers
{
    /// <summary>
    /// 
    /// </summary>
    public class FactoryBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        /// <summary>
        /// Will resolve Func[[T]] as FactoryProvider[[T]] if there is a binding for T.
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
        {
            if (!CanResolveType(request.Service))
                return Enumerable.Empty<IBinding>();

            var targetType = request.Service.GetGenericArguments()[0];

            if (!bindings.ContainsKey(targetType))
                return Enumerable.Empty<IBinding>();

            var providerConstructor = 
                typeof(FactoryProvider<object>)
                .GetGenericTypeDefinition()
                .MakeGenericType(new[] { targetType })
                .GetConstructor(new Type[] {});

            return new[] { new Binding(request.Service) {
                ProviderCallback = delegate
                {
                    return providerConstructor.Invoke(new object[] {}) as IProvider;
                },
                Target = BindingTarget.Factory
            }};
        }

        private static bool CanResolveType(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Func<>));
        }
    }
}
