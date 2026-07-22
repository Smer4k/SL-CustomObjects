namespace DONT_TOUCH.Scripts.VisualScriptSystem.Enums
{
    public enum NodeType : uint
    {
        None = 0,
        Constant = 1,
        Variable = 2,
        
        ConditionAnd = 0x01000,
        ConditionOr,
        ConditionCompare,
        ConditionHasEffect,
        ConditionHasItem,
        
        MathAdd = 0x02000,
        MathSubtract,
        MathMultiply,
        MathDivide,
        
        SetPlayerPosition = 0x03000,
        SetPlayerHealth,
        SetAnimation,
        
        GetPlayer = 0x04000,
        GetPlayerPosition,
        GetPlayerHealth,
        GetPlayerId,
        GetPlayerCurrentItem,
        GetObjectPosition,
        
        ActionCommand = 0x05000,
        ActionDamagePlayer,
        ActionDestroy,
        ActionIfElse,
        ActionPlayAudio,
        ActionWait,
        
        EventSpawn = 0x06000,
    }
}