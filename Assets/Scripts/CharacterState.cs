using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//This is supposed to act as a place to store character variables which
// can be edidted by other objects for the sake of avoiding dependendies.
public enum CHARACTER_CONTROL_STATE { PLAYERCONTROLLED = 0, CUTSCENE, MENU, MENU_NO_MOVEMENT }
public enum ABILITY_FLAG { NULL, INVISSIBLE, SUPERHEARING }
public static class CharacterState
{
    static CursorUsabilityControll _cursorControll = new CursorUsabilityControll();
    static CHARACTER_CONTROL_STATE _controlState = 0;
    public static CHARACTER_CONTROL_STATE Control_State { get => _controlState; }
    static List<ABILITY_FLAG> _abilityFlags = new List<ABILITY_FLAG>();
    public static float PositionType { get; set; }

    //Förenkla läslighet i tillkopplad kod
    public static bool MayMove
    {
        get
        {
            if (_controlState == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED || _controlState == CHARACTER_CONTROL_STATE.MENU)
            {
                return true;
            }
            return false;
        }
    }
    public static bool MouseLook
    {
        get
        {
            if (_controlState == CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
            {
                return true;
            }
            return false;
        }
    }
    public static void AddAbilityFlag(ABILITY_FLAG flag, float time)
    {
        _abilityFlags.Add(flag);
        ActionDelayer.RunAfterDelay(() => { _abilityFlags.Remove(flag); }, time);
    }
    public static void AddAbilityFlag(string s, float time)
    {
        //Debug.Log(s + " added");
        ABILITY_FLAG flag = GetFlagFromString(s);
        _abilityFlags.Add(flag);
        ActionDelayer.RunAfterDelay(() =>
        {
            //Debug.Log(s + " added");
            _abilityFlags.Remove(flag);
        }, time);
    }


    public static bool IsAbilityFlagActive(ABILITY_FLAG flag)
    {
        return (_abilityFlags.Contains(flag));
    }

    public static void SetControlState(CHARACTER_CONTROL_STATE state)
    {
        _controlState = state;
        _cursorControll.SetMouseState(state);
    }
    public static void SetControlState(int state)
    {
        _controlState = (CHARACTER_CONTROL_STATE)state;
    }

    //This is not thread safe at the moment and just here as a test due to the lack of cooroutines.
    //public static void SetControlState(CHARACTER_CONTROL_STATE startState, CHARACTER_CONTROL_STATE endState, float time)
    //{
    //    _controlState = startState;
    //    Task.Run(async () =>
    //    {
    //        await Task.Delay(System.TimeSpan.FromSeconds(time));
    //        _controlState = endState;
    //    });
    //}
    public static ABILITY_FLAG GetFlagFromString(string s)
    {
        switch (s)
        {
            case "INVISSIBLE":
                return ABILITY_FLAG.INVISSIBLE;
            case "SUPERHEARING":
                return ABILITY_FLAG.SUPERHEARING;
            default:
                return ABILITY_FLAG.NULL;
        }
    }
}
