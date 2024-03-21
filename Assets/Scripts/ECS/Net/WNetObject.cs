using Unity.Netcode;

public class WNetObject : NetworkBehaviour
{
    public int ServerID { get; private set; }
    public NetworkObject _obj;

    public override void OnNetworkSpawn()
    {
    }
}
