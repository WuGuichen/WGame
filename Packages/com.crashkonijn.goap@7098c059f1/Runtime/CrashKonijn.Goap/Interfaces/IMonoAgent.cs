namespace CrashKonijn.Goap.Interfaces
{
    public interface IMonoAgent : IAgent, IMonoBehaviour
    {
        public int EntityID { get; set; }
    }
}