using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization
{
    public class VisualScriptRuntimeGraph : ScriptableObject
    {
        public List<SerializableNode> Nodes = new();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}