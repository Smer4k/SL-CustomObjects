#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Player.Get
{
    [Serializable]
    [Node("Player/Get", "", "Player current item")]
    public sealed class GetPlayerCurrentItemNode : GetNode
    {
        public override NodeType NodeType { get; } = NodeType.GetPlayerCurrentItem;
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddOutputValuePort(context, typeof(ItemType));
        }
    }
}
#endif