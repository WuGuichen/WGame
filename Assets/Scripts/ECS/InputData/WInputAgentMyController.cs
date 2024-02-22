using UnityEngine.InputSystem;
using WGame.Input;

public class WInputAgentMyController : WInputAgent
{
    // private MyController _wInput = new MyController();
    private WInput _wInput = new WInput();
    public WInput Input => _wInput;
    protected override string SaveKeyName => "TestInput";
    public override InputActionAsset InputAsset => _wInput.asset;
}
