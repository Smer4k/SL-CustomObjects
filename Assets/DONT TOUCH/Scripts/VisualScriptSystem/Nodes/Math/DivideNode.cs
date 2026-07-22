using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Math
{
    [Serializable]
    [Node("Math", "", "Divide")]
    public sealed class DivideNode : MathNode
    {
        public override NodeType NodeType { get; } = NodeType.MathDivide;
        
        public override void CheckErrors(List<string> msg)
        {
            base.CheckErrors(msg);
            if (PortType == typeof(string))
            {
                msg.Add($"Type \"{PortType}\" does not support division.");
            }
        }
    }
}