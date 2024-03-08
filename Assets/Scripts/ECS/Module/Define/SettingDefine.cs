// 数据定义

using UnityEngine.InputSystem;

namespace WGame.UI
{
	public class SettingDefine : Runtime.Singleton<SettingDefine>
	{
		public const int inputTypeNums = 7;
		public const int attack = 0;
		public const int defense = 1;
		public const int jump = 2;
		public const int step = 3;
		public const int look = 4;
		public const int move = 5;
		public const int interact = 6;

		public const string ComboBox = "ComboBox";
		public const string Toggle = "Toggle";
		public const string Slider = "Slider";

		// public static readonly int[] rangeExposure = new int[2] { -100, 100 };
		public const int Exposure = 0;
		public const int Contrast = 1;
		public const int Saturation = 2;

		public string[] settingTabs = new string[]
		{
			"输入", "显示"
		};

		public string[] inputType = new string[]
		{
			"键盘", "手柄"
		};

		public string[] inputBtns = new string[]
		{
			"移动", "攻击", "防御", "跳跃", "视角", "闪避", "锁定","交互", "特殊"
		};

		public InputAction[] InputActions = new InputAction[inputTypeNums];

		public string[] keyboardInputs = new string[]
		{
			"a", "b", "c", "d"				
		};

		/// <summary>
		/// 待配表
		/// </summary>
		public string[,] grapicConfigs = new string[,]
		{
			{"帧率", ComboBox},
			{"显示帧率", Toggle},
			{"显示消息", Toggle},
			{"亮度", Slider},
			{"对比度", Slider},
			{"饱和度", Slider},
		};

		public string[] fpsListStr = new string[]
		{
			"30帧", "60帧", "120帧", "无限制"
		};

		public int[] fpsList = new int[]
		{
			30, 60, 120, -1
		};
	}
}
