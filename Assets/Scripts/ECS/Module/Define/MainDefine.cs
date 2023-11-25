// 数据定义

using System;
using System.Collections.Generic;

namespace WGame.UI
{
	public class MainDefine : Runtime.Singleton<MainDefine>
	{
		public string[] mainBtnListNames = new string[]
		{
			"开始", "命令列表", "设置", "退出"
		};
		
		public string[] mainTopBtnListNames = new string[]
		{
			"选中随机角色", "生成随机角色", "生成武器", "丢弃武器"
		};

		public float joystickDeadRadiusRate = 0.2f;

		public string[] presetCommands = new string[]
		{
			"加入武器", "敌人跟随"
		};
	}
}
