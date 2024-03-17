namespace WGame.Ability
{
    public struct EntityMoveInfo
    {
        public EntityMoveType MoveType { get; }
        public float Speed { get; }

        public EntityMoveInfo(EntityMoveType moveType, float speed)
        {
            MoveType = moveType;
            Speed = speed;
        }
    }
}