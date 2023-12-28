using Entitas;

public class UpdateFocusInputSystem : IExecuteSystem
{
    private readonly InputContext _inputContext;
    private readonly ITimeService _timeService;
    private readonly IGroup<GameEntity> _cameraGroup;
    private const float THRESHOLDS = 18;
    private float thresholds = 18;
    private const float COOLDOWN = 0.5f;
    private float timer = COOLDOWN;
    
    public UpdateFocusInputSystem(Contexts contexts)
    {
        _inputContext = contexts.input;
        _timeService = contexts.meta.timeService.instance;
        _cameraGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Camera, GameMatcher.GameViewService));
    }
    public void Execute()
    {
        foreach (var entity in _cameraGroup)
        {
            float area = 18f;
            if (_inputContext.focusInput.value)
            {

                if (entity.hasFocus)
                    entity.ReplaceActionFocus(FocusType.Cancel, area);
                else
                    entity.ReplaceActionFocus(FocusType.Focus, area);
            }

            if (entity.hasFocus)
            {
                var look = _inputContext.lookInput.value;
                if (look.y > thresholds)
                {
                    entity.ReplaceActionFocus(FocusType.Up, area);
                    thresholds = 9999;
                }
                else if (look.y < -thresholds)
                {
                    entity.ReplaceActionFocus(FocusType.Down, area);
                    thresholds = 9999;
                }
                else if (look.x < -thresholds)
                {
                    entity.ReplaceActionFocus(FocusType.Left, area);
                    thresholds = 9999;
                }
                else if (look.x > thresholds)
                {
                    entity.ReplaceActionFocus(FocusType.Right, area);
                    thresholds = 9999;
                }

                if (thresholds > THRESHOLDS)
                {
                    timer -= _timeService.DeltaTime;
                    if (timer < 0)
                    {
                        thresholds = THRESHOLDS;
                        timer = COOLDOWN;
                    }
                }
            }
            
        }
    }
}