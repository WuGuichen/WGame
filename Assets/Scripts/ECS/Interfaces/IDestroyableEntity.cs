using Entitas;

public interface IDestroyableEntity : IEntity, IDestroyedEntity
{
    
}

public partial class GameEntity : IDestroyableEntity { }
public partial class InputEntity : IDestroyableEntity { }
public partial class WeaponEntity : IDestroyableEntity { }
public partial class SensorEntity : IDestroyableEntity { }
