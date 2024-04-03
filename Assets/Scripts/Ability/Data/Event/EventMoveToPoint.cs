using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventMoveToPoint : IData, IEventData
    {
        public string DebugName { get; }
        
        [EditorData("目标点", EditorDataType.Vector3)]
        public Vector3 Point { get; set; }
        [EditorData("是世界位置", EditorDataType.Bool)]
        public bool IsWordPos { get; set; }
        [EditorData("是否可移动", EditorDataType.Bool)]
        public bool CanMove { get; set; }
        [EditorData("使用重力", EditorDataType.Bool)]
        public bool UseGravity { get; set; }

        [EditorData("曲线", EditorDataType.Enum)]
        public WEaseType EaseType { get; set; } = WEaseType.Linear;
        
        public void Deserialize(JsonData jd)
        {
            var data = jd["Move"];
            Point = JsonHelper.ReadVector3(data[0]);
            IsWordPos = JsonHelper.ReadBool(data[1]);
            CanMove = JsonHelper.ReadBool(data[2]);
            UseGravity = JsonHelper.ReadBool(data[3]);
            EaseType = JsonHelper.ReadEnum<WEaseType>(data[4]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("Move");
            writer.WriteArrayStart();
            JsonHelper.Write(ref writer, Point);
            writer.Write(IsWordPos);
            writer.Write(CanMove);
            writer.Write(UseGravity);
            writer.Write(EaseType.ToString());
            writer.WriteArrayEnd();
            return writer;
        }

        private Vector3 _lastDeltaPos;
        private Vector3 _totalPos;
        public EventDataType EventType => EventDataType.MoveToPoint;
        public void Enter(EventOwner owner)
        {
            if (!UseGravity)
            {
                owner.SetGravity(0);
            }

            _lastDeltaPos = Vector3.zero;
            _totalPos = Vector3.zero;
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            var rate = WEaseManager.Evaluate(EaseType, duration, totalTime);
            var curPos = Point * rate;
            if (rate > 1f)
            {
                rate = 1f;
            }
            
            owner.Move(curPos - _lastDeltaPos);
            _totalPos += curPos - _lastDeltaPos;
            _lastDeltaPos = curPos;
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            owner.SetGravity(0, true);
            if (!isBreak)
            {
                owner.Move(Point - _lastDeltaPos);
                _totalPos += Point - _lastDeltaPos;
            }
        }

        public IEventData Clone()
        {
            return new EventMoveToPoint()
            {
                Point = Point, IsWordPos = IsWordPos, EaseType = EaseType,
                CanMove = CanMove, UseGravity = UseGravity
            };
        }
    }
}