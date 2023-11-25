using Entitas;
using Entitas.VisualDebugging.Unity;

namespace Weapon
{
    [Weapon][DontDrawComponent]
    public class HitTargetsComponent : IComponent
    {
        public GameEntity[] targets;
    }
}