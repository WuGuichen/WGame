using System;
using System.Collections.Generic;
using Motion;
using UnityEngine;
using WGame.Res;
using WGame.Runtime;
using WGame.Trigger;
using Random = UnityEngine.Random;


public class MethodDefine
{
    private const int TYPE_INT = BaseDefinition.TYPE_INT;
    private const int TYPE_FLOAT = BaseDefinition.TYPE_FLOAT;
    private const int TYPE_TABLE = BaseDefinition.TYPE_TABLE;
    private const int TYPE_BOOLEN = BaseDefinition.TYPE_BOOLEN;
    private const int TYPE_METHOD = BaseDefinition.TYPE_METHOD;
    private const int TYPE_STRING = BaseDefinition.TYPE_STRING;
    private const int TYPE_CHAR = BaseDefinition.TYPE_CHAR;

    private static object locker = new object();

    public void BindAll(Action<string, Action<List<Symbol>, Interpreter>> bind)
    {
        bind("Print", Print);
        bind("print", Print);
        bind("Resolve", Resolve);
        bind("InputDown", InputDown);
        bind("SetLogEnable", SetLogEnable);
        bind("BeginSample", BeginSample);
        bind("EndSample", EndSample);
        bind("GetAttr", GetAttr);
        bind("SetAttr", SetAttr);
        bind("Random_100", Random_100);
        bind("Random_Range", Random_Range);
        bind("DoMove", DoMove);
        bind("Dot", Math_Dot);
        bind("GetForward", GetForward);
        bind("GetPosition", GetPosition);
        bind("SwitchMotion", SwitchMotion);
        bind("TransMotionType", TransMotionType);
        bind("CallMotion", CallMotion);
        bind("TriggerEvent", TriggerEvent);
        bind("LoadEffect", LoadEffectToEntity);
        bind("DisposeEffect", DisposeEffect);
        bind("Signal", Signal);
        bind("SetFSM", SetFSM);
        bind("RemoveFSM", RemoveFSM);
        bind("TriggerFSM", TriggerFSM);
        bind("ChangeFSMState", ChangeFSMState);
        bind("InitGotHitDict", InitGotHitDict);
        bind("GetTargetSensorLayer", GetTargetSensorLayer);
    }

    public void TransMotionType(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        if (entity.hasLinkMotion)
        {
            if(param[1].Type != TYPE_INT)
                WLogger.Error("动作类型数据错误");
            entity.linkMotion.Motion.motionService.service.TransMotionByMotionType(param[1].Value);
        }
    }
    public void SwitchMotion(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        if (entity.hasLinkMotion)
        {
            if(param[1].Type != TYPE_INT)
                WLogger.Error("动作ID数据错误");
            entity.linkMotion.Motion.motionService.service.SwitchMotion(param[1].Value);
        }
    }
    public void Print(List<Symbol> param, Interpreter interpreter)
    {
        Method.PRINT.Call(param, interpreter);
    }
    
    public void InputDown(List<Symbol> param, Interpreter interpreter)
    {
        interpreter.SetRetrun(Input.GetKeyDown((KeyCode)param[0].Value));
    }
    
