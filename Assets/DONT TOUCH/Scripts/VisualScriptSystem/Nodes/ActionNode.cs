namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes
{
    public abstract class ActionNode : BaseNode
    {
        public override void OnEnable()
        {
            base.OnEnable();
            DefaultColor = NodeColors.ActionColor;
        }
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
        }
    }
}