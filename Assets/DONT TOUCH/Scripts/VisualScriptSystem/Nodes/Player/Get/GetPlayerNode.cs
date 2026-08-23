#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.Types;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Player.Get
{
    [Serializable]
    [Node("Player/Get", "", "Player")]
    public sealed class GetPlayerNode : GetNode
    {
        public override NodeType NodeType { get; } = NodeType.GetPlayer;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddOutputValuePort(context, typeof(PlayerType));
        }
    }
}
#endif