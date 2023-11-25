using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WGame.Attribute
{
    public class WAttrAddType
    {
        public static Stack<WAttrAddType> _pool;
        public static WAttrAddType Get()
        {
            if (_pool.Count > 0)
                return _pool.Pop();
            return new WAttrAddType();
        }

        public static void Push(WAttrAddType attrAddType)
        {
            _pool.Push(attrAddType);
        }
        
        public int[][] attrRates;
        public int addValue;

        public float GetValue(WAttribute attribute)
        {
            float res = 0;
            for (int i = 0; i < attrRates.Length; i++)
            {
                res += attribute.Get(attrRates[i][0]) * attrRates[i][1];
            }

            res += addValue;
            return res;
        }
    }
}