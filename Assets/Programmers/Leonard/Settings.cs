using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class needs to work together with AudioSettings to actually change volumes, and with OptionsMenu to actually allow it to be changed
/// </summary>
public static class Settings
{
	static float _masterVolume = 0.5f;
	static float _musicVolume = 0.5f;
	static float _sfxVolume = 0.5f;
	static bool _headBobbing = true;
	public static float? GetVolume(VolumeSliders volumeType)
	{
		switch (volumeType)
		{
			case VolumeSliders.Master:
				return _masterVolume;
			case VolumeSliders.Music:
				return _musicVolume;
			case VolumeSliders.SFX:
				return _sfxVolume;
		}
		return null;
	}
	public static bool SetVolume(VolumeSliders volumeType, float value)
	{
		switch(volumeType)
		{
			case VolumeSliders.Master:
				_masterVolume = value;
				return true;
			case VolumeSliders.Music:
				_musicVolume = value;
				return true;
			case VolumeSliders.SFX:
				_sfxVolume = value;
				return true;
		}
		return false;
	}

	public static bool GetToggle(Toggles toggle)
	{
		switch (toggle)
		{
			case Toggles.Headbobbing:
				return _headBobbing;
		}
		return false;
	}

	public static bool SetToggle(Toggles toggle, bool value)
	{
		switch (toggle)
		{
			case Toggles.Headbobbing:
				_headBobbing = value;
				return true;
		}
		return false;
	}
}

public enum VolumeSliders
{
	Master, Music, SFX
}

public enum Toggles
{
	Headbobbing
}