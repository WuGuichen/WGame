using Unity.Netcode;

public class WNetAgent : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            WNetMgr.Inst.SetAgent(this);
        }
    }
}
