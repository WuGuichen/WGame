using LitJson;

namespace WGame.Ability
{
    public interface IData
    {
        string DebugName { get; }
        void Deserialize(JsonData jd);
        JsonWriter Serialize(JsonWriter writer);
    }
}