    public void GetForward(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(interpreter.ParseInt(param, 0), out var entity))
            return;
        interpreter.SetRetrun(entity.gameViewService.service.Model.forward);
    }
    
    public void GetPosition(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(interpreter.ParseInt(param, 0), out var entity))
            return;
        interpreter.SetRetrun(entity.position.value);
    }

    public void GetAttr(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(interpreter.ParseInt(param, 0), out var entity))
            return;
        var symType = param[1];
        if (symType.Type == TYPE_INT)
            interpreter.SetRetrun(entity.attribute.value.Get(symType.Value));
        else
            WLogger.Warning("请确认输入属性类型数据：GetAttr");
    }
    
    public void SetAttr(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        var symType = param[1];
        if (symType.Type == TYPE_INT)
        {
            var value = param[2];
            if(value.Type == TYPE_INT)
                entity.attribute.value.Set(symType.Value, value.Value);
            else if(value.Type == TYPE_FLOAT)
                entity.attribute.value.Set(symType.Value, (int)value.ToFloat(interpreter.Definition));
        }
        else
            WLogger.Warning("请确认输入属性类型数据：GetAttr");
    }
    
    public void Random_100(List<Symbol> param, Interpreter interpreter)
    {
        var res = new Symbol(Random.Range(0, 101));
        interpreter.SetRetrun(res);
    }
    public void Random_Range(List<Symbol> param, Interpreter interpreter)
    {
        var l = param[0];
        var r = param[1];
        var res = Symbol.NULL;
        var def = interpreter.Definition;
        if (l.Type == r.Type)
        {
            if(l.Type == TYPE_INT)
                res = new Symbol(Random.Range(param[0].Value, param[1].Value));
            else if(l.Type == TYPE_FLOAT)
                res = def.Define(Random.Range(param[0].ToFloat(def), param[1].ToFloat(def)));
        }
        if(res.IsNull)
            WLogger.Warning("请输入相同类型的Int 或 Float");
        interpreter.SetRetrun(res);
    }

    private float MultiNumber(in Symbol l, in Symbol r, BaseDefinition def)
    {
        if (l.Type == r.Type)
        {
            if (l.Type == TYPE_INT)
                return l.Value * r.Value;
            else if(l.Type == TYPE_FLOAT)
                return def.GetFloat(l.Value) * def.GetFloat(r.Value);
        }
        else
        {
            if (l.Type == TYPE_INT && r.Type == TYPE_FLOAT)
                return l.Value * def.GetFloat(r.Value);
            else if(l.Type == TYPE_FLOAT && r.Type == TYPE_INT)
                return def.GetFloat(l.Value) * r.Value;
        }

        WLogger.Error("运算错误");
        return 0;
    }

    public void Math_Dot(List<Symbol> param, Interpreter interpreter)
    {
        var l = param[0];
        var r = param[1];
        if (r.Type == l.Type)
        {
            if (r.Type == TYPE_TABLE)
            {
                var def = interpreter.Definition;
                var lR = def.GetTable(r.Value);
                var lL = def.GetTable(l.Value);
                if (lR.Count == lL.Count)
                {
                    float res = 0;
                    for (int i = 0; i < lR.Count; i++)
                    {
                        res += MultiNumber(lR[i], lL[i], def);
                    }
                    interpreter.SetRetrun(res);
                    return;
                }
                else
                {
                    WLogger.Error("请确保运算数据一致");
                }
            }
        }
        WLogger.Error("点乘运算错误");
    }
    public void DoMove(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        var def = interpreter.Definition;
        var p2 = param[2];
        float speed = p2.Value;
        if (p2.Type == TYPE_FLOAT)
            speed = def.GetFloat(p2.Value);
        ActionHelper.DoMove(entity, param[1].ToVector3(def), speed);
    }

    private static bool CheckEntity(int id, out GameEntity entity)
    {
        entity = EntityUtils.GetGameEntity(id);
        if (entity != null && entity.isEnabled)
            return false;
        WLogger.Warning("没有目标角色:" + id);
        return true;
    }

    private void CallMotion(List<Symbol> param, Interpreter interpreter)
    {
        WLangMgr.Inst.CallCode(param[0].Text, interpreter);
    }
    
    // entity, objId, key, modelPart=body, position=default, rotation=default, duration=-1.0, callback=null
    private void LoadEffectToEntity(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        int paramNum = param.Count;
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        var def = interpreter.Definition;
        float duration = -1;
        Action<GameObject> callback = null;
        Transform parent = entity.gameViewService.service.Model;
        int transType = param[2].Value;
        switch (transType)
        {
            case CharacterModelPart.Body:
                parent = entity.gameViewService.service.Model;
                break;
            case CharacterModelPart.Foot:
                parent = entity.gameViewService.service.Model;
                break;
            case CharacterModelPart.Head:
                parent = entity.gameViewService.service.Model;
                break;
            default:
                break;
        }
        if (paramNum > 3)
        {
            if (param[3].Type == TYPE_TABLE)
                pos = param[3].ToVector3(def);
            if (paramNum > 4)
            {
                if (param[4].Type == TYPE_TABLE)
                {
                    rot = param[4].ToQuaternion(def);
                }

                if (paramNum > 5)
                {
                    duration = param[5].ToFloat(def);
                    if (paramNum > 6)
                    {
                        callback = o =>
                        {
                             param[6].ToMethod(def)?.Call(new List<Symbol>(){}, interpreter);
                        };
                    }
                }
            }
        }
        EffectMgr.LoadEffect(param[1].Value, parent, pos, rot, duration, callback);
    }

    private void DisposeEffect(List<Symbol> param, Interpreter interpreter)
    {
        float delay = 0f;
        if (param.Count > 2)
            delay = param[2].ToFloat(interpreter.Definition);
        EffectMgr.DisposeEffect(param[0].Value, param[1].Value, delay);
    }
    
    private void TriggerEvent(List<Symbol> param, Interpreter interpreter)
    {
        lock (locker)
        {
            SharedScope.Inst.Define("E_P", SharedDefinition.Inst.Define(param.GetRange(1,param.Count-1)));
            EventCenter.Trigger(param[0].Value);
        }
    }

    private void Signal(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;

        float val = param[2].ToFloat(interpreter.Definition);
        switch (param[1].Value)
        {
            case InputSignalSubType.Attack:
                entity.ReplaceSignalAttack(val);
                break;
            case InputSignalSubType.Defense:
                entity.ReplaceSignalDefense(val);
                break;
            case InputSignalSubType.Step:
                entity.ReplaceSignalStep(val);
                break;
            // case InputSignalSubType.Block:
            //     // 这是什么来着
            //     // entity.ReplaceSignalBlock(val);
            //     break;
            case InputSignalSubType.Jump:
                entity.ReplaceSignalJump(val);
                break;
            default:
                WLogger.Error("没有定义的输入" + param[1].Value);
                break;
        }
    }

    private void SetFSM(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        
        entity.aiAgent.service.FSMAgent.SetObject(param[1].Text);
    }

    private void RemoveFSM(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        
        entity.aiAgent.service.FSMAgent.SetObject(param[1].Text);
    }

    private void TriggerFSM(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        if(param.Count > 2)
            entity.aiAgent.service.TriggerFSM(param[1].Text, param[2].Value);
        else
            entity.aiAgent.service.TriggerFSM(param[1].Value);
    }
    
    private void ChangeFSMState(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        var fsmName = interpreter.ParseString(param, 1);
        var newState = interpreter.ParseInt(param, 2);
        entity.aiAgent.service.FSMAgent.TransFSMState(fsmName, newState);
    }

    private void ShowMessage(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        if(param.Count > 0)
            entity.uIHeadPad.service.SetMessage(param[0].Text);
        else
            entity.uIHeadPad.service.SetMessage(null);
    }

    private void InitGotHitDict(List<Symbol> param, Interpreter interpreter)
    {
        MotionIDs.onHitDict.Clear();
        for (int i = 0; i < param.Count; i++)
        {
            var list = param[i].ToTable(interpreter.Definition);
            for (int j = 0; j < list.Count; j++)
            {
                var item = list[j].ToTable(interpreter.Definition);
                MotionIDs.onHitDict[item[0].Value] = item[1].Text;
            }
        }
    }

    private void GetTargetSensorLayer(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        interpreter.SetRetrun(new Symbol(EntityUtils.GetTargetSensorLayer(entity)));
    }

    public void Resolve(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(interpreter.ParseInt(param, 0), out var entity))
            return; 
        var sym = entity.linkVM.VM.vMService.service.Resolve(interpreter.ParseString(param, 1));
        interpreter.SetRetrun(sym);
    }
    
    public static void MoveEntityToTarget(List<Symbol> param, Interpreter interpreter)
    {
        if (CheckEntity(param[0].Value, out var entity))
            return;
        var agent = entity.aiAgent.service.MoveAgent;
        if(param.Count < 2)
            interpreter.SetRetrun(agent.MoveToTarget());
        else
        {
            if (param[1].Type == BaseDefinition.TYPE_FLOAT)
                interpreter.SetRetrun(agent.MoveToTarget(param[1].ToFloat(interpreter.Definition)));
            else
                interpreter.SetRetrun(agent.MoveToTarget(param[1].Value));
        }
    }

    private static void SetLogEnable(List<Symbol> param, Interpreter interpreter)
    {
        var value = interpreter.ParseBool(param, 0, true);
        WLogger.IsEnableWLangLog = value;
    }
    
    private static void BeginSample(List<Symbol> param, Interpreter interpreter)
    {
        var name = interpreter.ParseString(param, 0);
        UnityEngine.Profiling.Profiler.BeginSample(name);
    }
    
    private static void EndSample(List<Symbol> param, Interpreter interpreter)
    {
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
