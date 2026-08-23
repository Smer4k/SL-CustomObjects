using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockComponents;
using DONT_TOUCH.Scripts.Extensions;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockSerialization
{
    public interface IActionEventHost
    {
        List<ActionEventList> ActionEvents { get; set; }
        void EnsureActionEventsInitialized();
    }

    public static class ActionEventSerialization
    {
        public static void PrepareHostForCompile(IActionEventHost actionEventHost)
        {
            if (actionEventHost == null)
                return;

            actionEventHost.EnsureActionEventsInitialized();
            PrepareForCompile(actionEventHost.ActionEvents);
        }

        public static List<ActionEventList> ReadHostEventListsFromProperties(
            IActionEventHost actionEventHost,
            Dictionary<string, object> properties,
            string key)
        {
            if (actionEventHost == null)
                return new List<ActionEventList>();

            actionEventHost.ActionEvents = ReadEventListsFromProperties(properties, key);
            actionEventHost.EnsureActionEventsInitialized();
            return actionEventHost.ActionEvents;
        }

        public static void EnsureEventLists(List<ActionEventList> eventLists)
        {
            if (eventLists == null)
                return;

            for (int i = eventLists.Count - 1; i >= 0; i--)
            {
                ActionEventList eventList = eventLists[i];
                if (eventList == null)
                {
                    eventLists.RemoveAt(i);
                    continue;
                }

                eventList.EnsureDefaults(i + 1);
                NormalizeActions(eventList.Actions);
            }
        }

        public static void PrepareForCompile(List<ActionEventList> eventLists)
        {
            EnsureEventLists(eventLists);

            if (eventLists == null)
                return;

            foreach (ActionEventList eventList in eventLists)
            {
                if (eventList?.Actions == null)
                    continue;

                foreach (ActionGame action in eventList.Actions)
                {
                    if (action == null)
                        continue;

                    // For Animation type, resolve TargetId and ParamType
                    if (action.Type == ActionType.Animation)
                    {
                        action.TargetId = action.Target != null
                            ? action.Target.GetId()
                            : 0;

                        var resolvedType = ResolveAnimatorParamType(action.Target, action.Param);
                        if (resolvedType != default)
                            action.ParamType = resolvedType;
                    }

                    // For SetComponentProperty type, resolve TargetId
                    if (action.Type == ActionType.SetComponentProperty)
                    {
                        action.TargetId = action.Target != null
                            ? action.Target.GetId()
                            : 0;
                        if (action.TargetId != 0 && action.Target.TryGetComponent(out SchematicBlock block))
                        {
                            action.BlockType = block.BlockType;
                        }
                    }

                    if (action.Type == ActionType.Destroy)
                    {
                        action.TargetId = action.Target != null
                            ? action.Target.GetId()
                            : 0;
                    }

                    // Clear irrelevant parameters for all types
                    action.EnsureDefaults();
                }
            }
        }

        public static List<ActionEventList> ReadEventListsFromProperties(Dictionary<string, object> properties, string key)
        {
            if (properties == null || !properties.TryGetValue(key, out object eventListsObject))
                return new List<ActionEventList>();

            return ReadEventListsFromObject(eventListsObject);
        }

        public static List<ActionEventList> ReadEventListsFromObject(object eventListsObject)
        {
            List<ActionEventList> eventLists = (eventListsObject as JArray)?.ToObject<List<ActionEventList>>() ??
                                               eventListsObject as List<ActionEventList> ??
                                               new List<ActionEventList>();

            EnsureEventLists(eventLists);
            return eventLists;
        }

        public static IEnumerable<ActionGame> EnumerateActions(List<ActionEventList> eventLists)
        {
            if (eventLists == null)
                yield break;

            foreach (ActionEventList eventList in eventLists)
            {
                if (eventList?.Actions == null)
                    continue;

                foreach (ActionGame action in eventList.Actions)
                    yield return action;
            }
        }

#if UNITY_6000_5_OR_NEWER
        public static void RebindTargets(List<ActionEventList> eventLists, IReadOnlyDictionary<long, Transform> objectFromId)
#else
        public static void RebindTargets(List<ActionEventList> eventLists, IReadOnlyDictionary<int, Transform> objectFromId)
#endif
        {
            if (objectFromId == null)
                return;

            foreach (ActionGame action in EnumerateActions(eventLists))
            {
                if (action == null || action.TargetId == 0)
                    continue;

                // For Animation and SetComponentProperty types, restore Target from TargetId
                if (action.Type is ActionType.Animation or ActionType.SetComponentProperty or ActionType.Destroy)
                {
                    if (objectFromId.TryGetValue(action.TargetId, out Transform targetTransform))
                        action.Target = targetTransform.gameObject;
                }
            }
        }

        private static AnimatorControllerParameterType ResolveAnimatorParamType(GameObject target, string paramName)
        {
            if (target == null || string.IsNullOrEmpty(paramName))
                return default;

            // NO NEED to search in child or parent objects.
            Animator animator = target.GetComponent<Animator>();

            if (animator == null)
                return default;

            AnimatorControllerParameter[] parameters = animator.parameters;

#if UNITY_EDITOR
            if (parameters == null || parameters.Length == 0)
            {
                RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
                if (runtimeController is AnimatorOverrideController overrideController)
                    runtimeController = overrideController.runtimeAnimatorController;

                if (runtimeController is UnityEditor.Animations.AnimatorController animatorController)
                    parameters = animatorController.parameters;
            }
#endif

            if (parameters == null || parameters.Length == 0)
                return default;

            foreach (AnimatorControllerParameter parameter in parameters)
            {
                if (parameter.name == paramName)
                    return parameter.type;
            }

            return default;
        }

        private static void NormalizeActions(List<ActionGame> actions)
        {
            if (actions == null)
                return;

            for (int i = actions.Count - 1; i >= 0; i--)
            {
                ActionGame action = actions[i];
                if (action == null)
                {
                    actions.RemoveAt(i);
                    continue;
                }

                action.EnsureDefaults();
            }
        }
    }
}