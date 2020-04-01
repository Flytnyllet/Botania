using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum CHARACTER_CONTROL_STATE { PLAYERCONTROLLED = 0, CUTSCENE }
[CreateAssetMenu(fileName = "CharacterControlState")]
public class CharacterState : ScriptableObject
{
    CHARACTER_CONTROL_STATE _controlState = 0;
    public CHARACTER_CONTROL_STATE CONTROL_STATE
    {
        get { return _controlState; }
    }
    private void Awake()
    {
        _controlState = CHARACTER_CONTROL_STATE.PLAYERCONTROLLED;
    }

    public void SetControlState(CHARACTER_CONTROL_STATE state)
    {
        _controlState = state;
    }
    public void SetControlState(int state)
    {
        _controlState = (CHARACTER_CONTROL_STATE)state;
    }

    //This is not thread safe at the moment and just here as a test due to the lack of cooroutines.
    public void SetControlState(CHARACTER_CONTROL_STATE state, float time)
    {
        CHARACTER_CONTROL_STATE returnState = _controlState;
        _controlState = state;
        Task task = Task.Run(async () =>
        {
            await Task.Delay(System.TimeSpan.FromSeconds(time));
            _controlState = returnState;
        });
    }
}
