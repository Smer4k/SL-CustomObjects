using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents.Locker
{
	[ExecuteInEditMode, SelectionBase]
	public class LockerComponent : SchematicBlock
	{
		public override bool RequiredUniqName { get; } = true;
		public List<LockerChamber> Chambers = new();
		public List<LockerItem> Loot = new();
		public LockerType LockerType;
		public override BlockType BlockType => BlockType.Locker;
		public override bool RequiredUniqName { get; } = true;

		public override void Compile(SchematicBlockData block)
		{
			block.BlockType = BlockType.Locker;
			List<string> jsonLoot = new(Loot.Count);
			List<string> jsonChamber = new(Chambers.Count);

			foreach (var chamber in Chambers)
			{
				jsonChamber.Add(JsonConvert.SerializeObject(chamber));
			}

			foreach (var loot in Loot)
			{
				jsonLoot.Add(JsonConvert.SerializeObject(loot));
			}

			block.Properties = new Dictionary<string, object>()
			{
				{ "LockerType", LockerType },
				{ "Chambers", jsonChamber },
				{ "Loot", jsonLoot },
			};

			base.Compile(block);
		}

		public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
		{
			LockerType lockerType = (LockerType)Convert.ToInt32(block.Properties["LockerType"]);
			if (lockerType is LockerType.PedestalScp018 or LockerType.PedestalScp207 or LockerType.PedestalScp244
			    or LockerType.PedestalScp268 or LockerType.PedestalScp1853 or LockerType.PedestalScp2176
			    or LockerType.PedestalScpScp1576 or LockerType.PedestalAntiScp207 or LockerType.PedestalScp1344)
			{
				lockerType = LockerType.PedestalScp500;
			}
			LockerComponent locker = Create<LockerComponent>($"Assets/Resources/Blocks/Lockers/{lockerType}.prefab");
			gameObject = locker.gameObject;
		
			if (LockerType is LockerType.PedestalScp500)
			{
				locker.LockerType = (LockerType)Convert.ToInt32(block.Properties["LockerType"]);
			}
		
			var tokenLoot = block.Properties["Loot"] as JArray;
			List<string> jsonLoot = tokenLoot != null
				? tokenLoot.ToObject<List<string>>()
				: block.Properties["Loot"] as List<string>;
		
			var tokenChambers = block.Properties["Chambers"] as JArray;
			List<string> jsonChambers = tokenChambers != null
				? tokenChambers.ToObject<List<string>>()
				: block.Properties["Chambers"] as List<string>;
		
			locker.Chambers.Clear();
			locker.Loot.Clear();
		
			foreach (var chamber in jsonChambers)
			{
				locker.Chambers.Add(JsonConvert.DeserializeObject<LockerChamber>(Convert.ToString(chamber)));
			}
		
			foreach (var loot in jsonLoot)
			{
				locker.Loot.Add(JsonConvert.DeserializeObject<LockerItem>(Convert.ToString(loot)));
			}

			base.Decompile(ref gameObject, block, parent);
		}
	}
}