using BaseData.Character;
using UnityEngine;

[DisallowMultipleComponent]
public class WCharacterInfo : MonoBehaviour
{
    [Header("属性ID")]
    [SerializeField] private int infoID;
    [Header("初始武器")] [SerializeField] private int weaponID;
    [SerializeField] private Job job;
    [SerializeField] private Gender gender;
    [SerializeField] private Race race;
    [SerializeField] private Camp camp;
    [Header("移动速度")]
    [SerializeField] private int moveSpeed;
    [Header("转向速度")]
    [SerializeField] private int rotationSpeed;
    [Header("跑步倍率")]
    [SerializeField] private int runSpeedRate;
    [Header("巡逻点")]
    [SerializeField] private Vector3[] patrolPoints;
    [Header("其他属性")]
    [SerializeField] private int maxHP;
    [SerializeField] private int curHP;
    [SerializeField] private int maxMP;
    [SerializeField] private int curMP;
    [SerializeField] private int ATK;
    [SerializeField] private int DEF;
    [SerializeField] private int PatrolMul;
    [SerializeField] private int ChaseMul;
    [SerializeField] private bool showPatrolPoint = false;

    private static Vector3[] defaultPatrolPoints = new Vector3[]
    {
        new Vector3(5,0,-2),
        new Vector3(1,0,6),
        new Vector3(-5, 0, -2),
        new Vector3(-7, 0, 2),
    };

    public static CharacterInitInfo GetCharacterInfo(int id)
    {
        var data = GameData.Tables.TbCharacterInfo[id];
        if (data == null)
        {
            WLogger.Error("CharacterInfoData表没有对应角色信息id：" + id);
            return DefaultValue;
        }

        var info = new CharacterInitInfo()
        {
            job = data.Job,
            camp = data.Camp,
            gender = data.Gender,
            race = data.Race,
            patrolPoints = defaultPatrolPoints,
            moveSpeed = data.MoveSpeed*0.01f,
            rotateSpeed = data.RotationSpeed * 0.01f,
            runMultiRate = data.RunSpeedMul,
            MaxHP = data.MaxHP,
            CurHP = data.CurHP,
            MaxMP = data.MaxMP,
            CurMP = data.CurMP,
            ATK = data.ATK,
            DEF = data.DEF,
            Weapon = data.Weapon,
            PatrolMul = data.PatrolMul*0.01f,
            ChaseMul =data.ChaseMul*0.01f
        };
        return info;
    }
    
    private static CharacterInitInfo DefaultValue = new CharacterInitInfo()
    {
        job = Job.Swordman,
        camp = Camp.Enemy,
        gender = Gender.Male,
        race = Race.Skeleton,
        patrolPoints = defaultPatrolPoints,
        moveSpeed = 400*0.01f,
        rotateSpeed = 800 * 0.01f,
        runMultiRate = 200,
        MaxHP = 100,
        CurHP = 100,
        MaxMP = 100,
        CurMP = 100,
        ATK = 2,
        DEF = 2,
        Weapon = 0,
        PatrolMul = 60*0.01f,
        ChaseMul = 100*0.01f,
    };
    public CharacterInitInfo GetCharacterInfo()
    {
        if (infoID > 0)
        {
            return GetCharacterInfo(infoID);
        }
        var points = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
            points[i] = patrolPoints[i] + transform.position;
        var info = new CharacterInitInfo
        {
            job = job,
            camp = camp,
            gender = gender,
            race = race,
            patrolPoints = points,
            moveSpeed = moveSpeed*0.01f,
            rotateSpeed = rotationSpeed * 0.01f,
            runMultiRate = runSpeedRate,
            MaxHP = maxHP,
            CurHP = curHP,
            MaxMP = maxMP,
            CurMP = curMP,
            ATK = ATK,
            DEF = DEF,
            Weapon = weaponID,
            PatrolMul = PatrolMul*0.01f,
            ChaseMul = ChaseMul*0.01f,
        };
        return info;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showPatrolPoint)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                var pos = transform.position + patrolPoints[i];
                Gizmos.color = new Color(0, i * 0.3f, 1f);
                Gizmos.DrawSphere(pos, 0.12f);
            }
        }
    }
    #endif
}
