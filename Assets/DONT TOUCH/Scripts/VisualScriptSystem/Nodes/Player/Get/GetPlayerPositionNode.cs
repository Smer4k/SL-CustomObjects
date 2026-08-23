#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Player.Get
{
    [Serializable]
    [Node("Player/Get", "", "Player position")]
    public sealed class GetPlayerPositionNode : GetNode
    {
        public override NodeType NodeType { get; } = NodeType.GetPlayerPosition;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddOutputValuePort(context, typeof(Vector3));
        }
    }
}
#endif