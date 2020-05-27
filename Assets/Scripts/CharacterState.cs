using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//This is supposed to act as a place to store character variables which
// can be edidted by other objects for the sake of avoiding dependendies.
public enum CHARACTER_CONTROL_STATE { PLAYERCONTROLLED = 0, CUTSCENE, MENU, MENU_NO_MOVEMENT }
public enum ABILITY_FLAG { NULL = 0, INVISSIBLE, SUPERHEARING, STONE, LEVITATE, SLOWFALL, CALM_ALL_FLOWERS, VISSION, TELEPORT }
public static class CharacterState
{
    static CursorUsabilityControll _cursorControll = new CursorUsabilityControll();
    static CHARACTER_CONTROL_STATE _controlState = 0;
    public static CHARACTER_CONTROL_STATE Control_State { get => _controlState; }
    static List<ABILITY_FLAG> _abilityFlags = new List<ABILITY_FLAG>();
    public static float PositionType { get; set; }
    static CharacterState()
    {
        Debug.Log(_abilityFlags.Count);
    }

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
        ActionDelayer.RunAfterDelay(() =>
        {
            _abilityFlags.Remove(flag);
        }, time);
    }
    public static void AddAbilityFlag(string s, float time)
    {
        AddAbilityFlag(GetFlagFromString(s), time);
    }

    //public static void RemoveAbilityFlag(ABILITY_FLAG flag)
    //{
    //    Debug.Log("THIS SHOULDN'T HAPPEN!");
    //    _abilityFlags.Remove(flag);
    //}

    //public static bool RemoveAbilityFlag(string name)
    //{
    //    ABILITY_FLAG flag = GetFlagFromString(name);
    //    if (IsAbilityFlagActive(flag))
    //    {
    //        _abilityFlags.Remove(flag);
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    public static bool IsAbilityFlagActive(ABILITY_FLAG flag)
    {
        return (_abilityFlags.Contains(flag));
    }
    public static bool IsAbilityFlagActive(string name)
    {
        ABILITY_FLAG flag = GetFlagFromString(name);
        if (flag == ABILITY_FLAG.NULL)
        {
            return false;
        }
        return IsAbilityFlagActive(flag);
    }

    public static void SetControlState(CHARACTER_CONTROL_STATE state)
    {
        //Kanske fel ställe att lägga in det på så flytta ifall bad => sparar här!
        if (state != CHARACTER_CONTROL_STATE.PLAYERCONTROLLED)
            SaveSystem.SaveStatic();
        //=====

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
            case "STONE":
                return ABILITY_FLAG.STONE;
            case "LEVITATE":
                return ABILITY_FLAG.LEVITATE;
            case "SLOWFALL":
                return ABILITY_FLAG.SLOWFALL;
            case "CALM":
                return ABILITY_FLAG.CALM_ALL_FLOWERS;
            case "VISION":
                return ABILITY_FLAG.VISSION;
			case "TELEPORT":
				return ABILITY_FLAG.TELEPORT;
			default:
                return ABILITY_FLAG.NULL;
        }
    }
}
