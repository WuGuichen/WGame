using BaseData;
using BaseData.Character;
using UnityEngine;

public struct CharacterInitInfo
{
    public Job job { get; set; }
    public Gender gender { get; set; }
    public Race race { get; set; }
    public Camp camp { get; set; }
    public Vector3[] patrolPoints { get; set; }
    public float moveSpeed { get; set; }
    public float rotateSpeed { get; set; }
    public int runMultiRate { get; set; }
    public int MaxHP { get; set; }
    public int CurHP { get; set; }
    public int MaxMP { get; set; }
    public int CurMP { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int Weapon { get; set; }
    public float ChaseMul { get; set; }
    public float PatrolMul { get; set; }
    public CharAI AICfg { get; set; }

    public static CharacterInitInfo Empty = new CharacterInitInfo();

}
