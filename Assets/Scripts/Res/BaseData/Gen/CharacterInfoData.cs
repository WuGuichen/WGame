//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace BaseData
{ 

public sealed partial class CharacterInfoData :  Bright.Config.BeanBase 
{
    public CharacterInfoData(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["weapon"].IsNumber) { throw new SerializationException(); }  Weapon = _json["weapon"]; }
        { if(!_json["AI"].IsNumber) { throw new SerializationException(); }  AI = _json["AI"]; }
        { if(!_json["job"].IsNumber) { throw new SerializationException(); }  Job = (Character.Job)_json["job"].AsInt; }
        { if(!_json["gender"].IsNumber) { throw new SerializationException(); }  Gender = (Character.Gender)_json["gender"].AsInt; }
        { if(!_json["race"].IsNumber) { throw new SerializationException(); }  Race = (Character.Race)_json["race"].AsInt; }
        { if(!_json["camp"].IsNumber) { throw new SerializationException(); }  Camp = (Character.Camp)_json["camp"].AsInt; }
        { if(!_json["moveSpeed"].IsNumber) { throw new SerializationException(); }  MoveSpeed = _json["moveSpeed"]; }
        { if(!_json["rotationSpeed"].IsNumber) { throw new SerializationException(); }  RotationSpeed = _json["rotationSpeed"]; }
        { if(!_json["runSpeedMul"].IsNumber) { throw new SerializationException(); }  RunSpeedMul = _json["runSpeedMul"]; }
        { if(!_json["maxHP"].IsNumber) { throw new SerializationException(); }  MaxHP = _json["maxHP"]; }
        { if(!_json["curHP"].IsNumber) { throw new SerializationException(); }  CurHP = _json["curHP"]; }
        { if(!_json["maxMP"].IsNumber) { throw new SerializationException(); }  MaxMP = _json["maxMP"]; }
        { if(!_json["curMP"].IsNumber) { throw new SerializationException(); }  CurMP = _json["curMP"]; }
        { if(!_json["ATK"].IsNumber) { throw new SerializationException(); }  ATK = _json["ATK"]; }
        { if(!_json["DEF"].IsNumber) { throw new SerializationException(); }  DEF = _json["DEF"]; }
        { if(!_json["patrolMul"].IsNumber) { throw new SerializationException(); }  PatrolMul = _json["patrolMul"]; }
        { if(!_json["chaseMul"].IsNumber) { throw new SerializationException(); }  ChaseMul = _json["chaseMul"]; }
        PostInit();
    }

    public CharacterInfoData(int id, string name, int weapon, int AI, Character.Job job, Character.Gender gender, Character.Race race, Character.Camp camp, int moveSpeed, int rotationSpeed, int runSpeedMul, int maxHP, int curHP, int maxMP, int curMP, int ATK, int DEF, int patrolMul, int chaseMul ) 
    {
        this.Id = id;
        this.Name = name;
        this.Weapon = weapon;
        this.AI = AI;
        this.Job = job;
        this.Gender = gender;
        this.Race = race;
        this.Camp = camp;
        this.MoveSpeed = moveSpeed;
        this.RotationSpeed = rotationSpeed;
        this.RunSpeedMul = runSpeedMul;
        this.MaxHP = maxHP;
        this.CurHP = curHP;
        this.MaxMP = maxMP;
        this.CurMP = curMP;
        this.ATK = ATK;
        this.DEF = DEF;
        this.PatrolMul = patrolMul;
        this.ChaseMul = chaseMul;
        PostInit();
    }

    public static CharacterInfoData DeserializeCharacterInfoData(JSONNode _json)
    {
        return new CharacterInfoData(_json);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 初始武器
    /// </summary>
    public int Weapon { get; private set; }
    /// <summary>
    /// AI配置ID
    /// </summary>
    public int AI { get; private set; }
    /// <summary>
    /// 职业
    /// </summary>
    public Character.Job Job { get; private set; }
    /// <summary>
    /// 性别
    /// </summary>
    public Character.Gender Gender { get; private set; }
    /// <summary>
    /// 种族
    /// </summary>
    public Character.Race Race { get; private set; }
    /// <summary>
    /// 阵营
    /// </summary>
    public Character.Camp Camp { get; private set; }
    /// <summary>
    /// 移速(cm/s)
    /// </summary>
    public int MoveSpeed { get; private set; }
    /// <summary>
    /// 转向速度(度/s)
    /// </summary>
    public int RotationSpeed { get; private set; }
    /// <summary>
    /// 跑步倍率
    /// </summary>
    public int RunSpeedMul { get; private set; }
    public int MaxHP { get; private set; }
    public int CurHP { get; private set; }
    public int MaxMP { get; private set; }
    public int CurMP { get; private set; }
    public int ATK { get; private set; }
    public int DEF { get; private set; }
    /// <summary>
    /// 巡逻倍率
    /// </summary>
    public int PatrolMul { get; private set; }
    /// <summary>
    /// 追击倍率
    /// </summary>
    public int ChaseMul { get; private set; }

    public const int __ID__ = -1485248607;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "Weapon:" + Weapon + ","
        + "AI:" + AI + ","
        + "Job:" + Job + ","
        + "Gender:" + Gender + ","
        + "Race:" + Race + ","
        + "Camp:" + Camp + ","
        + "MoveSpeed:" + MoveSpeed + ","
        + "RotationSpeed:" + RotationSpeed + ","
        + "RunSpeedMul:" + RunSpeedMul + ","
        + "MaxHP:" + MaxHP + ","
        + "CurHP:" + CurHP + ","
        + "MaxMP:" + MaxMP + ","
        + "CurMP:" + CurMP + ","
        + "ATK:" + ATK + ","
        + "DEF:" + DEF + ","
        + "PatrolMul:" + PatrolMul + ","
        + "ChaseMul:" + ChaseMul + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
