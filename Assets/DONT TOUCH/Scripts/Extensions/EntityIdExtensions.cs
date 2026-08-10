using UnityEngine;

namespace DONT_TOUCH.Scripts.Extensions
{
    public static class EntityIdExtensions
    {
#if UNITY_6000_5_OR_NEWER
        public static long GetId(this EntityId entityId)
        {
            return (long)EntityId.ToULong(entityId);
        }

        public static long GetId(this Object target)
        {
            return (long)EntityId.ToULong(target.GetEntityId());
        }
#else
        public static int GetId(this Object target)
        {
            return target.GetInstanceID();
        }
#endif
    }
}