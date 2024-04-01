using System.Collections.Generic;
using LitJson;
using WGame.Runtime;

namespace WGame.Ability
{
    public class DataMgr : Singleton<DataMgr>
    {
        private Dictionary<string, BuffFactoryData> mBuffPropertyHash = new();

        private IGameAssetLoader _loader;

        public void Init(IGameAssetLoader loader)
        {
            _loader = loader;
            
            Load("Buff", (BuffFactoryData b) => { mBuffPropertyHash.Add(b.Name, b); });
        }

        public void Dispose()
        {
            mBuffPropertyHash.Clear();
            _loader = null;
        }

        public void Load<T>(string name, System.Action<T> handler) where T : IData, new()
        {
            var text = _loader.LoadTextAsset(name);
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

        public bool TryGetBuffData(string id, out BuffFactoryData data)
        {
            return mBuffPropertyHash.TryGetValue(id, out data);
        }
        public BuffFactoryData GetBuffData(string id)
        {
            if (!mBuffPropertyHash.TryGetValue(id, out var buff))
            {
                WLogger.Error($"the \"{id}\" of BuffProperty is not exists!");
            }

            return buff;
        }

    }
}