using System.Collections.Generic;
using UnityEngine;

namespace DONT_TOUCH.Scripts
{
    public static class SchematicIdAllocator
    {
        public static void BakeAll(Schematic root)
        {
            var nextId = 1;
            Assign(root.transform, ref nextId);
        }
        
        public static void Assign(Transform t, ref int nextId)
        {
            var block = t.GetComponent<SchematicBlock>();
            if (block == null)
                return;
            block.ObjectId = nextId++;

            for (var i = 0; i < t.childCount; i++)
                Assign(t.GetChild(i), ref nextId);
        }
        
        public static int GetOrAssignId(Schematic root, SchematicBlock target)
        {
            if (target.ObjectId > 0)
                return target.ObjectId;

            var used = new HashSet<int>();
            CollectUsedIds(root.transform, used);

            var candidate = 1;
            while (used.Contains(candidate))
                candidate++;

            target.ObjectId = candidate;
            return candidate;
        }
        
        public static void CollectUsedIds(Transform t, HashSet<int> used)
        {
            var id = t.GetComponent<SchematicBlock>();
            if (id != null && id.ObjectId > 0)
                used.Add(id.ObjectId);

            for (var i = 0; i < t.childCount; i++)
                CollectUsedIds(t.GetChild(i), used);
        }
    }
}