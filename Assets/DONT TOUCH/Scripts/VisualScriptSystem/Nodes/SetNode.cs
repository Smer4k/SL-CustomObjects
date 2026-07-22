using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes
{
    public abstract class SetNode : BaseNode
    {
        public override void OnEnable()
        {
            base.OnEnable();
            DefaultColor = NodeColors.SetColor;
        }

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["ValueNodeIndex"] = GetConnectedNodeIndex(Constants.IN_VALUE_PORT_NAME, nodeToRuntimeIndex);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
        }

        protected void AddInputSetValuePort(IPortDefinitionContext ctx, Type typePort)
        {
            ctx.AddInputPort(Constants.IN_VALUE_PORT_NAME)
                .WithDataType(typePort)
                .WithDisplayName("Value")
                .Build();
        }
    }
}