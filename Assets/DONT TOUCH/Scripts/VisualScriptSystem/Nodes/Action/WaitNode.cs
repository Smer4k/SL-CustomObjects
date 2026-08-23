#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Action
{
    [Serializable]
    [Node("Action", "", "Wait")]
    public sealed class WaitNode : ActionNode
    {
        public override NodeType NodeType { get; } = NodeType.ActionWait;

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["TimeNodeIndex"] = GetConnectedNodeIndex(Constants.IN_VALUE_PORT_NAME, nodeToRuntimeIndex);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort<float>(Constants.IN_VALUE_PORT_NAME)
                .WithDisplayName("Time")
                .Build();
        }
    }
}
#endif