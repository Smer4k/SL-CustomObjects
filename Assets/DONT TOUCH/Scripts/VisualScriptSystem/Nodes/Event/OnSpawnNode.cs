using System;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.Types;
using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes.Event
{
    [Serializable]
    [Node("Event", "", "On Spawn")]
    public sealed class OnSpawnNode : EventNode
    {
        public override NodeType NodeType { get; } = NodeType.EventSpawn;

        
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            context.AddOutputPort<PlayerType>(Constants.OUT_VALUE_PORT_NAME)
                .WithDisplayName("Player")
                .Build();
        }
    }
}