using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    const string MENU_BUTTON = "Cancel";

    [SerializeField] GameObject _menu;

    CHARACTER_CONTROL_STATE _previousState;

    private void Awake()
    {
        _menu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown(MENU_BUTTON))
        {
            if (CharacterState.Control_State != CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT || CharacterState.Control_State != CHARACTER_CONTROL_STATE.MENU)
            {
                _previousState = CharacterState.Control_State;
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT);
                _menu.SetActive(true);
            }
            else if (_menu.activeSelf)
            {//If menu object is active, turn it off and set characterState to previousState
                ExitMenu();
            }
        }
    }

    void ExitMenu()
    {
        CharacterState.SetControlState(_previousState);
        _menu.SetActive(false);
    }


    public void TEST()
    {
        Debug.LogError("PRESSED!");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
