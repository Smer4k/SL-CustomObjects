using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Player.Get
{
    [Serializable]
    [Node("Player/Get", "", "Player id")]
    public sealed class GetPlayerIdNode : GetNode
    {
        public override NodeType NodeType { get; } = NodeType.GetPlayerId;
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddOutputValuePort(context, typeof(int));
        }
    }
}