using System.Text;
using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public class WGoapDebugger : IAgentDebugger
    {
        public string GetInfo(IMonoAgent agent, IComponentReference references)
        {
            var entity = agent.Entity as GameEntity;
            var buf = new StringBuilder();
            if (entity != null)
            {
                buf.Append("id：");
                buf.Append(entity.entityID.id);
                if (entity.hasLinkSensor)
                {
                    var info = entity.linkSensor.Sensor.detectorCharacterService.service.HatePointInfo;
                    buf.Append("\n仇恨等级：");
                    buf.Append(info.MaxHateEntityRank);
                    buf.Append("\n仇恨值：");
                    buf.Append(info.MaxHateEntityPoint);
                }

                return buf.ToString();
            }

            return "null";
        }
    }
}