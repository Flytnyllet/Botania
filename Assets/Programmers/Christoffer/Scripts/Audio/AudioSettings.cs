using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] string _masterName;
    [SerializeField] string _musicName;
    [SerializeField] string _SFXName;

    FMOD.Studio.Bus _master;
    FMOD.Studio.Bus _music;
    FMOD.Studio.Bus _SFX;

    static readonly string BUS_START_PREFIX = "bus:/";

    void Awake()
    {
        _master = RuntimeManager.GetBus(BUS_START_PREFIX + _masterName);
        _music = RuntimeManager.GetBus(BUS_START_PREFIX + _musicName);
        _SFX = RuntimeManager.GetBus(BUS_START_PREFIX + _SFXName);
    }

    public void MasterVolumeLevel(Slider thisSlider)
    {
        _master.setVolume(thisSlider.value);
    }

    public void MusicVolumeLevel(Slider thisSlider)
    {
        _music.setVolume(thisSlider.value);
    }

    public void SFXVolumeLevel(Slider thisSlider)
    {
        _SFX.setVolume(thisSlider.value);
    }
}
