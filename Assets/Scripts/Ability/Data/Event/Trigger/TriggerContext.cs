using System.Collections.Generic;
using WGame.Utils;

namespace WGame.Ability
{
    public sealed class TriggerContext
    {
        private static Stack<TriggerContext> _pool = new();
        public static TriggerContext Get(int triggerType)
        {
            if (_pool.Count > 0)
            {
                var res = _pool.Pop();
                res.TriggerType = triggerType;
                return res;
            }

            return new TriggerContext(){TriggerType = triggerType};
        }

        public static void Push(TriggerContext context)
        {
            _pool.Push(context);
        }
        
        public int TriggerType { get; private set; }
        
        private Dictionary<string, TAny> propertyHash = new Dictionary<string, TAny>();

        public void AddProperty(string key, TAny value)
        {
            propertyHash[key] = value;
        }
        
        public void AddProperty(string key, DataType type)
        {
            propertyHash[key] = TAny.New(type);
        }

        public void AddProperty<T>(string key, DataType type, T defaultV)
        {
            var t = TAny.New(type);
            propertyHash[key] = t;
            TAny.Set<T>(t, defaultV);
        }

        public void RemoveProperty(string key)
        {
            propertyHash.Remove(key);
        }

        public TAny GetProperty(string key)
        {
            return propertyHash[key];
        }
    }
}