namespace WGame.UI.Character
{
	public partial class FUI_HeadPad
	{
		public void SetMaxHP(int hp)
		{
			hpBar.max = hp;
		}

		public void SetValue(int value)
		{
			hpBar.value = value;
		}

		public void ShowValue(string value)
		{
			hpBar.txtValue.text = value;
		}
	}
}
