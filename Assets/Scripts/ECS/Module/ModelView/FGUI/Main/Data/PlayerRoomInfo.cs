using Unity.Netcode;
using WGame.UI;

public struct PlayerRoomInfo : INetworkSerializable
{
    public ulong id;
    public bool isReady;
    public int charId;

    public PlayerRoomInfo(ulong id, bool isReady)
    {
        this.id = id;
        this.isReady = isReady;
        this.charId = NetPlayerInfoModel.Inst.CurCharacterID;
    }
    public PlayerRoomInfo(ulong id, bool isReady, int charId)
    {
        this.id = id;
        this.isReady = isReady;
        this.charId = charId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref isReady);
        serializer.SerializeValue(ref charId);
    }
}
