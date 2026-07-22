using Unity.GraphToolkit.Editor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes
{
    public abstract class EventNode : BaseNode
    {
        public override void OnEnable()
        {
            base.OnEnable();
            DefaultColor = NodeColors.EventColor;
        }
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort(Constants.EXECUTION_PORT_OUT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}