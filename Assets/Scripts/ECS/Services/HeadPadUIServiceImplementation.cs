using DG.Tweening;
using FairyGUI;
using UnityEngine;
using UnityTimer;
using WGame.Attribute;
using WGame.UI;
using WGame.UI.Character;

public class HeadPadUIServiceImplementation : ICharacterUIService
{
    private bool isHeadPadActive;
    private bool isHeadPadVisible;
    private FUI_HeadPad headPad;
    private FUI_MessagePad messagePad;
    private int totalValue = 0;
    private Timer showTimer;

    public HeadPadUIServiceImplementation()
    {
        headPad = FUI_HeadPad.CreateInstance();
        messagePad = FUI_MessagePad.CreateInstance();
        GRoot.inst.AddChild(headPad);
        GRoot.inst.AddChild(messagePad);
        SetMessage(null);
        headPad.visible = true;
        headPad.touchable = false;
        messagePad.touchable = false;
        headPad.ShowValue("");
        isHeadPadActive = true;
        headPad.hpBar.alpha = 0;
    }

    public bool IsActive
    {
        get => isHeadPadActive;
        set
        {
            isHeadPadActive = value;
            headPad.visible = value;
            if(value)
                Show();
            else
                Hide();
        }
    }

    public void UpdateUI(GameEntity entity)
    {
        if (messagePad.alpha > 0)
        {
            messagePad.position = headPad.position + new Vector3(0, 3f, 0);
            if (SettingModel.Inst.ShowStatePad == false)
                messagePad.alpha = 0;
        }
        if (!isHeadPadVisible)
            return;
        var gameView = entity.gameViewService.service;
        var pos = HitTestContext.cachedMainCamera.WorldToScreenPoint(gameView.HeadPos);
        if (pos.z >= 0)
            pos.y = Screen.height - pos.y;
        else
            pos.y = 999999;
        headPad.position = GRoot.inst.GlobalToLocal(new Vector2(pos.x, pos.y));
    }

    public void OnDead(GameEntity entity)
    {
        Hide();
        SetMessage();
    }
    public void Destroy(GameEntity entity)
    {
        headPad.Dispose();
        if (entity.hasAttribute)
        {
            var attribute = entity.attribute.value;
            attribute.CancelEvent(WAttrType.MaxHP, SetMaxHPValue);
            attribute.CancelEvent(WAttrType.CurHP, SetHPValue);
        }
    }

    public void RegisterEvent(WAttribute attribute)
    {
        attribute.RegisterEvent(WAttrType.MaxHP, SetMaxHPValue);
        attribute.RegisterEvent(WAttrType.CurHP, SetHPValue);
        headPad.SetValue(attribute.Get(WAttrType.CurHP));
        headPad.SetMaxHP(attribute.Get(WAttrType.MaxHP));
    }

    public void Show(float time = 0f)
    {
        isHeadPadVisible = true;
        headPad.show.Play();
        showTimer?.Cancel();
        if (time > 0)
        {
            showTimer = Timer.Register(time, () =>
            {
                Hide();
            });
        }
    }

    public void Hide(float time = 0f)
    {
        showTimer?.Cancel();
        if (time > 0)
        {
            showTimer = Timer.Register(time, (() =>
            {
                headPad.show.PlayReverse(1, 0, () =>
                {
                    totalValue = 0;
                    headPad.ShowValue("");
                    showTimer = null;
                    isHeadPadVisible = false;
                });
            }));
        }
        else
        {
            headPad.show.PlayReverse(1, 0, () =>
            {
                totalValue = 0;
                headPad.ShowValue("");
                showTimer = null;
                isHeadPadVisible = false;
            });
        }
    }

    public void SetMessage(string msg=null)
    {
        if (msg == null)
        {
            messagePad.alpha = 0;
            return;
        }
        messagePad.SetMessage(msg);
        messagePad.alpha = 1;
    }

    public void SetHPValue(WaEventContext context)
    {
        Show(5f);
        if (context.changedValue < 0)
            totalValue += -context.changedValue;
        headPad.SetValue(context.value);
        headPad.ShowValue(totalValue.ToString());
    }

    public void SetMaxHPValue(WaEventContext context)
    {
        headPad.SetMaxHP(context.value);
    }
}
