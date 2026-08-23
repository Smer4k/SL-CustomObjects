#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Math
{
    public abstract class MathNode : BaseNode, IDynamicNode
    {
        public Type PortType { get; set; } = typeof(object);

        public override void OnEnable()
        {
            base.OnEnable();
            DefaultColor = NodeColors.MathColor;
        }

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            var portA = GetConnectedNodeIndex(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_A_PORT_NAME,
                nodeToRuntimeIndex);
            var portB = GetConnectedNodeIndex(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_B_PORT_NAME,
                nodeToRuntimeIndex);

            node.Properties["ANodeIndex"] = portA;
            node.Properties["BNodeIndex"] = portB;
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_A_PORT_NAME)
                .WithDataType(PortType)
                .WithDisplayName("A")
                .Build();
            context.AddInputPort(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_B_PORT_NAME)
                .WithDataType(PortType)
                .WithDisplayName("B")
                .Build();
            context.AddOutputPort(Constants.DYNAMIC_PORT_PREFIX + Constants.OUT_RESULT_PORT_NAME)
                .WithDataType(PortType)
                .WithDisplayName("Result")
                .Build();
        }
    }
}
#endif