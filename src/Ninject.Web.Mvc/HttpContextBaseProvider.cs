using System;
using System.Web;
using Ninject.Activation;

namespace Ninject.Web.Mvc
{
	public class HttpContextBaseProvider : Provider<HttpContextBase>
	{
		protected override HttpContextBase CreateInstance(IContext context)
		{
			return new HttpContextWrapper(HttpContext.Current);
		}
	}
}