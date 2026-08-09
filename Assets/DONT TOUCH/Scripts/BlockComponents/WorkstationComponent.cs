using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode, SelectionBase]
    public class WorkstationComponent : SchematicBlock
    {
        public override bool RequiredUniqName { get; } = true;
        [Tooltip("Whether the workstation should be interactable in game. If set to false it won't be enabled in game.")]
        public bool IsInteractable = true;
        public override bool RequiredUniqName { get; } = true;
        public override BlockType BlockType => BlockType.Workstation;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>
            {
                { "IsInteractable", IsInteractable },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            WorkstationComponent workstationComponent = Create<WorkstationComponent>("Assets/Resources/Blocks/Workstation.prefab");
            gameObject = workstationComponent.gameObject;

            workstationComponent.IsInteractable = block.Properties.TryGetValue("IsInteractable", out object isInteractable) && Convert.ToBoolean(isInteractable);

            base.Decompile(ref gameObject, block, parent);
        }
    }
}

