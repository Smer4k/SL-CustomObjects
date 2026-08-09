using UnityEngine;

namespace DONT_TOUCH.Scripts.Extensions
{
    public static class EntityIdExtensions
    {
#if UNITY_6000_5_OR_NEWER
        public static ulong GetId(this EntityId entityId)
        {
            return EntityId.ToULong(entityId);
        }

        public static ulong GetId(this Object target)
        {
            return EntityId.ToULong(target.GetEntityId());
        }
#else
        public static int GetId(this Object target)
        {
            return target.GetInstanceID();
        }
#endif
    }
}