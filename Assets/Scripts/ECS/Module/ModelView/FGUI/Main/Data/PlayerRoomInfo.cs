using Unity.Netcode;

public struct PlayerRoomInfo : INetworkSerializable
{
    public ulong id;
    public bool isReady;

    public PlayerRoomInfo(ulong id, bool isReady)
    {
        this.id = id;
        this.isReady = isReady;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref isReady);
    }
}
