using System.Collections.Generic;

namespace WGame.Utils
{
    public sealed class PropertyContext
    {
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
            propertyHash[key] = TAny.New(type);
            TAny.Set<T>(GetProperty(key), defaultV);
        }

        public void RemoveProperty(string key)
        {
            propertyHash.Remove(key);
        }

        public TAny GetProperty(string key)
        {
            return propertyHash[key];
        }
        
        public bool TryGetProperty(string key, out TAny value)
        {
            return propertyHash.TryGetValue(key, out value);
        }
    }
}