using UnityEngine;

namespace WGame.Ability
{
    public struct AbilityEntityInfo
    {
        public EntityMoveType MoveType { get; }
        public Vector3 OriginPos { get; }
        public Vector3 TargetPosOrDir { get; }

        public AbilityEntityInfo(EntityMoveType moveType, Vector3 originPos, Vector3 targetPosOrDir)
        {
            MoveType = moveType;
            OriginPos = originPos;
            TargetPosOrDir = targetPosOrDir;
        }
    }
}