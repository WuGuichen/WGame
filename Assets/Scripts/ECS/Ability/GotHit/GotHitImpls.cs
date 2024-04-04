public class GotHitImpls
{
    public static IGotHitService Normal = new GotHitAbilityServiceImplementation();
    public static IGotHitService Invincible = new OnStepGotHitImpl();
}

public class HitTargetImpls
{
    public static IHitTargetService Normal = new NormalHitTargetImpl();
}