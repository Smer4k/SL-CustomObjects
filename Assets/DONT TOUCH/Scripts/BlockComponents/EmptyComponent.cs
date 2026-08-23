using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using DONT_TOUCH.Scripts.Editors;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class EmptyComponent : ActionEventHostBlockBase
    {
        public override BlockType BlockType => BlockType.Empty;
        [Header("If enabled, all child objects can be damaged.")]
        public bool Damageable = false;

        [Min(0)] public float Health = 100f;

        [Header("Which explosion types can deal damage"), Tooltip("If the list is empty, none can"), SearchableEnum]
        public List<DefaultExplosionType> ExplosionTypes = new()
        {
            DefaultExplosionType.Grenade,
            DefaultExplosionType.SCP018,
            DefaultExplosionType.PinkCandy,
            DefaultExplosionType.Cola,
            DefaultExplosionType.Disruptor,
            DefaultExplosionType.Jailbird,
            DefaultExplosionType.Custom
        };
        [Header("Which weapons can deal damage (optional)"), Tooltip("If the list is empty, any weapon can"), SearchableEnum]
        public List<ItemType> Weapons = new();
        [Header("Which roles can deal damage (optional)"), Tooltip("If the list is empty, any role can"), SearchableEnum]
        public List<DefaultRoleTypeId> Roles = new();

        public override void Compile(SchematicBlockData block)
        {
            if (!Damageable)
            {
                base.Compile(block);
                return;
            }
            
            PrepareActionEventsForCompile();

            block.Properties = new()
            {
                { nameof(Damageable), Damageable },
                { nameof(Health), Health },
                { nameof(ExplosionTypes), ExplosionTypes },
                { nameof(Weapons), Weapons },
                { nameof(Roles), Roles },
                { nameof(ActionEvents), ActionEvents },
            };
            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            var emptyComponent = Create<EmptyComponent>("Assets/Resources/Blocks/Empty.prefab");
            gameObject = emptyComponent.gameObject;

            if (block.Properties.TryGetValue(nameof(Damageable), out var damageableObj))
            {
                emptyComponent.Damageable = Convert.ToBoolean(damageableObj);
            }

            if (block.Properties.TryGetValue(nameof(Health), out var healthObj))
            {
                emptyComponent.Health = Convert.ToSingle(healthObj);
            }

            if (block.Properties.TryGetValue(nameof(Weapons), out var weaponsObj))
            {
                foreach (var weapon in ((JArray)weaponsObj).ToObject<List<ItemType>>())
                {
                    emptyComponent.Weapons.Add(weapon);
                }
            }

            if (block.Properties.TryGetValue(nameof(ExplosionTypes), out var explosionTypesObj))
            {
                foreach (var explosionType in ((JArray)explosionTypesObj).ToObject<List<DefaultExplosionType>>())
                {
                    emptyComponent.ExplosionTypes.Add(explosionType);
                }
            }

            if (block.Properties.TryGetValue(nameof(Roles), out var rolesObj))
            {
                foreach (var role in ((JArray)rolesObj).ToObject<List<DefaultRoleTypeId>>())
                {
                    emptyComponent.Roles.Add(role);
                }
            }
            emptyComponent.ReadActionEventsFromProperties(block.Properties);

            base.Decompile(ref gameObject, block, parent);
        }

        public override List<ActionEventList> CreateDefaultActionEvents()
        {
            return new List<ActionEventList>
            {
                new("OnDamage", "On Damage"),
                new("OnDeath", "On Death"),
            };
        }
    }
}