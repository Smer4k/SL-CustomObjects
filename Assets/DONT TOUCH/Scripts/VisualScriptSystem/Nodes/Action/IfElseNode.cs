#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Action
{
    [Serializable]
    [Node("Action", "", "If else")]
    public sealed class IfElseNode : ActionNode
    {
        public override NodeType NodeType { get; } = NodeType.ActionIfElse;

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            node.Properties["ConditionNodeIndex"] =
                GetConnectedNodeIndex(Constants.IN_CONDITION_PORT_NAME, nodeToRuntimeIndex);
            node.Properties["TrueNextNodeIndex"] = GetNextNodeIndex(Constants.OUT_TRUE_PORT_NAME, nodeToRuntimeIndex);
            node.Properties["FalseNextNodeIndex"] = GetNextNodeIndex(Constants.OUT_FALSE_PORT_NAME, nodeToRuntimeIndex);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(Constants.EXECUTION_PORT_IN_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            // Данные: булево условие, к которому можно подключить провод
            context.AddInputPort<bool>(Constants.IN_CONDITION_PORT_NAME)
                .WithDisplayName("Condition")
                .WithDefaultValue(false)
                .Build();

            // Два выходных execution-порта — две ветки выполнения
            context.AddOutputPort(Constants.OUT_TRUE_PORT_NAME)
                .WithDisplayName("True")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(Constants.OUT_FALSE_PORT_NAME)
                .WithDisplayName("False")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
#endif