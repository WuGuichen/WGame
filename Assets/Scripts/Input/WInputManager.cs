using System;
using UnityEngine.InputSystem;
using WGame.Runtime;

public class WInputManager : Singleton<WInputManager>
{
    public int MouseID { get; private set; } = -1;
    public bool IsMouse => CurrentDeviceID == MouseID;
    public bool HasMouse => MouseID >= 0;
    public int KeyboardID { get; private set; } = -1;
    public bool HasKeyboard => KeyboardID >= 0;
    public bool IsKeyboard => CurrentDeviceID == KeyboardID;
    public bool IsKeyboardOrMouse => CurrentDeviceID == KeyboardID || CurrentDeviceID == MouseID;
    public int XInputControllerID { get; private set; } = -1;
    public bool HasXInputController => XInputControllerID >= 0;
    public bool IsXInputController => CurrentDeviceID == XInputControllerID;
    public InputDevice CurrentDevice { get; private set; }
    public int CurrentDeviceID { get; private set; } = -1;

    public Action onDeviceChanged;

    public override void InitInstance()
    {
        var device = InputSystem.GetDevice("Mouse");
        if(device != null) 
            MouseID = device.deviceId;
        device = InputSystem.GetDevice("XInputControllerWindows");
        if(device != null) 
            XInputControllerID = device.deviceId;
        device = InputSystem.GetDevice("Keyboard");
        if(device != null) 
            KeyboardID = device.deviceId;
        InputSystem.onDeviceChange += (inputDevice, change) =>
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
                if (inputDevice.name == "XInputControllerWindows")
                {
                    XInputControllerID = inputDevice.deviceId;
                }
                else if(inputDevice.name == "Mouse")
                {
                    MouseID = inputDevice.deviceId;
                }
                else if (inputDevice.name == "Keyboard")
                {
                    KeyboardID = inputDevice.deviceId;
                }
            }
            else if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected)
            {
                if (inputDevice.name == "XInputControllerWindows")
                {
                    XInputControllerID = -1;
                }
                else if(inputDevice.name == "Mouse")
                {
                    MouseID = -1;
                }
                else if (inputDevice.name == "Keyboard")
                {
                    KeyboardID = -1;
                }
            }
        };
        InputSystem.onActionChange += (o, change) =>
        {
            var action = o as InputAction;
            if (action == null || action.activeControl == null)
            {
                return;
            }

            CurrentDevice = action.activeControl.device;
            if (CurrentDevice.deviceId != CurrentDeviceID)
            {
                CurrentDeviceID = CurrentDevice.deviceId;
                // WLogger.Print("Change: " + CurrentDevice.name + ", ID:" + CurrentDeviceID);
                onDeviceChanged?.Invoke();
            }
        };
    }
}
