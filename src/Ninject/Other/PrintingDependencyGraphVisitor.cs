using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject.Infrastructure.Introspection;

namespace Ninject.Other
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPrintingDependencyGraphVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="i"></param>
        /// <param name="missingDependencies"></param>
        void BeginNodeVisit(DependencyGraphNode node, int i, IEnumerable<Type> missingDependencies);
    }

    /// <summary>
    /// 
    /// </summary>
    public class PrintingDependencyGraphVisitor : IPrintingDependencyGraphVisitor
    {
        private readonly TextWriter _writer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public PrintingDependencyGraphVisitor(TextWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="i"></param>
        /// <param name="missingDependencies"></param>
        public void BeginNodeVisit(DependencyGraphNode node, int i, IEnumerable<Type> missingDependencies)
        {
            if (i == 0)
                _writer.WriteLine();

            _writer.Write(String.Concat(Enumerable.Range(0, i).Select(_ => "    ").ToArray()));

            if (node.ImplementationName == null
                || node.ImplementationName == node.InterfaceType.Format())
            {
                _writer.WriteLine("{0}", node.InterfaceType.Name);
            } else
            {
                _writer.WriteLine("{0} ({1})", node.ImplementationName, node.InterfaceType.Format());
            }

            if (missingDependencies.Any())
            {
                _writer.WriteLine("MISSING DEPENDENCIES: " +
                                  string.Join(", ", missingDependencies.Select(d => d.Format()).ToArray()));
            }
        }
    }
}