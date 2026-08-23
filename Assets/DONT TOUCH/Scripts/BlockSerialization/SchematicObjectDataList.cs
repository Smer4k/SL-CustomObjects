using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts;
using DONT_TOUCH.Scripts.BlockSerialization;
using Newtonsoft.Json;


[Serializable]
public class SchematicObjectDataList
{
#if UNITY_6000_5_OR_NEWER
    public long RootObjectId { get; set; }
#else
    public int RootObjectId { get; set; }
#endif
    public List<SchematicBlockData> Blocks { get; set; } = new List<SchematicBlockData>();
}