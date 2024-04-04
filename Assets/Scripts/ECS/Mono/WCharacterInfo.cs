using BaseData.Character;
using UnityEngine;

[DisallowMultipleComponent]
public class WCharacterInfo : MonoBehaviour
{
    private MonoBehaviour rigidbodyService;
    [Header("属性ID")]
    [SerializeField] private int infoID;
    [Header("初始武器")] [SerializeField] private int weaponID;
    [Header("AI配置ID")] [SerializeField] private int aiID;
    [Header("基本动画组配置ID")] [SerializeField] private int animID;
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
    [SerializeField] private int DetectDegree;
    [SerializeField] private float DetectRadius1;
    [SerializeField] private float DetectRadius2;
    [SerializeField] private bool showPatrolPoint = false;
    [SerializeField] private bool showDetectArea = false;

    private Vector3 originPos = Vector3.zero;
    private GameEntity entity = null;
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
            return new CharacterInitInfo();
        }

        var info = new CharacterInitInfo()
        {
            job = data.Job,
            camp = data.Camp,
            gender = data.Gender,
            race = data.Race,
            patrolPoints = defaultPatrolPoints,
            moveSpeed = data.MoveSpeed,
            rotateSpeed = data.RotationSpeed,
            runMultiRate = data.RunSpeedMul,
            MaxHP = data.MaxHP,
            CurHP = data.CurHP,
            MaxMP = data.MaxMP,
            CurMP = data.CurMP,
            ATK = data.ATK,
            DEF = data.DEF,
            Weapon = data.Weapon,
            PatrolMul = data.PatrolMul*0.01f,
            ChaseMul =data.ChaseMul*0.01f,
            DetectDegree = data.DetectDegree,
            DetectRadius1 = data.DetectRadius1,
            DetectRadius2 = data.DetectRadius2,
            AICfg = GameData.Tables.TbCharAI.Get(data.AI),
            AnimCfg = GameData.Tables.TbCharAnim.Get(data.AnimGroup)
        };
        return info;
    }
    
    public CharacterInitInfo GetCharacterInfo()
    {
        if (infoID > 0)
        {
            return GetCharacterInfo(infoID);
        }

        if (aiID <= 0)
            aiID = 1;
        if (animID <= 0)
            animID = 1;
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
            moveSpeed = moveSpeed,
            rotateSpeed = rotationSpeed,
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
            DetectDegree = DetectDegree,
            DetectRadius1 = DetectRadius1,
            DetectRadius2 = DetectRadius2,
            AICfg = GameData.Tables.TbCharAI.Get(aiID),
            AnimCfg = GameData.Tables.TbCharAnim.Get(animID),
        };
        return info;
    }

    public void InitOriginPos(Vector3 pos, GameEntity entity)
    {
        originPos = pos;
        this.entity = entity;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            originPos = transform.position;
        }
        else
        {
            if (entity != null && entity.isCamera)
            {
                return;
            }
        }
        if (showPatrolPoint)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                var pos = originPos + patrolPoints[i];
                Gizmos.color = new Color(0, i * 0.3f, 1f);
                Gizmos.DrawSphere(pos, 0.12f);
            }
        }

        if (showDetectArea)
        {
            var pos = transform.position;
            var pos1 = pos + transform.forward * DetectRadius1;
            var pos2 = pos + transform.forward * DetectRadius2;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos1);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos1, pos2);
        }
    }
    #endif

    private void Awake()
    {
    }
}
