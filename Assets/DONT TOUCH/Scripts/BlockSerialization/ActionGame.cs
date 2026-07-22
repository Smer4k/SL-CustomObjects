using System;
using DONT_TOUCH.Enums;
using Newtonsoft.Json;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockSerialization
{
    [Serializable]
    public class ActionGame
    {
        public ActionType Type;
        public float ActionDelay;
        public string Value = string.Empty;

        [JsonIgnore] public GameObject Target;
        public BlockType BlockType;
        [JsonConverter(typeof(UncheckedULongConverter))]
        public ulong TargetId;
        public string Param = string.Empty;
        public AnimatorControllerParameterType ParamType;

        [JsonIgnore] public bool EditorIsExpanded = true;
        [JsonIgnore] public string Name = string.Empty;

        public void EnsureDefaults()
        {
            Value ??= string.Empty;
            Param ??= string.Empty;
            ActionDelay = Mathf.Max(0f, ActionDelay);

            // Clear parameters that are not relevant for the current action type
            switch (Type)
            {
                case ActionType.Command:
                    // Command and Audio use Value, clear the rest
                    TargetId = 0;
                    Param = string.Empty;
                    ParamType = default;
                    BlockType = default;
                    break;

                case ActionType.Animation:
                    // Animation uses Target, Param, ParamType, Value
                    BlockType = default;
                    break;

                case ActionType.SetComponentProperty:
                    // SetComponentProperty uses Target and Param (property name), Value (new value)
                    // TargetId is filled during compilation from Target
                    ParamType = default;
                    break;
                case ActionType.Destroy:
                    ParamType = default;
                    BlockType = default;
                    Value = string.Empty;
                    Param = string.Empty;
                    break;
            }
        }
    }
}