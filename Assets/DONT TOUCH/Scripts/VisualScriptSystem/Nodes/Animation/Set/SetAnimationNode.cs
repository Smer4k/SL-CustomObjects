#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Animation.Set
{
    [Serializable]
    [Node("Animation/Set", "", "Set Animation")]
    public sealed class SetAnimationNode : SetNode
    {
        public override NodeType NodeType { get; } = NodeType.SetAnimation;
        
        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["TargetNodeIndex"] =
                GetConnectedNodeIndex(Constants.IN_VALUE_PORT_NAME, nodeToRuntimeIndex);
            var option = GetNodeOptionByName(Constants.ANIMATION_OPTION_NAME);
            if (option != null && option.TryGetValue<string>(out var animationName))
            {
                node.Properties["Name"] = animationName;
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(Constants.ANIMATION_OPTION_NAME)
                .WithDisplayName("Name")
                .WithTooltip("Animation name")
                .WithDefaultValue(string.Empty);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            AddInputSetValuePort(context, typeof(GameObject));
        }
    }
}
#endif