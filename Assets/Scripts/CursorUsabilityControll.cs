using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CursorUsabilityControll
{
    public void SetMouseState(CHARACTER_CONTROL_STATE state)
    {
        switch (state)
        {
            case CHARACTER_CONTROL_STATE.PLAYERCONTROLLED:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case CHARACTER_CONTROL_STATE.MENU:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case CHARACTER_CONTROL_STATE.CUTSCENE:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            default:
                Debug.Log("CharacterControlState does not have mouse switch case defined, defaulting to menu case");
                SetMouseState(CHARACTER_CONTROL_STATE.MENU);
                break;
        }
    }
}
