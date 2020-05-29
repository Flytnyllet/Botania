using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
	[SerializeField] Slider _masterVolume;
	[SerializeField] Slider _musicVolume;
	[SerializeField] Slider _sfxVolume;
	void Start()
    {
		_masterVolume.value = (float)Settings.GetVolume(VolumeSliders.Master);
		_musicVolume.value = (float)Settings.GetVolume(VolumeSliders.Music);
		_sfxVolume.value = (float)Settings.GetVolume(VolumeSliders.SFX);

		_masterVolume.onValueChanged.AddListener(delegate {
			Debug.Log($"Master volume changed to {_masterVolume.value}");
			Settings.SetVolume(VolumeSliders.Master, _masterVolume.value); });
		_musicVolume.onValueChanged.AddListener(delegate { Settings.SetVolume(VolumeSliders.Music, _musicVolume.value); });
		_sfxVolume.onValueChanged.AddListener(delegate { Settings.SetVolume(VolumeSliders.SFX, _sfxVolume.value); });
	}

    void Update()
    {
        
    }
}
