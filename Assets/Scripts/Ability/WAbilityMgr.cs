using LitJson;
using UnityEngine;

namespace WGame.Ability
{
    using Runtime;
    using System.Collections.Generic;

    public interface IGameAssetLoader
    {
        TextAsset LoadTextAsset(string filePath);
        string[] GetFiles(string filePath);
    }

    public class WAbilityMgr : Singleton<WAbilityMgr>
    {
        private Dictionary<string, AbilityData> _abilityDataDict = new();
        public IGameAssetLoader Loader { get; private set; }

        public void Initialize(IGameAssetLoader loader)
        {
            Loader = loader;
            string[] fileList = Loader.GetFiles("/GameData/Action/");
            foreach (var filePath in fileList)
            {
                Load(filePath, (AbilityData ac) => { _abilityDataDict.Add(ac.ID, ac); });
            }
        }
        
        private void Load<T>(string name, System.Action<T> handler) where T : IData, new()
        {
            var text = Loader.LoadTextAsset(name);
            if (text == null)
            {
                WLogger.Error($"the {name} of resource is not exist!!!");
                return;
            }

            JsonData jd = JsonMapper.ToObject(text.ToString().Trim());
            JsonData data = jd["Data"];
            for (int i = 0; i < data.Count; ++i)
            {
                T t = new T();
                t.Deserialize(data[i]);
                handler(t);
            }
        }
    }
}