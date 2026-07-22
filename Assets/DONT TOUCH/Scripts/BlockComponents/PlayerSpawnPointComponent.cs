using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
	[ExecuteInEditMode, SelectionBase]
	public class PlayerSpawnPointComponent : SchematicBlock
	{
		public override bool RequiredUniqName { get; } = true;
		public override BlockType BlockType { get; } = BlockType.PlayerSpawnPoint;
		public List<DefaultRoleTypeId> Roles = new();
	
		public override void Compile(SchematicBlockData block)
		{
			block.Properties = new Dictionary<string, object>()
			{
				{ nameof(Roles), Roles },
			};
			base.Compile(block);
		}

		public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
		{
			PlayerSpawnPointComponent spawnPoint = Create<PlayerSpawnPointComponent>("Assets/Resources/Blocks/SpawnPoint.prefab");
			gameObject = spawnPoint.gameObject;
			foreach (var role in ((JArray)block.Properties["Roles"]).ToObject<List<DefaultRoleTypeId>>())
			{
				spawnPoint.Roles.Add(role);
			}		
			base.Decompile(ref gameObject, block, parent);
		}
	}
}