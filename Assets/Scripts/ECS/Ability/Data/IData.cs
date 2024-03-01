using LitJson;

namespace WGame.Ability
{
    public interface IData
    {
        string DebugName { get; }
        void Deserialize(JsonData jsonData);
        JsonWriter Serialize(JsonWriter writer);
    }
}
