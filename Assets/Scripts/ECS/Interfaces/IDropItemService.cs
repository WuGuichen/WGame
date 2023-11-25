using UnityEngine;

public enum DropItemType
{
    Weapon = 0,
}

public interface IDropItemService
{
    GameEntity Entity { get; }
    Transform Transform { get; }
    DropItemType Type { get; }
    void Drop();
}
