using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ninject.Other;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Other
{
    public class PrintingDependencyGraphVisitorTest
    {
        Type interfaceType = typeof(IList<string>);

        [Fact]
        public void prints_interface_name()
        {
            var node = new DependencyGraphNode()
            {
                InterfaceType = interfaceType
            };

            string resultString = DoBeginNodeVisitForNode(node, 0);

            resultString.ShouldContain("IList");
        }

        [Fact]
        public void prints_implementation_name()
        {
            var node = new DependencyGraphNode()
            {
                InterfaceType = interfaceType,
                ImplementationName = "someString1254"
            };

            string resultString = DoBeginNodeVisitForNode(node, 0);

            resultString.ShouldContain(node.ImplementationName);
        }

        [Fact]
        public void prints_spacing()
        {
            int offset = 2;
            string expectedSpacing = "        ";

            var node = new DependencyGraphNode()
            {
                InterfaceType = interfaceType
            };

            string resultString = DoBeginNodeVisitForNode(node, offset);

            resultString.Substring(0, 8).ShouldBe(expectedSpacing);
        }

        private string DoBeginNodeVisitForNode(DependencyGraphNode node, int offset)
        {
            StringBuilder result = new StringBuilder();
            TextWriter writer = new StringWriter(result);

            var sut = new PrintingDependencyGraphVisitor(writer);

            sut.BeginNodeVisit(node, offset, new Type[0]);

            return result.ToString();
        }
    }
}
