using System;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes
{
    public interface IDynamicNode : INode
    {
        public Type PortType { get; set; }
    }
}