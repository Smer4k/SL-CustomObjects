using UnityEngine;

namespace DONT_TOUCH.Scripts.Extensions
{
    public static class EntityIdExtensions
    {
        public static ulong GetId(this EntityId entityId)
        {
            return EntityId.ToULong(entityId);
        }

        public static ulong GetId(this Object target)
        {
            return EntityId.ToULong(target.GetEntityId());
        }
    }
}