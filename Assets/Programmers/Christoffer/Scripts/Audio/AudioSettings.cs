using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    FMOD.Studio.EventInstance SFXTest;

    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus SFX;
    float MasterVolume;
    float MusicVolume;
    float SFXVolume;

    void Awake()
    {
        Master = FMODUnity.RuntimeManager.GetBus("");
        Music = FMODUnity.RuntimeManager.GetBus("");
        SFX = FMODUnity.RuntimeManager.GetBus("");
        SFXTest = FMODUnity.RuntimeManager.CreateInstance("");
    }

    void Update()
    {
        Master.setVolume(MasterVolume);
        SFX.setVolume(SFXVolume);
        Master.setVolume(MasterVolume);
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
    }

    public void MusicVolumeLevel(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
    }

    public void SFXVolumeLevel(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;

        FMOD.Studio.PLAYBACK_STATE pb;
        SFXTest.getPlaybackState(out pb);
        if(pb != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
            SFXTest.start();
        }
    }
}
