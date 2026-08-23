using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.Extensions;
using UnityEngine;
using CameraType = DONT_TOUCH.Enums.CameraType;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    public class Scp079CameraTransferComponent : SchematicBlock
    {
        public override BlockType BlockType { get; } = BlockType.CameraTransfer;
        public Scp079CameraComponent TargetCamera;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>();
            if (TargetCamera != null)
            {
                block.Properties[nameof(CameraType)] = TargetCamera.CameraType;
                block.Properties[nameof(TargetCamera)] = TargetCamera.transform.GetId();
            }
            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            var cameraTransfer = Create<Scp079CameraTransferComponent>("Assets/Resources/Blocks/Cameras/CameraTransfer.prefab");
            gameObject = cameraTransfer.gameObject;
            base.Decompile(ref gameObject, block, parent);
        }
    }
}