#if UNITY_6000_5_OR_NEWER && UNITY_EDITOR
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
#endif