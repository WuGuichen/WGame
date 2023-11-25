// 数据缓存，数据逻辑操作

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using WGame.Runtime;

namespace WGame.UI
{
	public class SettingModel : Runtime.Singleton<SettingModel>
	{
		private SettingDefine define;
		private MyController _input;

		private const string keyFPS = "GameFPS";
		private const string keyShowFPS = "GameShowFPS";
		public bool isShowFPS;
		private Volume volume;

		public bool ShowStatePad = false;

		public void SetVolume(Volume volume)
		{
			this.volume = volume;
			InitSetting();
		}

		public bool IsShowFPS
		{
			get => isShowFPS;
			set
			{
				if (value == false)
				{
					MainModel.Inst.frames = -1;
				}
				else
				{
					if(isShowFPS == false)
						MainModel.Inst.frames = 0;
				}
				isShowFPS = value;
			}
		}
		private int fps;
		public int FPS
		{
			get => fps;
			set
			{
				fps = value;
				Application.targetFrameRate = fps;
			}
		}

		private const string KeyExposure = "KeyBrightness";
		private const string KeySaturation = "KeySaturation";
		private const string KeyContrast = "KeyContrast";
		private int[] ScreenSettings = new int[8];
		public SettingModel()
		{
			RegisterEvents();
			define = SettingDefine.Inst;
		}

		private void RegisterEvents()
		{
			EventCenter.AddListener(EventDefine.OnJoystickStateChanged, RefreshGamePlayInputState);	
			EventCenter.AddListener(EventDefine.OnGameExit, OnGameExit);
		}

		private void RefreshGamePlayInputState()
		{
			if (MainModel.Inst.IsUseJoystick)
			{
				_input.Disable();	
			}
			else
			{
				_input.Enable();
			}
		}

		public void InitSetting()
		{
			FPS = PlayerPrefs.GetInt(keyFPS, 60);
			IsShowFPS = PlayerPrefs.GetInt(keyShowFPS, 1) == 1;
			SetScreenSetting(SettingDefine.Exposure,PlayerPrefs.GetInt(KeyExposure, 15));
			SetScreenSetting(SettingDefine.Contrast, PlayerPrefs.GetInt(KeyContrast, 10));
			SetScreenSetting(SettingDefine.Saturation,PlayerPrefs.GetInt(KeySaturation, 25));
			
			_input = new MyController();
			// WLogger.Info(_input.GamePlay.Attack.Rebi
			define.InputActions[SettingDefine.attack] = _input.GamePlay.Attack;
			define.InputActions[SettingDefine.jump] = _input.GamePlay.Jump;
			define.InputActions[SettingDefine.defense] = _input.GamePlay.Defense;
			define.InputActions[SettingDefine.step] = _input.GamePlay.Step;
			define.InputActions[SettingDefine.look] = _input.GamePlay.Look;
			define.InputActions[SettingDefine.move] = _input.GamePlay.Move;
			define.InputActions[SettingDefine.interact] = _input.GamePlay.Interact;
			_input.Enable();
		}

		public void SetInputAction(int inputType, int btnType, string key)
		{
			var action = define.InputActions[btnType];
			action.Reset();
			
		}
		
		public void OnSettingTabItemClick(SettingView view, int idx)
		{
			if (idx == 0)
			{
				view.OpenSubView(VDB.SettingInputView);
			}
			else if (idx == 1)
			{
				view.OpenSubView(VDB.SettingGraphicView);
			}
		}

		public int GetSettingType(string type)
		{
			int settingType = -1;
            if (type == "亮度")
            {
                settingType = SettingDefine.Exposure;
            }
            else if (type == "对比度")
            {
                settingType = SettingDefine.Contrast;
            }
            else if (type == "饱和度")
            {
                settingType = SettingDefine.Saturation;
            }

            return settingType;
		}

		public int GetSettingTypeValue(int type)
		{
			return ScreenSettings[type];
		}

		public void SetScreenSetting(int type, int value)
		{
			ScreenSettings[type] = value;
			if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
			{
				if (type == SettingDefine.Exposure)
				{
					colorAdjustments.postExposure.overrideState = value != 0;
					colorAdjustments.postExposure.value = value * 0.01f;
				}
				else if (type == SettingDefine.Contrast)
				{
					colorAdjustments.contrast.overrideState = value != 0;
					colorAdjustments.contrast.value = value;
				}
				else if (type == SettingDefine.Saturation)
				{
					colorAdjustments.saturation.overrideState = value != 0;
					colorAdjustments.saturation.value = value;
				}
			}
		}

		private void OnGameExit()
		{
			PlayerPrefs.SetInt(keyFPS, FPS);
			PlayerPrefs.SetInt(keyShowFPS, isShowFPS ? 1 : 0);
			PlayerPrefs.SetInt(KeyExposure, ScreenSettings[SettingDefine.Exposure]);
			PlayerPrefs.SetInt(KeyContrast, ScreenSettings[SettingDefine.Contrast]);
			PlayerPrefs.SetInt(KeySaturation, ScreenSettings[SettingDefine.Saturation]);
		}

	}
}
