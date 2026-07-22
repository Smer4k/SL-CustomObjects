using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.Types;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Enums
{
    /// <summary>
    /// Two-way mapping between runtime <see cref="Type"/> and the serializable <see cref="DataType"/> enum.
    /// Used both at compile time (graph node port Type -> DataType) and at runtime (DataType -> Type)
    /// </summary>
    public static class DataTypeExtensions
    {
        private static readonly IReadOnlyDictionary<Type, DataType> TypeToDataType = new Dictionary<Type, DataType>
        {
            { typeof(int), DataType.Int },
            { typeof(long), DataType.Long },
            { typeof(float), DataType.Float },
            { typeof(double), DataType.Double },
            { typeof(string), DataType.String },
            { typeof(Vector3), DataType.Vector3 },
            { typeof(Vector2), DataType.Vector2 },
            { typeof(Color), DataType.Color },
            { typeof(PlayerType), DataType.Player },
            { typeof(ItemType), DataType.ItemType },
            { typeof(bool), DataType.Bool },
        };

        private static readonly IReadOnlyDictionary<DataType, Type> DataTypeToType = new Dictionary<DataType, Type>
        {
            { DataType.Int, typeof(int) },
            { DataType.Long, typeof(long) },
            { DataType.Float, typeof(float) },
            { DataType.Double, typeof(double) },
            { DataType.String, typeof(string) },
            { DataType.Vector3, typeof(Vector3) },
            { DataType.Vector2, typeof(Vector2) },
            { DataType.Color, typeof(Color) },
            { DataType.Player, typeof(PlayerType) },
            { DataType.ItemType, typeof(ItemType) },
            { DataType.Bool, typeof(bool) },
        };

        /// <summary>
        /// Converts a runtime <see cref="Type"/> to its <see cref="DataType"/> representation.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the type has no registered mapping.</exception>
        public static DataType ToDataType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (TypeToDataType.TryGetValue(type, out var dataType))
                return dataType;

            throw new ArgumentException($"No DataType mapping registered for type '{type.FullName}'.", nameof(type));
        }

        /// <summary>
        /// Converts a <see cref="DataType"/> back to its runtime <see cref="Type"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the DataType has no registered mapping.</exception>
        public static Type ToType(this DataType dataType)
        {
            if (DataTypeToType.TryGetValue(dataType, out var type))
                return type;

            throw new ArgumentException($"No Type mapping registered for DataType '{dataType}'.", nameof(dataType));
        }

        /// <summary>
        /// Non-throwing variant of <see cref="ToDataType"/>.
        /// </summary>
        public static bool TryGetDataType(this Type type, out DataType dataType)
        {
            dataType = default;
            return type != null && TypeToDataType.TryGetValue(type, out dataType);
        }

        /// <summary>
        /// Non-throwing variant of <see cref="ToType"/>.
        /// </summary>
        public static bool TryGetType(this DataType dataType, out Type type)
        {
            return DataTypeToType.TryGetValue(dataType, out type);
        }
    }
}