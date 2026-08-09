#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes
{
    [Serializable]
    public abstract class BaseNode : Node
    {
        public abstract NodeType NodeType { get; }
        
        /// <summary>
        /// Defines common input and output execution ports for all nodes in the Action Director tool.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected void AddInputOutputExecutionPorts(IPortDefinitionContext context)
        {
            context.AddInputPort(Constants.EXECUTION_PORT_IN_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(Constants.EXECUTION_PORT_OUT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }

        public virtual void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            node.NodeType = NodeType;
            node.NextNodeIndex = GetNextNodeIndex(Constants.EXECUTION_PORT_OUT_NAME, nodeToRuntimeIndex);
            if (this is IDynamicNode dynamicNode)
            {
                node.DataType = dynamicNode.PortType.ToDataType();
            }
        }
        
        /// <summary>
        /// Resolves the runtime index of the node connected to the given output execution port.
        /// Returns -1 when the port is missing, not connected, or the target node is not in the map
        /// (i.e. the end of the execution chain).
        /// </summary>
        protected int GetNextNodeIndex(string outputPortName, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            return ResolveConnectedNodeIndex(GetOutputPortByName(outputPortName), outputPortName, nodeToRuntimeIndex);
        }

        /// <summary>
        /// Resolves the runtime index of the value node connected to the given input data port.
        /// Returns -1 when the port is missing, not connected, or the source node is not in the map.
        /// Executors use this index to fetch the value node from the runtime graph and evaluate it.
        /// </summary>
        protected int GetConnectedNodeIndex(string inputPortName, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            return ResolveConnectedNodeIndex(GetInputPortByName(inputPortName), inputPortName, nodeToRuntimeIndex);
        }
        
        private int ResolveConnectedNodeIndex(IPort port, string portName, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            if (port == null)
            {
                Debug.LogWarning($"{GetType().Name}: port '{portName}' not found.");
                return -1;
            }

            if (!port.IsConnected || port.FirstConnectedPort == null)
                return -1;

            return nodeToRuntimeIndex.GetValueOrDefault(port.FirstConnectedPort.GetNode(), -1);
        }
        
        public virtual void CheckErrors(List<string> msg)
        {
            var connected = new List<IPort>();
            foreach (var port in GetOutputPorts())
            {
                port.GetConnectedPorts(connected);
                if (port.DataType is not null)
                    continue;
                if (connected.Count > 0 && connected[0].DataType != null)
                {
                    msg.Add($"Недопустимое соединение");
                }
                if (connected.Count < 2)
                    continue;
                if (!string.IsNullOrEmpty(port.DisplayName))
                    msg.Add($"Output port \"{port.DisplayName}\" has too many connections (maximum: 1).");
                else
                    msg.Add("Output port has too many connections (maximum: 1).");
            }
            // Check duplicate port names (for input & output) - hard error
        }
    }
}
#endif