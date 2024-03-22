namespace WGame.UI.Main
{
	public partial class FUI_ServerItem
	{
		private ulong id;
		public void SetData(PlayerRoomInfo info)
		{
			this.id = info.id;
			SetInfo(info);
		}

		private void SetInfo(PlayerRoomInfo info)
		{
			txtName.text = "玩家 " + info.id;
			txtState.text = info.isReady ? "已准备" : "未准备";
			var data = GameData.Tables.TbCharacter.Get(info.charId);
			txtCharName.text = data.Name;
			imgServer.visible = info.id == 0;
		}

		public void Refresh()
		{
			if( WNetMgr.Inst.TryGetPlayerRoomInfo(id, out var info))
			{
				SetInfo(info);
			}
		}
    }
}
