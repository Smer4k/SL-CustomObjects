#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Condition
{
    [Serializable]
    [Node("Condition", "", "AND")]
    public sealed class AndNode : ConditionNode
    {
        public override NodeType NodeType { get; } = NodeType.ConditionAnd;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort<bool>(Constants.IN_A_PORT_NAME)
                .WithDisplayName("A")
                .WithDefaultValue(false)
                .Build();

            context.AddInputPort<bool>(Constants.IN_B_PORT_NAME)
                .WithDisplayName("B")
                .WithDefaultValue(false)
                .Build();
        }
    }
}
#endif