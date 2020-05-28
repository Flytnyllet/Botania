using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    const string MENU_BUTTON = "Cancel";

    [SerializeField] GameObject _menu;

    CHARACTER_CONTROL_STATE _previousState;

    bool _onStart = true;

    void Update()
    {
        if ((_onStart || Input.GetButtonDown(MENU_BUTTON)) && _menu != null)
        {
            _onStart = false;

            if (CharacterState.Control_State != CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT && CharacterState.Control_State != CHARACTER_CONTROL_STATE.MENU)
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

    public void ExitMenu()
    {
        CharacterState.SetControlState(_previousState);
        _menu.SetActive(false);
    }

    public void Quit()
    {
        SaveSystem.SaveStatic();
        Application.Quit();
    }

    public void Restart()
    {
        Destroy(_menu);
        OnStartFadeIn.FadeOut();
        StartCoroutine(RestartWithFade());
    }

    IEnumerator RestartWithFade()
    {
        while (!OnStartFadeIn.Done())
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
