using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
	[ExecuteInEditMode, SelectionBase]
	public class CapybaraComponent : SchematicBlock
	{
		public override bool RequiredUniqName { get; } = true;
		public override BlockType BlockType { get; } = BlockType.Capybara;
		public bool Collidable = true;
		
		public override void Compile(SchematicBlockData block)
		{
			block.Properties = new Dictionary<string, object>()
			{
				[nameof(Collidable)] = Collidable,
			};
			base.Compile(block);
		}

		public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
		{
			CapybaraComponent capybara = Create<CapybaraComponent>("Assets/Resources/Blocks/Capybara.prefab");
			gameObject = capybara.gameObject;
			if (block.Properties.TryGetValue(nameof(Collidable), out object collidableObj))
			{
				capybara.Collidable = Convert.ToBoolean(collidableObj);
			}
			base.Decompile(ref gameObject, block, parent);
		}
	}
}