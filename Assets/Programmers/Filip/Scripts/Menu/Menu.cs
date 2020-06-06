using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Settings")]


    [SerializeField] GameObject _menu;
    [SerializeField] GameObject _main;
    [SerializeField] GameObject[] _rest;

    [SerializeField] MenuItemHover[] _changeNames;
    [SerializeField] GameObject _logo;

    [Header("UI Sounds")]
    [EventRef] public string ui_select_Ref;
    [EventRef] public string ui_start_Ref;

    CHARACTER_CONTROL_STATE _previousState;

    bool _onStart = true;

    bool _mainMenu = true;

    void Update()
    {
        if ((_onStart || Input.GetButtonDown(InputKeyWords.CANCEL)) && _menu != null)
        {
            _onStart = false;

            if (CharacterState.Control_State != CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT && CharacterState.Control_State != CHARACTER_CONTROL_STATE.MENU)
            {
                EnterMenu();
            }
            else if (_menu.activeSelf)
            {//If menu object is active, turn it off and set characterState to previousState
                ExitMenu();
            }
        }
    }

    public static void Save()
    {
        float masterVol, musicVol, SFXVol;
        RuntimeManager.GetBus(AudioSettings.BUS_START_PREFIX + AudioSettings.BUS_MASTER).getVolume(out masterVol);
        RuntimeManager.GetBus(AudioSettings.BUS_START_PREFIX + AudioSettings.BUS_MUSIC).getVolume(out musicVol);
        RuntimeManager.GetBus(AudioSettings.BUS_START_PREFIX + AudioSettings.BUS_SFX).getVolume(out SFXVol);


        OptionsSave saveData = new OptionsSave
            (masterVol, musicVol, SFXVol,
             Screen.currentResolution,
             Screen.fullScreen, QualitySettings.GetQualityLevel(),
             QualitySettings.masterTextureLimit, TerrainGenerator.RenderDistanceIndex,
             Player.GetPlayerCamera().fieldOfView,
             Player.GetSensitivity());

        Serialization.Save(Saving.FileNames.SETTINGS, saveData);
    }

    public static void Load()
    {
        OptionsSave saveData = (OptionsSave)Serialization.Load(Saving.FileNames.SETTINGS);

        if (saveData != null)
        {
            RuntimeManager.GetBus(AudioSettings.BUS_START_PREFIX + AudioSettings.BUS_MASTER).setVolume(saveData._masterVol);
            RuntimeManager.GetBus(AudioSettings.BUS_START_PREFIX + AudioSettings.BUS_MUSIC).setVolume(saveData._musicVol);
            RuntimeManager.GetBus(AudioSettings.BUS_START_PREFIX + AudioSettings.BUS_SFX).setVolume(saveData._SFXVol);

            QualitySettings.SetQualityLevel(saveData._graphicsQuality);
            QualitySettings.masterTextureLimit = saveData._textureDetail;
            TerrainGenerator.SetRenderDistanceOnStart(saveData._renderDistance);

            Player.GetPlayerCamera().fieldOfView = saveData._FOV;
            Player.SetSensitivity(saveData._lookSensitivity);
        }
    }


    public void EnterMenu()
    {
        ResetMenu();
        _previousState = CharacterState.Control_State;
        CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT);
        _menu.SetActive(true);
    }

    public void ExitMenu()
    {
        CharacterState.SetControlState(_previousState);
        if (Music_Manager.Instance.StartMenu)
            Music_Manager.Instance.Stop_StartMenuMusic();

        for (int i = 0; i < _changeNames.Length; i++)
        {
            _changeNames[i].ChangeName();
        }

        ResetMenu();
        _menu.SetActive(false);
    }


    private void ResetMenu()
    {
        _main.SetActive(true);

        for (int i = 0; i < _rest.Length; i++)
        {
            _rest[i].SetActive(false);
        }
    }

    public void Quit()
    {
        SaveSystem.SaveStatic();
        Application.Quit();
    }

    public void Restart()
    {
        Music_Manager.Instance.Stop_TriggerMusic();
        if (!Music_Manager.Instance.StartMenu)
            Music_Manager.Instance.Start_StartMenuMusic();
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

    public void Init_UI_Select()
    {
        RuntimeManager.PlayOneShot(ui_select_Ref);
    }

    public void Init_UI_Start()
    {
        if (Music_Manager.Instance.StartMenu)
            RuntimeManager.PlayOneShot(ui_start_Ref);
        else
            Init_UI_Select();
    }
}


[System.Serializable]
public struct MenuNumberString
{
    public float number;
    public string name;
}


[System.Serializable]
public class OptionsSave
{
    public float _masterVol = 1;
    public float _musicVol = 1;
    public float _SFXVol = 1;

    public int _resolutionWidth;
    public int _resolutionHeight;
    public bool _fullScreen;
    public int _graphicsQuality;
    public int _textureDetail;
    public int _renderDistance;

    public float _lookSensitivity = 3;
    public float _FOV = 65;

    public OptionsSave(float masterVol, float musicVol, float SFXVol, Resolution resolution, bool fullScreen, int graphicsQuality, int textureDetail, int renderDistance, float FOV, float lookSensitivity)
    {
        this._masterVol = masterVol;
        this._musicVol = musicVol;
        this._SFXVol = SFXVol;

        this._resolutionWidth = resolution.width;
        this._resolutionHeight = resolution.height;
        this._fullScreen = fullScreen;
        this._graphicsQuality = graphicsQuality;
        this._textureDetail = textureDetail;
        this._renderDistance = renderDistance;

        this._FOV = FOV;
        this._lookSensitivity = lookSensitivity;
    }
}
