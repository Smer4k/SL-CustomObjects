using System.Collections.Generic;
using DONT_TOUCH.Scripts.BlockSerialization;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    public abstract class ActionEventHostBlockBase : SchematicBlock, IActionEventHost
    {
        public List<ActionEventList> ActionEvents = new();

        List<ActionEventList> IActionEventHost.ActionEvents
        {
            get => ActionEvents;
            set => ActionEvents = value;
        }

        public abstract List<ActionEventList> CreateDefaultActionEvents();
        void IActionEventHost.EnsureActionEventsInitialized() => EnsureActionEventsInitialized();

        protected void PrepareActionEventsForCompile()
        {
            ActionEventSerialization.PrepareHostForCompile(this);
        }

        protected void ReadActionEventsFromProperties(Dictionary<string, object> properties, string key = nameof(ActionEvents))
        {
            ActionEventSerialization.ReadHostEventListsFromProperties(this, properties, key);
        }

        protected void EnsureActionEventsInitialized()
        {
            if (ActionEvents == null || ActionEvents.Count == 0)
                ActionEvents = CreateDefaultActionEvents();

            ActionEventSerialization.EnsureEventLists(ActionEvents);
        }

        protected override void Reset()
        {
            base.Reset();
            EnsureActionEventsInitialized();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            EnsureActionEventsInitialized();
        }
    }
}