using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class TriggerComponent : ActionEventHostBlockBase
    {
        public PrimitiveType Type;

        public override BlockType BlockType => BlockType.Trigger;
        public TriggerTargetType TargetType;
        
        internal MeshFilter _filter;
        private MeshRenderer _renderer;
        private Material _sharedTransparent;
        private PrimitiveType? _prevType;   
    
        public override void Compile(SchematicBlockData block)
        {
            PrepareActionEventsForCompile();

            block.Properties = new Dictionary<string, object>
            {
                { "PrimitiveType", Type },
                { nameof(ActionEvents), ActionEvents },
                { "TargetType", TargetType },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            TriggerComponent trigger = Create<TriggerComponent>("Assets/Resources/Blocks/Trigger.prefab");
            gameObject = trigger.gameObject;

            trigger.Type = (PrimitiveType)Convert.ToInt32(block.Properties["PrimitiveType"]);
            trigger.ReadActionEventsFromProperties(block.Properties, nameof(ActionEvents));
            if (block.Properties.TryGetValue("TargetType", out var targetType))
            {
                trigger.TargetType = (TriggerTargetType)Convert.ToInt32(targetType);
            }

            base.Decompile(ref gameObject, block, parent);
        }

        private void Start()
        {
            TryGetComponent(out _filter);
            TryGetComponent(out _renderer);
            _sharedTransparent = new Material((Material)Resources.Load("Materials/Transparent"));
            _renderer.sharedMaterial = _sharedTransparent;
            _renderer.sharedMaterial.color = new Color(1, 1, 0, 0.1f);
        }

        private void Update()
        {
            if (_filter != null)
                _filter.hideFlags = HideFlags.HideInInspector;
            if (_renderer != null)
                _renderer.hideFlags = HideFlags.HideInInspector;
        
            if (Type is PrimitiveType.Quad or PrimitiveType.Plane)
                Type = PrimitiveType.Cube;
        
            if (_prevType == Type)
                return;
        
            _prevType = Type;
            _filter.sharedMesh = PrimitiveMeshGetter.GetPrimitiveMesh(Type);
        }

        public override List<ActionEventList> CreateDefaultActionEvents()
        {
            return new List<ActionEventList>
            {
                new("OnTriggerEnter", "On Enter"),
                new("OnTriggerExit", "On Exit"),
                new("OnTriggerStay", "While Inside"),
            };
        }
    }
}