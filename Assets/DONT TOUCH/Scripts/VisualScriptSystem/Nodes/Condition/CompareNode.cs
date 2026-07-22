using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Condition
{
    [Serializable]
    [Node("Condition", "", "COMPARE")]
    public sealed class CompareNode : ConditionNode, IDynamicNode
    {
        public Type PortType { get; set; } = typeof(object);
        public override NodeType NodeType { get; } = NodeType.ConditionCompare;
        
        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            var option = GetNodeOptionByName(Constants.COMPARE_TYPE_OPTION_NAME);
            if (option != null && option.TryGetValue(out CompareOperation compareType))
            {
                node.Properties["CompareType"] = compareType;
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<CompareOperation>(Constants.COMPARE_TYPE_OPTION_NAME)
                .WithDisplayName("Operation")
                .WithDefaultValue(CompareOperation.Equal);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddInputPort(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_A_PORT_NAME).WithDataType(PortType)
                .WithDisplayName("A").Build();
            context.AddInputPort(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_B_PORT_NAME).WithDataType(PortType)
                .WithDisplayName("B").Build();
        }
    }
}