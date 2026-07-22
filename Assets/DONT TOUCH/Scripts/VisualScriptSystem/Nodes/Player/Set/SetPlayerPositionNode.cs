using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using DONT_TOUCH.Scripts.VisualScriptSystem.Types;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Player.Set
{
    [Serializable]
    [Node("Player/Set", "", "Player position")]
    public sealed class SetPlayerPositionNode : SetNode
    {
        public override NodeType NodeType { get; } = NodeType.SetPlayerPosition;

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["PlayerNodeIndex"] =
                GetConnectedNodeIndex(Constants.IN_PLAYER_PORT_NAME, nodeToRuntimeIndex);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
            context.AddInputPort<PlayerType>(Constants.IN_PLAYER_PORT_NAME)
                .WithDisplayName("Player")
                .Build();
            AddInputSetValuePort(context, typeof(Vector3));
        }
    }
}