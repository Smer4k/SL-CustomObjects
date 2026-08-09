#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Value
{
    [Serializable]
    [Node("", "", "Variable")]
    public class VariableNode : BaseNode, IDynamicNode
    {
        public Type PortType { get; set; } = typeof(object);
        public override NodeType NodeType { get; } = NodeType.Variable;

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["InputNodeIndex"] =
                GetConnectedNodeIndex(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_VALUE_PORT_NAME, nodeToRuntimeIndex);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
            context.AddInputPort(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_VALUE_PORT_NAME)
                .WithDataType(PortType)
                .WithDisplayName("Input")
                .Build();
            context.AddOutputPort(Constants.DYNAMIC_PORT_PREFIX + Constants.OUT_VALUE_PORT_NAME)
                .WithDataType(PortType)
                .WithDisplayName("Result")
                .Build();
        }
    }
}
#endif