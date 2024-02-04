using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

namespace WGame.GOAP
{
    public class GoapWorldState
    {
        public const int MAXATOMS = 64;
        // 世界状态值
        private long values;
        public long Values => values;
        
        // 标记未被使用的位
        private long dontCare;
        public long DontCare => dontCare;
        
        // 判断共享状态位
        private long shared;
        public long Shared => shared;

        // 存储各个状态名字与其在values中的对应位，方便查找状态
        private readonly Dictionary<string, int> namesTable;
        
        // 存储的已用状态长度
        private int curNamesLen;

        /// <summary>
        /// 初始化为空白世界状态
        /// </summary>
        public GoapWorldState()
        {
            // 赋值0，则二进制位全置为0；赋值-1，则二进制全置为1
            namesTable = new Dictionary<string, int>();
            // 全置为0，表示世界状态默认为false
            values = 0L;
            // 全置为1，表示世界状态的位都未被使用
            dontCare = -1L;
            shared = -1L;
            curNamesLen = 0;
        }

        /// <summary>
        /// 基于某世界状态的进一步创建，相当于复制状态设置但清空值
        /// </summary>
        /// <param name="worldState"></param>
        public GoapWorldState(GoapWorldState worldState)
        {
            // 复制信息
            namesTable = new Dictionary<string, int>(worldState.namesTable);
            curNamesLen = worldState.curNamesLen;
            shared = worldState.shared;
            // 清空世界状态
            values = 0L;
            dontCare = -1L;
        }

        /// <summary>
        /// 根据状态名，修改单个状态的值
        /// </summary>
        /// <param name="atomName">状态名</param>
        /// <param name="value">状态值</param>
        /// <param name="isShared">设置状态是否共享</param>
        /// <returns>是否修改成功</returns>
        public bool SetAtomValue(string atomName, bool value = false, bool isShared = false)
        {
            var pos = GetIdxOfAtomName(atomName);
            if (pos == -1)
            {
                // 不存在该状态
                return false;
            }

            var mask = 1L << pos;
            values = value ? (values | mask) : (values & ~mask);
            // 标记该状态已被使用(置为0)
            dontCare &= ~mask;
            if (!isShared)
            {
                // 标记该状态不共享
                shared &= ~mask;
            }
            return true;
        }

        /// <summary>
        /// 计算该世界状态与指定世界状态的相关度
        /// </summary>
        /// <returns></returns>
        public int CalcCorrelation(GoapWorldState to)
        {
            // 排除dontcare状态
            var care = to.dontCare ^ -1L;
            var diff = (values & care) ^ (to.Values & care);
            // 统计多少位是相同的，以表示相关度
            int dist = 0;
            for (int i = 0; i < MAXATOMS; ++i)
            {
                if ((diff & (1L << i)) != 0)
                {
                    // 状态相同，相关度高，代价则减少
                    --dist;
                }
            }

            return dist;
        }

        public void SetValues(long newValues)
        {
            values = newValues;
        }

        public void SetDontCare(long newDontCare)
        {
            dontCare = newDontCare;
        }

        public void Clear()
        {
            values = 0L;
            namesTable.Clear();
            curNamesLen = 0;
            dontCare = -1L;
        }

        /// <summary>
        /// 根据状态名，获取单个状态在Values中的位，如果没包含则尝试添加
        /// </summary>
        /// <param name="atomName">状态名</param>
        /// <returns>状态位</returns>
        private int GetIdxOfAtomName(string atomName)
        {
            if (namesTable.TryGetValue(atomName, out int idx))
            {
                return idx;
            }

            if (curNamesLen < MAXATOMS)
            {
                namesTable.Add(atomName, curNamesLen);
                curNamesLen++;
            }

            return -1;
        }
    }
}