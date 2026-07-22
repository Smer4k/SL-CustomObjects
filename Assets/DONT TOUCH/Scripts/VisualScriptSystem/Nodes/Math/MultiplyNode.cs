using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Math
{
    [Serializable]
    [Node("Math", "", "Multiply")]
    public sealed class MultiplyNode : MathNode
    {
        public override NodeType NodeType { get; } = NodeType.MathMultiply;
    }
}