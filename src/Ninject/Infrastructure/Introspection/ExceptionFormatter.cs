﻿#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ninject.Activation;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Infrastructure.Introspection
{
	internal static class ExceptionFormatter
	{
		public static string CouldNotResolveBinding(IRequest request)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error activating {0}", request.Service.Format());
				sw.WriteLine("No matching bindings are available, and the type is not self-bindable.");

				sw.WriteLine("Activation path:");
				sw.WriteLine(request.FormatActivationPath());

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) Ensure that you have defined a binding for {0}.", request.Service.Format());
				sw.WriteLine("  2) If the binding was defined in a module, ensure that the module has been loaded into the kernel.");
				sw.WriteLine("  3) Ensure you have not accidentally created more than one kernel.");
				#if !SILVERLIGHT
				sw.WriteLine("  4) If you are using automatic module loading, ensure the search path and filters are correct.");
				#endif

				return sw.ToString();
			}
		}

		public static string CyclicalDependenciesDetected(IContext context)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error activating {0} using {1}", context.Request.Service.Format(), context.Binding.Format(context));
				sw.WriteLine("A cyclical dependency was detected between the constructors of two services.");
				sw.WriteLine();

				sw.WriteLine("Activation path:");
				sw.WriteLine(context.Request.FormatActivationPath());

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) Ensure that you have not declared a dependency for {0} on any implementations of the service.", context.Request.Service.Format());
				sw.WriteLine("  2) Consider combining the services into a single one to remove the cycle.");
				sw.WriteLine("  3) Use property injection instead of constructor injection, and implement IInitializable");
				sw.WriteLine("     if you need initialization logic to be run after property values have been injected.");

				return sw.ToString();
			}
		}

		public static string InvalidAttributeTypeUsedInBindingCondition(IBinding binding, string methodName, Type type)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error registering binding for {0}", binding.Service.Format());
				sw.WriteLine("The type {0} used in a call to {1}() is not a valid attribute.", type.Format(), methodName);
				sw.WriteLine();

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) Ensure that you have passed the correct type.");
				sw.WriteLine("  2) If you have defined your own attribute type, ensure that it extends System.Attribute.");
				sw.WriteLine("  3) To avoid problems with type-safety, use the generic version of the the method instead,");
				sw.WriteLine("     such as {0}<SomeAttribute>().", methodName);

				return sw.ToString();
			}
		}

		public static string NoConstructorsAvailable(IContext context)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error activating {0} using {1}", context.Request.Service.Format(), context.Binding.Format(context));
				sw.WriteLine("No constructor was available to create an instance of the implementation type.");
				sw.WriteLine();

				sw.WriteLine("Activation path:");
				sw.WriteLine(context.Request.FormatActivationPath());

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) Ensure that the implementation type has a public constructor.");
				sw.WriteLine("  2) If you have implemented the Singleton pattern, use a binding with InSingletonScope() instead.");

				return sw.ToString();
			}
		}

		public static string NoConstructorsAvailableForComponent(Type component, Type implementation)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error loading Ninject component {0}", component.Format());
				sw.WriteLine("No constructor was available to create an instance of the registered implementation type {0}.", implementation.Format());
				sw.WriteLine();

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) Ensure that the implementation type has a public constructor.");

				return sw.ToString();
			}
		}

		public static string NoSuchComponentRegistered(Type component)
		{
			using (var sw = new StringWriter())
			{
				sw.WriteLine("Error loading Ninject component {0}", component.Format());
				sw.WriteLine("No such component has been registered in the kernel's component container.");
				sw.WriteLine();

				sw.WriteLine("Suggestions:");
				sw.WriteLine("  1) If you have created a custom subclass for KernelBase, ensure that you have properly");
				sw.WriteLine("     implemented the AddComponents() method.");
				sw.WriteLine("  2) Ensure that you have not removed the component from the container via a call to RemoveAll().");
				sw.WriteLine("  3) Ensure you have not accidentally created more than one kernel.");

				return sw.ToString();
			}
		}
	}
}