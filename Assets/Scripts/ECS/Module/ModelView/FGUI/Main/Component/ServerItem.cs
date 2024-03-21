using FairyGUI;

namespace WGame.UI.Main
{
	public partial class FUI_ServerItem: GComponent
	{
		private ulong id;
		public void SetData(PlayerRoomInfo info)
		{
			this.id = info.id;
			txtName.text = "玩家 " + info.id;
			txtState.text = info.isReady ? "已准备" : "未准备";
		}

		public void Refresh()
		{
			if( WNetMgr.Inst.TryGetPlayerRoomInfo(id, out var info))
			{
				txtName.text = "玩家 " + info.id;
				txtState.text = info.isReady ? "已准备" : "未准备";
			}
		}
    }
}
