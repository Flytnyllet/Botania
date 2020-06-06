using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public static readonly string BUS_START_PREFIX = "bus:/";
    public static readonly string BUS_MASTER = "Master";
    public static readonly string BUS_MUSIC = "Master/Music";
    public static readonly string BUS_SFX = "Master/SFX";

    FMOD.Studio.Bus _master;
    FMOD.Studio.Bus _music;
    FMOD.Studio.Bus _SFX;

    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _SFXSlider;

    [SerializeField] TMP_Text _masterValue;
    [SerializeField] TMP_Text _musicValue;
    [SerializeField] TMP_Text _SFXValue;


    void Awake()
    {
        _master = RuntimeManager.GetBus(BUS_START_PREFIX + BUS_MASTER);
        _music = RuntimeManager.GetBus(BUS_START_PREFIX + BUS_MUSIC);
        _SFX = RuntimeManager.GetBus(BUS_START_PREFIX + BUS_SFX);
    }

    private void Start()
    {
        float volume;

        _master.getVolume(out volume);
        _masterSlider.value = volume;
        _music.getVolume(out volume);
        _musicSlider.value = volume;
        _SFX.getVolume(out volume);
        _SFXSlider.value = volume;
    }

    public void MasterVolumeLevel()
    {
        _master.setVolume(_masterSlider.value);
        _masterValue.text = GetVolumeLevelString(_masterSlider.value);
    }

    public void MusicVolumeLevel()
    {
        _music.setVolume(_musicSlider.value);
        _musicValue.text = GetVolumeLevelString(_musicSlider.value);
    }

    public void SFXVolumeLevel()
    {
        _SFX.setVolume(_SFXSlider.value);
        _SFXValue.text = GetVolumeLevelString(_SFXSlider.value);
    }

    string GetVolumeLevelString(float value)
    {
        return System.Math.Round(value, 2).ToString();
    }
}
