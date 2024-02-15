namespace CrashKonijn.Goap.Interfaces
{
    public interface IMonoAgent : IAgent, IMonoBehaviour
    {
        public Entitas.Entity Entity { get; set; }
    }
}