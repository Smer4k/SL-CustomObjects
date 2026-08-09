using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode, SelectionBase]
    public sealed class GeneratorComponent : SchematicBlock
    {
        public override BlockType BlockType { get; } = BlockType.Generator;
        public GeneratorFlags GeneratorFlags;
        public DoorPermissionFlags RequiredPermissions = DoorPermissionFlags.ArmoryLevelTwo;
        public float TotalActivationTime = 125;
        public float TotalDeactivationTime = 125;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>()
            {
                { nameof(GeneratorFlags), GeneratorFlags },
                { nameof(RequiredPermissions), RequiredPermissions },
                { nameof(TotalActivationTime), TotalActivationTime  },
                { nameof(TotalDeactivationTime), TotalDeactivationTime },
            };
            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            GeneratorComponent generator = Create<GeneratorComponent>($"Assets/Resources/Blocks/Generator.prefab");
            gameObject = generator.gameObject;

            if (block.Properties.TryGetValue(nameof(GeneratorFlags), out object generatorFlagsObj))
            {
                generator.GeneratorFlags = (GeneratorFlags)Convert.ToByte(generatorFlagsObj);
            }

            if (block.Properties.TryGetValue(nameof(RequiredPermissions), out object requiredPermissionsObj))
            {
                generator.RequiredPermissions = (DoorPermissionFlags)Convert.ToUInt16(requiredPermissionsObj);
            }

            if (block.Properties.TryGetValue(nameof(TotalActivationTime), out object totalActivationTimeObj))
            {
                generator.TotalActivationTime = Convert.ToSingle(totalActivationTimeObj);
            }

            if (block.Properties.TryGetValue(nameof(TotalDeactivationTime), out object totalDeactivationTimeObj))
            {
                generator.TotalDeactivationTime = Convert.ToSingle(totalDeactivationTimeObj);
            }
            
            base.Decompile(ref gameObject, block, parent);
        }
    }
}