using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Player.Get
{
    [Serializable]
    [Node("Player/Get", "", "Player health")]
    public sealed class GetPlayerHealthNode : GetNode
    {
        public override NodeType NodeType { get; } = NodeType.GetPlayerHealth;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddOutputValuePort(context, typeof(float));
        }
    }
}