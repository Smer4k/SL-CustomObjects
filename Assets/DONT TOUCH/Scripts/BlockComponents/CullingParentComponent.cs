using DONT_TOUCH.Enums;
using Newtonsoft.Json;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class CullingParentComponent : SchematicBlock
    {
        public override BlockType BlockType => BlockType.CullingParent;
        public Vector3 BoundsSize = new(1, 1, 1);

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new()
            {
                { "BoundsSize", new SerializableVector(BoundsSize) },
            };
            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            CullingParentComponent cullingParent =
                Create<CullingParentComponent>("Assets/Resources/Blocks/CullingParent.prefab");
            gameObject = cullingParent.gameObject;

            if (block.Properties.TryGetValue("BoundsSize", out object boundsSizeObj))
            {
                SerializableVector boundsSize =
                    JsonConvert.DeserializeObject<SerializableVector>(boundsSizeObj.ToString());
                if (boundsSize != null)
                    cullingParent.BoundsSize = (Vector3)boundsSize;
            }

            base.Decompile(ref gameObject, block, parent);
        }

        public void OnDrawGizmos()
        {
            if (transform.childCount == 1 && transform.GetChild(0).TryGetComponent(out LightComponent _))
            {
                Gizmos.color = new Color(0.48f, 1, 0, 0.5f);
                Gizmos.DrawCube(transform.position, BoundsSize / 3);
            }
            else
            {
                Gizmos.color = new Color(0, 1, 0.48f, 0.5f);
                Gizmos.DrawCube(transform.position, BoundsSize);
            }
        }
    }
}