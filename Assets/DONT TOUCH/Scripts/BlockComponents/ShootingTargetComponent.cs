using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
	[ExecuteInEditMode, SelectionBase]
	public class ShootingTargetComponent : SchematicBlock
	{
		public override bool RequiredUniqName { get; } = true;
		public override BlockType BlockType { get; } = BlockType.ShootingTarget;
		public TargetType TargetType;
	
		public override void Compile(SchematicBlockData block)
		{
			block.Properties = new Dictionary<string, object>()
			{
				{ nameof(TargetType), TargetType },
			};
			base.Compile(block);
		}

		public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
		{
			TargetType targetType = (TargetType)Convert.ToInt32(block.Properties["TargetType"]);
			ShootingTargetComponent shootingTarget = Create<ShootingTargetComponent>($"Assets/Resources/Blocks/ShootingTargets/{targetType}.prefab");
			gameObject = shootingTarget.gameObject;
			base.Decompile(ref gameObject, block, parent);
		}
	}
}