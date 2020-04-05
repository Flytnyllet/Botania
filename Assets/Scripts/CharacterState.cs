using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//This is supposed to act as a place to store character variables which
// can be edidted by other objects for the sake of avoiding dependendies.

public enum CHARACTER_CONTROL_STATE { PLAYERCONTROLLED = 0, CUTSCENE }
public static class CharacterState 
{
    static CHARACTER_CONTROL_STATE _controlState = 0;
    public static CHARACTER_CONTROL_STATE Control_State
    {
        get { return _controlState; }
    }

    public static void SetControlState(CHARACTER_CONTROL_STATE state)
    {
        _controlState = state;
    }
    public static void SetControlState(int state)
    {
        _controlState = (CHARACTER_CONTROL_STATE)state;
    }

    //This is not thread safe at the moment and just here as a test due to the lack of cooroutines.
    //public static void SetControlState(CHARACTER_CONTROL_STATE startState, CHARACTER_CONTROL_STATE endState, float time)
    //{
    //    _controlState = startState;
    //    Task task = Task.Run(async () =>
    //    {
    //        await Task.Delay(System.TimeSpan.FromSeconds(time));
    //        _controlState = endState;
    //    });
    //}
}
