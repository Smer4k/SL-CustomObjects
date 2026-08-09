#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Math
{
    [Serializable]
    [Node("Math", "", "Subtract")]
    public sealed class SubtractNode : MathNode
    {
        public override NodeType NodeType { get; } = NodeType.MathSubtract;
        
        public override void CheckErrors(List<string> msg)
        {
            base.CheckErrors(msg);
            if (PortType == typeof(string))
            {
                msg.Add($"Type \"{PortType}\" does not support subtraction.");
            }
        }
    }
}
#endif