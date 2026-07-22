using System.Collections.Generic;
using System.Linq;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.Nodes;
using DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Value;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem
{
    public sealed class VisualScriptGraphPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var extension = "." + VisualScriptGraph.AssetExtension;
            var myImportedFiles = importedAssets
                .Where(path => path.EndsWith(extension))
                .ToList();
            if (myImportedFiles.Count == 0)
                return;

            Debug.Log($"[Postprocessor] Завершён импорт {myImportedFiles.Count} файлов ActionDirector.");
            foreach (var path in myImportedFiles)
            {
                var graph = GraphDatabase.LoadGraph<VisualScriptGraph>(path);
                var runtimeGraph = AssetDatabase.LoadAssetAtPath<VisualScriptRuntimeGraph>(path);
                var startNodeModel = graph.GetNodes().OfType<EventNode>().FirstOrDefault();
                if (startNodeModel == null)
                {
                    continue;
                }

                BuildRuntimeGraph(startNodeModel, runtimeGraph);
            }
        }

        /// <summary>
        /// Builds the runtime graph by traversing all reachable nodes from the start node.
        /// Supports both linear and branching paths.
        /// </summary>
        /// <param name="startNode">The start node of the graph</param>
        /// <param name="visualScriptRuntimeAsset">The runtime asset to populate</param>
        private static void BuildRuntimeGraph(INode startNode, VisualScriptRuntimeGraph visualScriptRuntimeAsset)
        {
            // Map from editor nodes to their starting index in the runtime nodes list
            var nodeToRuntimeIndex = new Dictionary<INode, int>();

            // Queue for breadth-first traversal
            var nodesToProcess = new Queue<INode>();

            nodesToProcess.Enqueue(startNode);

            // Process all reachable nodes
            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();

                // Skip if we've already processed this node
                if (nodeToRuntimeIndex.ContainsKey(currentNode))
                    continue;

                // Record the starting index for this node's runtime nodes
                nodeToRuntimeIndex[currentNode] = visualScriptRuntimeAsset.Nodes.Count;

                // Convert the editor node to runtime node(s)
                var runtimeNodes = new SerializableNode();
                visualScriptRuntimeAsset.Nodes.Add(runtimeNodes);

                foreach (var port in currentNode.GetInputPorts())
                {
                    if (port.DataType is null)
                        continue;
                    if (!port.IsConnected || port.FirstConnectedPort is null)
                    {
                        var constantNode = new SerializableNode();
                        nodeToRuntimeIndex[new VariableNode()] = visualScriptRuntimeAsset.Nodes.Count;
                        constantNode.NodeType = NodeType.Constant;
                        constantNode.DataType = port.DataType.ToDataType();
                        constantNode.NodeIndex = visualScriptRuntimeAsset.Nodes.Count;
                        if (port.TryGetValue(out object value))
                        {
                            constantNode.Properties["Value"] = value;
                        }
                        visualScriptRuntimeAsset.Nodes.Add(constantNode);
                        continue;
                    }
                    var node = port.FirstConnectedPort.GetNode();
                    if (node != null)
                        nodesToProcess.Enqueue(node);
                }

                foreach (var port in currentNode.GetOutputPorts())
                {
                    if (!port.IsConnected || port.FirstConnectedPort is null || port.DataType is not null)
                        continue;
                    var node = port.FirstConnectedPort.GetNode();
                    if (node != null)
                        nodesToProcess.Enqueue(node);
                }
            }

            // Second pass: set up NextNodeIndex references
            SetupNodeReferences(visualScriptRuntimeAsset, nodeToRuntimeIndex, startNode);
        }

        /// <summary>
        /// Sets up the NextNodeIndex references for all runtime nodes.
        /// </summary>
        private static void SetupNodeReferences(VisualScriptRuntimeGraph visualScriptRuntimeAsset,
            Dictionary<INode, int> nodeToRuntimeIndex, INode startNode)
        {
            var processedNodes = new HashSet<INode>();
            var nodesToProcess = new Queue<INode>();

            nodesToProcess.Enqueue(startNode);

            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();

                if (!processedNodes.Add(currentNode))
                    continue;

                if (!nodeToRuntimeIndex.TryGetValue(currentNode, out var currentRuntimeIndex))
                    continue;

                foreach (var port in currentNode.GetInputPorts())
                {
                    if (!port.IsConnected || port.FirstConnectedPort is null || port.DataType is null)
                        continue;
                    var node = port.FirstConnectedPort.GetNode();
                    if (node != null)
                        nodesToProcess.Enqueue(node);
                }

                foreach (var port in currentNode.GetOutputPorts())
                {
                    if (!port.IsConnected || port.FirstConnectedPort is null || port.DataType is not null)
                        continue;
                    var node = port.FirstConnectedPort.GetNode();
                    if (node != null)
                        nodesToProcess.Enqueue(node);
                }

                var runtimeNode = visualScriptRuntimeAsset.Nodes[currentRuntimeIndex];
                runtimeNode.NodeIndex = currentRuntimeIndex;

                if (currentNode is BaseNode baseNode)
                {
                    baseNode.Compile(runtimeNode, nodeToRuntimeIndex);
                    if (currentNode is EventNode && runtimeNode.NextNodeIndex == -1)
                    {
                        visualScriptRuntimeAsset.Nodes.Remove(runtimeNode);
                    }
                }

                // TODO: реаилизовать сериализацию ISubgraphNode
                // if (currentNode is ISubgraphNode subgraphNode)
                // {
                //     var subgraph = subgraphNode.GetSubgraph();
                //     subgraph
                // }
                if (currentNode is IVariableNode variableNode)
                {
                    runtimeNode.NodeType = NodeType.Constant;
                    runtimeNode.DataType = variableNode.Variable.DataType.ToDataType();
                    if (variableNode.Variable.TryGetDefaultValue<object>(out var value))
                        runtimeNode.Properties["Value"] = value;
                } else if (currentNode is IConstantNode constantNode)
                {
                    runtimeNode.NodeType = NodeType.Constant;
                    runtimeNode.DataType = constantNode.DataType.ToDataType();
                    if (constantNode.TryGetValue<object>(out var value))
                        runtimeNode.Properties["Value"] = value;
                }
            }
        }
    }
}