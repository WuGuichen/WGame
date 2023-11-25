using WGame.Attribute;

public interface ICharacterUIService
{
    bool IsActive { get; set; }

    void UpdateUI(GameEntity entity);
    void Destroy(GameEntity entity);
    void OnDead(GameEntity entity);
    void RegisterEvent(WAttribute attribute);
    void Show(float time = -1f);
    void Hide(float time = -1f);
    void SetMessage(string msg);
}
