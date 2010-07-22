using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Other
{
    /// <summary>
    /// 
    /// </summary>
    public class DependencyGraphTraversal
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="visitor"></param>
        static public void VisitGraph(IEnumerable<DependencyGraphNode> nodes, IPrintingDependencyGraphVisitor visitor)
        {
            Dictionary<DependencyGraphNode, int> dependencyTreeSize = new Dictionary<DependencyGraphNode, int>();
            Dictionary<DependencyGraphNode, int> countOfUsages = new Dictionary<DependencyGraphNode, int>();

            foreach (var node in nodes)
                countOfUsages[node] = 0;

            foreach (var node in nodes.OrderBy(n => n.GetBoundServices(nodes).Count()))
            {
                int nodeWeight = 0;

                foreach (var boundService in node.GetBoundServices(nodes))
                {
                    if (!dependencyTreeSize.ContainsKey(boundService))
                        nodeWeight += 1;  // depedendency exists, but could not be bound
                    else
                        nodeWeight += dependencyTreeSize[boundService];

                    countOfUsages[boundService] += 1;
                }

                dependencyTreeSize[node] = nodeWeight;
            }

            var nodesToVisit = new Stack<KeyValuePair<DependencyGraphNode, int>>();

            var topLevelNodes = nodes.Where(n => countOfUsages[n] == 0);
            foreach (var node in OrderByPreferenceReversed(dependencyTreeSize, topLevelNodes))
            {
                nodesToVisit.Push(new KeyValuePair<DependencyGraphNode, int>(node, 0));
            }

            while (nodesToVisit.Any())
            {
                var next = nodesToVisit.Pop();

                DependencyGraphNode node = next.Key;
                int depth = next.Value;

                var missingDependencies = node.GetMissingDependencies(nodes);

                visitor.BeginNodeVisit(node, depth, missingDependencies);

                foreach (var boundService in OrderByPreferenceReversed(dependencyTreeSize, node.GetBoundServices(nodes)))
                    nodesToVisit.Push(new KeyValuePair<DependencyGraphNode, int>(boundService, depth + 1));
            }
        }

        static IOrderedEnumerable<DependencyGraphNode> OrderByPreferenceReversed(
            Dictionary<DependencyGraphNode, int> dependencyTreeSize, 
            IEnumerable<DependencyGraphNode> nodes)
        {
            return nodes.OrderBy(n => dependencyTreeSize[n]).ThenBy(n => n.InterfaceType.Name);
        }
    }
}
