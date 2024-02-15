using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Interfaces;
using CrashKonijn.Goap.Sensors;
using UnityEngine;

namespace WGame.GOAP
{
    public class PatrolTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
            // WLogger.Print("Update");
        }

        public override ITarget Sense(IMonoAgent agent, IComponentReference references)
        {
            var random = GetRandomPosition(agent);

            return new EntityTarget(EntityUtils.GetCameraEntity());
        }

        Vector3 GetRandomPosition(IMonoAgent agent)
        {
            var random = Random.insideUnitCircle * 5f;
            var position = agent.transform.position + new Vector3(random.x, 0f, random.y);

            return position;
        }
    }
}