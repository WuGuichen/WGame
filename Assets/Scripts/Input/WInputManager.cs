using System;
using UnityEngine.InputSystem;
using WGame.Runtime;

public class WInputManager : Singleton<WInputManager>
{
    public int MouseID { get; private set; }
    public bool IsMouse => CurrentDeviceID == MouseID;
    public int KeyboardID { get; private set; }
    public bool IsKeyboard => CurrentDeviceID == KeyboardID;
    public bool IsKeyboardOrMouse => CurrentDeviceID == KeyboardID || CurrentDeviceID == MouseID;
    public int XInputControllerID { get; private set; }
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
