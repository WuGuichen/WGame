using System;
using System.Collections.Generic;
using System.Text;
using CleverCrow.Fluid.BTs.Tasks;
using Motion;
using UnityEngine;
using WGame.Attribute;
using WGame.Trigger;

public class ConstDefine
{
    public void BindInt(System.Action<string, int> bind)
    {
        bind("ATK", WAttrType.ATK);
        bind("CUR_HP", WAttrType.CurHP);
        bind("MAX_HP", WAttrType.MaxHP);
        bind("DEF", WAttrType.DEF);
        bind("CUR_MP", WAttrType.CurMP);
        bind("MAX_MP", WAttrType.MaxMP);
        bind("ATTACK1", MotionType.Attack1);
        bind("ATTACK2", MotionType.Attack2);
        bind("ATTACK3", MotionType.Attack3);
        bind("JUMP", MotionType.Jump);
        bind("JUMP_ATTACK1", MotionType.JumpAttack1);
        bind("STEP", MotionType.Step);
        bind("DEFENSE", MotionType.Defense);
        bind("LOCAL_MOTION", MotionType.LocalMotion);
        bind("HIT_FWD", MotionType.HitFwd);
        bind("HIT_BWD", MotionType.HitBwd);
        bind("SUCCESS", (int)TaskStatus.Success);
        bind("FAIL", (int)TaskStatus.Failure);

        BindIntDefine("MODEL_", typeof(CharacterModelPart), bind);
        BindIntDefine("SIG_", typeof(InputSignalSubType), bind);
        BindIntDefine("SD_", typeof(StateDefine), bind);

        for (int i = 0; i < MotionIDs.IDList.Length; i++)
        {
            bind("MID_" + MotionIDs.NameList[i].ToUpper(), MotionIDs.IDList[i]);
        }

        bind("KEY_I", (int)KeyCode.I);
        bind("KEY_U", (int)KeyCode.U);
        bind("KEY_O", (int)KeyCode.O);
        
        InitIntConstName();
    }

    private void BindIntDefine(string title, Type type, System.Action<string, int> bind)
    {
        var infos = type.GetFields();
        StringBuilder buf = new StringBuilder();
        for (int i = 0; i < infos.Length; i++)
        {
            int val = (int)infos[i].GetRawConstantValue();
            FSMInterpreter.ParseDefineString(title, infos[i].Name, ref buf);
            bind(buf.ToString(), val);
            buf.Clear();
        }
    }

    private static Dictionary<int, string> stateDefineName = new();

    private static void InitIntConstName()
    {
        var infos = typeof(StateDefine).GetFields();
        for (int i = 0; i < infos.Length; i++)
        {
            stateDefineName[(int)infos[i].GetRawConstantValue()] = infos[i].Name;
        }
    }

    public static string GetStateDefineName(int key)
    {
        if (stateDefineName.TryGetValue(key, out var name))
            return name;
        return "未定义";
    }
}