﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
	public class EnumerableDependenciesContext
	{
		protected readonly StandardKernel kernel;

		public EnumerableDependenciesContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenServiceRequestsUnconstrainedEnumerableOfDependencies : EnumerableDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithAllAvailableDependencies()
		{
			kernel.Bind<IParent>().To<RequestsEnumerable>();
			kernel.Bind<IChild>().To<ChildA>();
			kernel.Bind<IChild>().To<ChildB>();

			var parent = kernel.Get<IParent>();

			parent.ShouldNotBeNull();
			parent.Children.ShouldNotBeNull();
			parent.Children.Length.ShouldBe(2);
			parent.Children[0].ShouldBeInstanceOf<ChildA>();
			parent.Children[1].ShouldBeInstanceOf<ChildB>();
		}
	}

	public class WhenServiceRequestsConstrainedEnumerableOfDependencies : EnumerableDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
		{
			kernel.Bind<IParent>().To<RequestsConstrainedEnumerable>();
			kernel.Bind<IChild>().To<ChildA>().Named("joe");
			kernel.Bind<IChild>().To<ChildB>().Named("bob");

			var parent = kernel.Get<IParent>();

			parent.ShouldNotBeNull();
			parent.Children.ShouldNotBeNull();
			parent.Children.Length.ShouldBe(1);
			parent.Children[0].ShouldBeInstanceOf<ChildB>();
		}
	}

	public interface IChild { }

	public class ChildA : IChild { }
	public class ChildB : IChild { }

	public interface IParent
	{
		IChild[] Children { get; }
	}

	public class RequestsEnumerable : IParent
	{
		public IChild[] Children { get; private set; }

		public RequestsEnumerable(IEnumerable<IChild> children)
		{
			Children = children.ToArray();
		}
	}

	public class RequestsConstrainedEnumerable : IParent
	{
		public IChild[] Children { get; private set; }

		public RequestsConstrainedEnumerable([Named("bob")] IEnumerable<IChild> children)
		{
			Children = children.ToArray();
		}
	}
}