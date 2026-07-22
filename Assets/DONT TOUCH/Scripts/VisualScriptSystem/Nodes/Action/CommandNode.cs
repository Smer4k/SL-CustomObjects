using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Action
{
    [Serializable]
    [Node("Action", "", "Command")]
    public sealed class CommandNode : ActionNode
    {
        public override NodeType NodeType { get; } = NodeType.ActionCommand;

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            var option = GetNodeOptionByName(Constants.COMMAND_OPTION_NAME);
            if (option != null && option.TryGetValue<string>(out var command))
            {
                node.Properties["Command"] = command;
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(Constants.COMMAND_OPTION_NAME)
                .WithDisplayName("Command")
                .WithDefaultValue(string.Empty);
        }
    }
}