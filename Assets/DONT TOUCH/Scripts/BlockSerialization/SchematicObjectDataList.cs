using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts;
using DONT_TOUCH.Scripts.BlockSerialization;
using Newtonsoft.Json;


[Serializable]
public class SchematicObjectDataList
{
    [JsonConverter(typeof(UncheckedULongConverter))]
    public ulong RootObjectId { get; set; }

    public List<SchematicBlockData> Blocks { get; set; } = new List<SchematicBlockData>();
}