using LitJson;

namespace WGame.Ability
{
    using Runtime;
    using System.Collections.Generic;

    public interface IGameAssetLoader
    {
        string LoadTextAsset(string filePath);
        string[] GetAbilityGroups();
    }

    public class WAbilityMgr : Singleton<WAbilityMgr>
    {
        private Dictionary<string, AbilityData> _abilityDataDict = new();
        public IGameAssetLoader Loader { get; private set; }

        public void Initialize(IGameAssetLoader loader)
        {
            Loader = loader;
            string[] fileList = Loader.GetAbilityGroups();
            foreach (var filePath in fileList)
            {
                Load(filePath, (AbilityData ac) => { _abilityDataDict.Add(ac.Name, ac); });
            }
        }

        public void HotReloadGroup(string filePath)
        {
            Load(filePath, (AbilityData ac) => { _abilityDataDict[ac.Name] = ac; });
            WLogger.Print($"已更新AbilityGroup: {filePath}");
        }
        
        private void Load<T>(string name, System.Action<T> handler) where T : IData, new()
        {
            var text = Loader.LoadTextAsset(name);
            if (string.IsNullOrEmpty(text))
            {
                WLogger.Error($"the {name} of resource is not exist!!!");
                return;
            }

            JsonData jd = JsonMapper.ToObject(text.Trim());
            JsonData data = jd["Data"];
            for (int i = 0; i < data.Count; ++i)
            {
                T t = new T();
                t.Deserialize(data[i]);
                handler(t);
            }
        }

        public AbilityData GetAbility(string name)
        {
            if (_abilityDataDict.TryGetValue(name, out var abilityData))
            {
                return abilityData;
            }

            WLogger.Error($"没有找到Ability {name}");
            return null;
        }
    }
}