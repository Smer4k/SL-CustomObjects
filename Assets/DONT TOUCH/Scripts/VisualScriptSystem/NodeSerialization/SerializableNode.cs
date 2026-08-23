using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization
{
    [Serializable]
    public sealed class SerializableNode
    {
        public int NodeIndex;
        public int NextNodeIndex = -1;
        public NodeType NodeType;
        public DataType DataType = DataType.None;
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}