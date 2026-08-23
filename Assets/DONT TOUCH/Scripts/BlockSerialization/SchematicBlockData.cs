using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts;
using Newtonsoft.Json;

public class SchematicBlockData
{
    public string Name { get; set; }

#if UNITY_6000_5_OR_NEWER
    public long ObjectId { get; set; }

    public long ParentId { get; set; }
    
    public string VisualScriptGraphName { get; set; }
#else
    public int ObjectId { get; set; }

    public int ParentId { get; set; }
#endif

    public virtual string AnimatorName { get; set; }
    
    public SerializableVector Position { get; set; }

    public SerializableVector Rotation { get; set; }

    public SerializableVector Scale { get; set; }

    public virtual BlockType BlockType { get; set; }

    public virtual Dictionary<string, object> Properties { get; set; }
}