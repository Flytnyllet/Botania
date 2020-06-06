using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputKeyWords
{
    public static readonly string HORIZONTAL = "Horizontal";
    public static readonly string VERTICAL = "Vertical";

    public static readonly string ACTION_0 = "Action0";

    public static readonly string INVENTORY = "Inventory";
    public static readonly string LORE = "Lore";
    public static readonly string MAP = "Map";
    public static readonly string MAP_CONTROLLER = "Map-Controller";
    public static readonly string FLOWERS = "Flowers";
    public static readonly string ALCHEMY = "Alchemy";
    public static readonly string WHEEL = "Wheel";
    public static readonly string USE_WHEEL = "Use Wheel";
    public static readonly string DUCK = "Duck";
    public static readonly string CAMERA = "Camera";
    public static readonly string SPRINT = "Sprint";
    public static readonly string JUMP = "Jump";
    public static readonly string MOUSE_X = "Mouse X";
    public static readonly string MOUSE_Y = "Mouse Y";
    public static readonly string SUBMIT = "Submit";
    public static readonly string CANCEL = "Cancel";
}

public class GetAxisRawDown
{
    bool _wasTrue = false;
    string _input;
    int _direction;

    public GetAxisRawDown(string input, int direction)
    {
        this._input = input;
        this._direction = direction;
    }

    public bool GetAxisDown()
    {
        if (Input.GetAxisRaw(_input) == _direction)
        {
            if (!_wasTrue)
            {
                _wasTrue = true;
                return true;
            }
            else
                return false;
        }

        _wasTrue = false;
        return false;
    }
}
