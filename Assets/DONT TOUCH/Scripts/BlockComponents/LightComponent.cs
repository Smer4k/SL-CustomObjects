using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode, SelectionBase]
    public class LightComponent : ActionEventHostBlockBase
    {
        public override BlockType BlockType => BlockType.Light;
        public override List<ActionEventList> CreateDefaultActionEvents()
        {
            return new List<ActionEventList>()
            {
                new("TurnOn", "Turn On"),
                new("TurnOff", "Turn Off"),
            };
        }

        [Tooltip("Will the light turn off when the facility lights are turned off?")]
        public bool Flicker;

        public DefaultFacilityZone FlickerZone;
        public bool Cycle = false;

        [Header("Time")] public bool RandomInRange = false;
        [Min(0f)] public float TimeToOn = 1f;
        [Min(0f)] public float TimeToOff = 2f;
        [Header("Time to On"), Min(0)] public float MaxOn = 3f;
        [Min(0f)] public float MinOn = 2f;
        [Header("Time to Off"), Min(0f)] public float MaxOff = 3f;
        [Min(0f)] public float MinOff = 2f;

        [HideInInspector] public LightType LightType;
        [HideInInspector] public float Intensity;
        [HideInInspector] public float Range;
        [HideInInspector] public float ShadowStrength;
        [HideInInspector] public LightShadows LightShadows;
        [HideInInspector] public float SpotAngle;
        [HideInInspector] public float InnerSpotAngle;
        [HideInInspector] public Color Color;

        public void Update()
        {
            if (MaxOff < MinOff)
            {
                MaxOff = MinOff;
            }

            if (MaxOn < MinOn)
            {
                MaxOn = MinOn;
            }
        }

        public override void Compile(SchematicBlockData block)
        {
            PrepareActionEventsForCompile();
            TryGetComponent(out Light light);

            block.Properties = new Dictionary<string, object>
            {
                { "LightType", light.type },
                { "Color", ColorUtility.ToHtmlStringRGBA(light.color) },
                { "Intensity", light.intensity },
                { "Range", light.range },
                { "Shape", light.shape },
                { "SpotAngle", light.spotAngle },
                { "InnerSpotAngle", light.innerSpotAngle },
                { "ShadowStrength", light.shadowStrength },
                { "ShadowType", light.shadows },
                { nameof(Flicker), Flicker },
                { nameof(FlickerZone), FlickerZone },
                { nameof(Cycle), Cycle },
                { "RandomInRange", RandomInRange },
            };
            
            if (RandomInRange)
            {
                block.Properties["MaxOn"] = MaxOn;
                block.Properties["MinOn"] = MinOn;
                block.Properties["MaxOff"] = MaxOff;
                block.Properties["MinOff"] = MinOff;
            }
            else
            {
                block.Properties["TimeToOn"] = TimeToOn;
                block.Properties["TimeToOff"] = TimeToOff;
            }

            block.Properties[nameof(ActionEvents)] = ActionEvents;
            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            var lightType = block.Properties.TryGetValue("LightType", out object objLightType)
                ? (LightType)Convert.ToInt32(objLightType)
                : LightType.Point;
            
            if ((int)lightType == 3)
                lightType = LightType.Rectangle;
            
            var lightComponent = Create<LightComponent>($"Assets/Resources/Blocks/Lights/{lightType} Light.prefab");
            var light = lightComponent.GetComponent<Light>();
            gameObject = light.gameObject;

            light.color = PrimitiveComponent.GetColorFromString(block.Properties["Color"].ToString());
            light.intensity = Convert.ToSingle(block.Properties["Intensity"]);
            light.range = Convert.ToSingle(block.Properties["Range"]);

            if (block.Properties.TryGetValue("Shadows", out object shadows))
            {
                // Backward compatibility
                light.shadows = Convert.ToBoolean(shadows) ? LightShadows.Soft : LightShadows.None;
            }
            else
            {
                light.shadows = (LightShadows)Convert.ToInt32(block.Properties["ShadowType"]);
                light.shape = (LightShape)Convert.ToInt32(block.Properties["Shape"]);
                light.spotAngle = Convert.ToSingle(block.Properties["SpotAngle"]);
                light.innerSpotAngle = Convert.ToSingle(block.Properties["InnerSpotAngle"]);
                light.shadowStrength = Convert.ToSingle(block.Properties["ShadowStrength"]);
            }

            if (block.Properties.TryGetValue(nameof(Flicker), out object flickerEnable))
            {
                Flicker = Convert.ToBoolean(flickerEnable);
            }

            if (block.Properties.TryGetValue(nameof(FlickerZone), out object flickerZone))
            {
                FlickerZone = (DefaultFacilityZone)Convert.ToInt32(flickerZone);
            }

            if (block.Properties.TryGetValue(nameof(Cycle), out var obj))
            {
                Cycle = Convert.ToBoolean(obj);
            }

            if (block.Properties.TryGetValue(nameof(RandomInRange), out obj))
            {
                RandomInRange = Convert.ToBoolean(obj);
            }

            if (RandomInRange)
            {
                if (block.Properties.TryGetValue(nameof(MaxOn), out obj))
                {
                    MaxOn = Convert.ToSingle(obj);
                }

                if (block.Properties.TryGetValue(nameof(MinOn), out obj))
                {
                    MinOn = Convert.ToSingle(obj);
                }

                if (block.Properties.TryGetValue(nameof(MaxOff), out obj))
                {
                    MaxOff = Convert.ToSingle(obj);
                }

                if (block.Properties.TryGetValue(nameof(MinOff), out obj))
                {
                    MinOff = Convert.ToSingle(obj);
                }
            }
            else
            {
                if (block.Properties.TryGetValue(nameof(TimeToOn), out obj))
                {
                    TimeToOn = Convert.ToSingle(obj);
                }

                if (block.Properties.TryGetValue(nameof(TimeToOff), out obj))
                {
                    TimeToOff = Convert.ToSingle(obj);
                }
            }
            lightComponent.ReadActionEventsFromProperties(block.Properties, nameof(ActionEvents));

            base.Decompile(ref gameObject, block, parent);
        }
    }
}