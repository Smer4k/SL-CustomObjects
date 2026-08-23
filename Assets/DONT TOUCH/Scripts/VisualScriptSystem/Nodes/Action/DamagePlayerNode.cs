#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using DONT_TOUCH.Scripts.VisualScriptSystem.Types;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Action
{
    [Serializable]
    [Node("Action", "", "Damage player")]
    public sealed class DamagePlayerNode : ActionNode
    {
        public override NodeType NodeType { get; } = NodeType.ActionDamagePlayer;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort<PlayerType>(Constants.IN_PLAYER_PORT_NAME)
                .WithDisplayName("Player")
                .Build();
            context.AddInputPort<float>(Constants.IN_VALUE_PORT_NAME)
                .WithDisplayName("Damage")
                .Build();
        }
        
        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["PlayerNodeIndex"] =
                GetConnectedNodeIndex(Constants.IN_PLAYER_PORT_NAME, nodeToRuntimeIndex);
            node.Properties["DamageNodeIndex"] =
                GetConnectedNodeIndex(Constants.IN_VALUE_PORT_NAME, nodeToRuntimeIndex);
        }
    }
}
#endif