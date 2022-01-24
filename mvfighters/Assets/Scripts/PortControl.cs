using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PortControl : MonoBehaviour
{
    [HideInInspector] public int gamepad1ID;
    
    [HideInInspector] public int gamepad2ID;

    [HideInInspector] public bool keyboardOnly1;
    
    [HideInInspector] public bool keyboardOnly2;
    public void ControllerDepplugged()
    {
        
    }

    public bool CheckID(InputAction.CallbackContext context, int playerPort)
    {
        int gamepadID = playerPort == 1 ? MainScript.instance.portController.gamepad1ID : MainScript.instance.portController.gamepad2ID;
        bool isKeyboardOnly = playerPort == 1
            ? MainScript.instance.portController.keyboardOnly1
            : MainScript.instance.portController.keyboardOnly2;
        if (isKeyboardOnly)
        {
            if (context.control.device.name != "Keyboard")
            {
                return false;
            }
        }
        else 
        {
            if (context.control.device.name != "Keyboard")
            {
                if (context.control.device.deviceId != gamepadID)
                    return false;
            }
        }
        return true;
    }

    public void SetupGamepadID(ReadOnlyArray<Gamepad> gamepads)
    {
        keyboardOnly1 = gamepads.Count <= 0;
        keyboardOnly2 = gamepads.Count <= 1;
        if (gamepads.Count > 0)
        {
            gamepad1ID = gamepads[0].deviceId;
        }
        else
        {
            gamepad1ID = -1;
        }
        if (gamepads.Count > 1)
        {
            gamepad2ID = gamepads[1].deviceId;
        }
        else
        {
            gamepad2ID = -1;
        }
        
    }
}
