using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using DONT_TOUCH.Scripts.VisualScriptSystem.Types;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Player.Condition
{
    [Serializable]
    [Node("Player/Condition", "", "HAS ITEM")]
    public sealed class HasItemNode : ConditionNode
    {
        public override NodeType NodeType { get; } = NodeType.ConditionHasItem;

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["PlayerNodeIndex"] = GetConnectedNodeIndex(Constants.IN_PLAYER_PORT_NAME, nodeToRuntimeIndex);
            node.Properties["ValueNodeIndex"] = GetConnectedNodeIndex(Constants.IN_VALUE_PORT_NAME, nodeToRuntimeIndex);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort<PlayerType>(Constants.IN_PLAYER_PORT_NAME)
                .WithDisplayName("Player")
                .Build();
            context.AddInputPort<ItemType>(Constants.IN_VALUE_PORT_NAME)
                .WithDisplayName("Item")
                .Build();
        }
    }
}