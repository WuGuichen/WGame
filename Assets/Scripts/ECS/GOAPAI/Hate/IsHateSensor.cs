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
            var target = agent.Entity as GameEntity;
            if (target != null)
            {
                var info = target.linkSensor.Sensor.detectorCharacterService.service.HatePointInfo;
                return new SenseValue(info.MaxHateEntityRank >= HateRankType.Alert);
            }

            return false;
        }
    }
}