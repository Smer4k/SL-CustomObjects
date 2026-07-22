using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;
using CameraType = DONT_TOUCH.Enums.CameraType;

namespace DONT_TOUCH.Scripts.BlockComponents
{
	[ExecuteInEditMode, SelectionBase]
	public class Scp079CameraComponent : SchematicBlock
	{
		public override bool RequiredUniqName { get; } = true;
		public override BlockType BlockType { get; } = BlockType.Camera;
		public CameraType CameraType;
		public string Label;

		public override void Compile(SchematicBlockData block)
		{
			block.Properties = new Dictionary<string, object>()
			{
				{ nameof(CameraType), CameraType },
				{ nameof(Label), Label }
			};
			base.Compile(block);
		}

		public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
		{
			CameraType cameraType = (CameraType)Convert.ToInt32(block.Properties["CameraType"]);
			Scp079CameraComponent camera = Create<Scp079CameraComponent>($"Assets/Resources/Blocks/Cameras/{cameraType}.prefab");
			gameObject = camera.gameObject;
			camera.Label = Convert.ToString(block.Properties["Label"]);
			base.Decompile(ref gameObject, block, parent);
		}
	}
}