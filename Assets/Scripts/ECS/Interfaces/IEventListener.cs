
using Entitas;

public interface IEventListener
{
    public void RegisterEventListener(Contexts contexts, IEntity entity);
}
