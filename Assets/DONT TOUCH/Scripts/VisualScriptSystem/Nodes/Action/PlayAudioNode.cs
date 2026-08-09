#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Action
{
    [Serializable]
    [Node("Action", "", "Play Audio")]
    public sealed class PlayAudioNode : ActionNode
    {
        public override NodeType NodeType { get; } = NodeType.ActionPlayAudio;

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["PositionNodeIndex"] =
                GetConnectedNodeIndex(Constants.IN_VALUE_PORT_NAME, nodeToRuntimeIndex);

            var option = GetNodeOptionByName(Constants.AUDIO_NAME_OPTION_NAME);
            if (option != null && option.TryGetValue<string>(out var audioName))
            {
                node.Properties["Name"] = audioName;
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(Constants.AUDIO_NAME_OPTION_NAME)
                .WithDisplayName("Audio Name")
                .Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort<Vector3>(Constants.IN_VALUE_PORT_NAME)
                .WithDisplayName("Position")
                .Build();
        }
    }
}
#endif