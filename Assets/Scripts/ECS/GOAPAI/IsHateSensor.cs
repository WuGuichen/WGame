using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Interfaces;
using CrashKonijn.Goap.Sensors;

namespace WGame.GOAP
{
    public class IsHateSensor : LocalWorldSensorBase
    {
        public override void Created()
        {
            
        }

        public override void Update()
        {
            
        }

        public override SenseValue Sense(IMonoAgent agent, IComponentReference references)
        {
            var target = agent;
            return new SenseValue(0);
        }
    }
}