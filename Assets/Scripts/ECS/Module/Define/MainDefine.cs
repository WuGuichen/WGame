// 数据定义

using System.Collections.Generic;

namespace WGame.UI
{
	public class MainDefine : Runtime.Singleton<MainDefine>
	{
		public const string BtnName_StartOffline = "离线开始";
		public const string BtnName_StartServer = "选择服务器";
		public const string BtnName_StartHost = "建立服务器并加入";
		public const string BtnName_ShutDownServer = "断开连接";
		public const string BtnName_CmdList = "命令列表";
		public const string BtnName_Setting = "设置";
		public const string BtnName_Quit = "退出";

		public const string MessageStartClient = "客户端已启动，正在搜索服务器: ";

		public readonly static List<MainBtnInfo> AllMainBtnList = new()
		{
			{new MainBtnInfo(Btn_StartOffline,"离线模式开始")},
			{new MainBtnInfo(Btn_StartServer,"选择服务器")},
			{new MainBtnInfo(Btn_StartHost,"创建服务器")},
			{new MainBtnInfo(Btn_ShutDownServer,"断开连接")},
			{new MainBtnInfo(Btn_CmdList,"命令界面")},
			{new MainBtnInfo(Btn_Setting,"设置")},
			{new MainBtnInfo(Btn_Quit,"退出游戏")},
		};

		public const int Btn_StartOffline = 0;
		public const int Btn_StartServer = 1;
		public const int Btn_StartHost = 2;
		public const int Btn_ShutDownServer = 3;
		public const int Btn_CmdList = 4;
		public const int Btn_Setting = 5;
		public const int Btn_Quit = 6;

		public string[] mainBtnListNames = new string[]
		{
			BtnName_StartOffline, BtnName_StartServer, BtnName_StartHost, BtnName_ShutDownServer,
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

		public int[] selectablePlayerIDs = { 1, 2 };
	}
}
