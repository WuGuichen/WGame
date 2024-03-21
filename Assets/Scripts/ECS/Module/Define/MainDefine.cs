// 数据定义

namespace WGame.UI
{
	public class MainDefine : Runtime.Singleton<MainDefine>
	{
		public const string BtnName_StartClient = "开始客户端";
		public const string BtnName_StartServer = "加入服务器";
		public const string BtnName_StartHost = "建立服务器并加入";
		public const string BtnName_ShutDownServer = "关闭服务器";
		public const string BtnName_CmdList = "命令列表";
		public const string BtnName_Setting = "设置";
		public const string BtnName_Quit = "退出";

		public string[] mainBtnListNames = new string[]
		{
			BtnName_StartClient, BtnName_StartServer, BtnName_StartHost, BtnName_ShutDownServer,
			BtnName_CmdList, BtnName_Setting, BtnName_Quit
		};
		
		public string[] mainTopBtnListNames = new string[]
		{
			"选中随机角色","生成红色", "生成白色", "生成武器", "丢弃武器"
		};

		public float joystickDeadRadiusRate = 0.2f;

		public string[] presetCommands = new string[]
		{
			"加入武器", "敌人跟随"
		};
	}
}
