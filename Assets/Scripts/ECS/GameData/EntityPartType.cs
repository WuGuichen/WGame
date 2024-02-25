public enum EntityPartType
{
    // 身体
    Body = 1,
    // 头部
    Head = 1 << 1,
    // 手部
    Hand_Left = 1 << 2,
    Hand_Right = 1 << 3,
    
    Leg_Left = 1 << 4,
    Leg_Right = 1 << 5,
    
    
    Weak_1 = 1 << 6,
    Weak_2 = 1 << 7,
    
    // 极限闪避
    Evasion = 1 << 8,
}
