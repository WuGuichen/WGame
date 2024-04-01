using System;
using System.Collections.Generic;
using UnityEngine;
using WGame.Attribute;
using WGame.Runtime;

namespace Motion
{
    public class MotionHelper : Singleton<MotionHelper>
    {
        private System.Reflection.FieldInfo[] motionInfos;
        private int[] codeStack = new int[256];
        private int cp = 0;

        public MotionHelper()
        {
            motionInfos = typeof(MotionIDs).GetFields();
        }
        /// <summary>
        /// 可以用来查询, 实际直接使用定点数
        /// </summary>
        public float ParseValue(int type, int param)
        {
            return type switch
            {
                AnimTriggerType.TransMotionType => param,
                _ => param * 0.01f
            };
        }
        
        public string ParseDesc(int type, int param)
        {
            return type switch
            {
                AnimTriggerType.ThrustUp => param*0.01f + "m",
                AnimTriggerType.KeepDistFromTarget => param*0.01f + "m",
                AnimTriggerType.Move => param < 0 ? "锁定平面" : (param + "%"),
                AnimTriggerType.Loop => param switch{0 => "标记", <100 => "重置trigger", _ => "重置trigger和动画"},
                AnimTriggerType.TransMotionType => GetMotionTypeName(param),
                AnimTriggerType.SwitchMotion => GetMotionName(param),
                AnimTriggerType.Rotate => param >0 ? param + "%" : -param + "%" + "包括锁定",
                AnimTriggerType.OpenSensor => GetWeaponSensorName(param),
                AnimTriggerType.CloseSensor => GetWeaponSensorName(param),
                _ => param + "%"
            };
        }

        private string GetWeaponSensorName(int param)
        {
            if(param < SensorType.list.Length)
                return SensorType.list[param];
            return "未定义";
        }

        public string GetMotionTypeName(int param)
        {
            for (int i = 0; i < MotionType.Count; i++)
            {
                if (MotionType.EnumList[i] == param)
                {
                    return MotionType.Names[i];
                }
            }

            return "未定义";
        }

        public string GetMotionName(int param)
        {
            for (int i = 0; i < MotionIDs.IDList.Length; i++)
            {
                if (MotionIDs.IDList[i] == param)
                {
                    return MotionIDs.NameList[i];
                }
            }
            return "未定义";
        }

        public string GetAttributeDesc(int param)
        {
            return param.ToString();
        }

        public string ParseCommandParam(int type, int param)
        {
            return type switch
            {
                ByteCodeType.GetAttribute => WAttrType.Names[param],
                ByteCodeType.SetAttribute => WAttrType.Names[param],
                ByteCodeType.End => "提前结束",
                ByteCodeType.Add or ByteCodeType.Subtract or ByteCodeType.Multiply 
                    or ByteCodeType.Divide or ByteCodeType.GetTarget or ByteCodeType.GetMySelf => "",
                ByteCodeType.Literal_FLOAT2 => (param*0.01f).ToString("f2"),
                _ => param.ToString()
            };
        }

        public void InterpretByteCode(int type, int param, GameEntity senderEntity)
        {
            var gameService = Contexts.sharedInstance.game;
            int tmp = 0;
            switch (type)
            {
                case ByteCodeType.Literal_INT:
                    codeStack[++cp] = param * 100;
                    break;
                case ByteCodeType.Literal_FLOAT2:
                    codeStack[++cp] = param;
                    break;
                case ByteCodeType.GetAttribute:
                    tmp = codeStack[cp--];
                    if (tmp > 0)
                    {
                        if (senderEntity.entityID.id == tmp)
                        {
                            codeStack[++cp] = senderEntity.attribute.value.Get(param) * 100;
                        }
                        else
                        {
                            var entt = gameService.GetEntityWithEntityID(tmp);
                            if (entt.isEnabled)
                            {
                                codeStack[++cp] = entt.attribute.value.Get(param) * 100;
                            }
                        }
                    }
                    else
                    {
                        WLogger.Warning("数据为空，跳出处理");
                        return;
                    }

                    break;
                case ByteCodeType.GetMax:
                    tmp = Math.Max(codeStack[cp--], codeStack[cp--]);
                    codeStack[++cp] = tmp;
                    break;
                case ByteCodeType.GetMin:
                    tmp = Math.Min(codeStack[cp--], codeStack[cp--]);
                    codeStack[++cp] = tmp;
                    break;
                case ByteCodeType.Add:
                    tmp = codeStack[cp--] + codeStack[cp--];
                    codeStack[++cp] = tmp;
                    break;
                case ByteCodeType.Subtract:
                    // 从栈顶减
                    tmp = codeStack[cp - 2] - codeStack[cp - 1];
                    cp -= 2;
                    codeStack[++cp] = tmp;
                    break;
                case ByteCodeType.Divide:
                    tmp = codeStack[cp - 2] / codeStack[cp - 1];
                    cp -= 2;
                    codeStack[++cp] = tmp;
                    break;
                case ByteCodeType.Multiply:
                    tmp = codeStack[cp - 2] * codeStack[cp - 1];
                    cp -= 2;
                    codeStack[++cp] = tmp;
                    break;
                case ByteCodeType.SetAttribute:
                    tmp = codeStack[cp--];
                    if (tmp > 0)
                    {
                        if (tmp == senderEntity.entityID.id)
                        {
                            tmp = codeStack[cp--] / 100;
                            senderEntity.attribute.value.Set(param, tmp);
                        }
                        else
                        {
                            var entt = gameService.GetEntityWithEntityID(tmp);
                            tmp = codeStack[cp--] / 100;
                            if (entt.isEnabled)
                            {
                                entt.attribute.value.Set(param, tmp);
                            }
                        }
                    }
                    else
                    {
                        WLogger.Warning("无目标，中止执行");
                        return;
                    }

                    break;
                case ByteCodeType.GetMySelf:
                    codeStack[++cp] = senderEntity.entityID.id;
                    break;
                case ByteCodeType.GetTarget:
                    if (senderEntity.hasFocusEntity)
                    {
                        codeStack[++cp] = senderEntity.focusEntity.entity.instanceID.ID;
                    }
                    else
                    {
                        codeStack[++cp] = -1;
                    }

                    break;
                case ByteCodeType.End:
                    return;
                default:
                    WLogger.Error("未定义的字节码解释");
                    break;
            }
        }

        public void InterpretByteCodeNode(ByteCodeCommandNode node, GameEntity entity)
        {
            cp = -1;
            for (int i = 0; i < node.commandType.Length; i++)
            {
                int type = node.commandType[i];
                int param = node.commandParam[i];
                // int tmp = 0;
                InterpretByteCode(type, param, entity);
            }
        }
    }
}