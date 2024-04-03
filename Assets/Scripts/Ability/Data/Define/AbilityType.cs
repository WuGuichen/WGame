public class AbilityType
{
    public const int JumpLand = 1;
    public const int FinishAttack = 1 << 1;
    public const int VictimFinishAttack = 1 << 2;
    public const int Falling = 1 << 3;
    public const int SpecialSkill = 1 << 4;

    public static readonly string[] Names = new string[]
    {
        "落地",
        "处决",
        "被处决",
        "坠落",
        "特殊技"
    };
}

public class WTypeMap
{
    private int[] _map = new int[37];

    public void Set(int type, int id)
    {
        _map[type%37] = id;
    }

    public void Remove(int type)
    {
        _map[type%37] = -1;
    }

    public int Get(int type)
    {
        return _map[type%37];
    }

    public bool TryGet(int type, out int id)
    {
        id = _map[type % 37];
        return id > 0;
    }
}