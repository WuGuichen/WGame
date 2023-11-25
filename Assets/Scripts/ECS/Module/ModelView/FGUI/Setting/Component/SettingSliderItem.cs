using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace WGame.UI.Setting
{
    public partial class FUI_SettingSliderItem
    {
        private EventCallback0 onValueChanged;
        private int settingType;
        public void SetData(int idx, string type, SettingModel model)
        {
            this.slider.max = 100;
            this.slider.min = -100;
            onValueChanged = OnValueChanged;
            settingType = model.GetSettingType(type);
            this.slider.value = model.GetSettingTypeValue(settingType);
            slider.onChanged.Add(onValueChanged);
            this.title.text = type;
        }

        private void OnValueChanged()
        {
            SettingModel.Inst.SetScreenSetting(settingType, (int)slider.value);
        }
    }
}