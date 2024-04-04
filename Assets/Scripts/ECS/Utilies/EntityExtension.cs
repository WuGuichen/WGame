public static class EntityExtension
{
}

public partial class GameEntity
{
    public bool TryGetAbility(out AbilityEntity ability)
    {
        if(hasLinkAbility)
        {
            ability = linkAbility.Ability;
            return true;
        }

        ability = null;
        return false;
    }
    
    public bool TryGetMotion(out MotionEntity motion)
    {
        if(hasLinkAbility)
        {
            motion = linkMotion.Motion;
            return true;
        }

        motion = null;
        return false;
    }
}
