using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Activation.Providers
{
    /// <summary>
    /// Allows for binding to factories as type Func[[T]]
    /// </summary>
    /// <typeparam name="T">type constructed by the factory</typeparam>
    public class FactoryProvider<T> : Provider<Func<T>> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public FactoryProvider()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Func<T> CreateInstance(IContext context)
        {
            return delegate()
            {
                return (T)context.Kernel.Get<T>();
            };
        }
    }
}
