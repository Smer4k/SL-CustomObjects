using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes
{
    public abstract class ConditionNode : BaseNode
    {
        public override void OnEnable()
        {
            base.OnEnable();
            DefaultColor = NodeColors.ConditionColor;
        }

        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
            int portA;
            int portB;
            if (this is IDynamicNode)
            {
                portA = GetConnectedNodeIndex(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_A_PORT_NAME,
                    nodeToRuntimeIndex);
                portB = GetConnectedNodeIndex(Constants.DYNAMIC_PORT_PREFIX + Constants.IN_B_PORT_NAME,
                    nodeToRuntimeIndex);
            }
            else
            {
                portA = GetConnectedNodeIndex(Constants.IN_A_PORT_NAME, nodeToRuntimeIndex);
                portB = GetConnectedNodeIndex(Constants.IN_B_PORT_NAME, nodeToRuntimeIndex);
            }

            if (portA != -1)
            {
                node.Properties["ANodeIndex"] = portA;
            }

            if (portB != -1)
            {
                node.Properties["BNodeIndex"] = portB;
            }
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<bool>(Constants.OUT_RESULT_PORT_NAME)
                .WithDisplayName("Result")
                .Build();
        }
    }
}