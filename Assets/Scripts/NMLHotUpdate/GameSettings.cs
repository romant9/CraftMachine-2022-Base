public class GameSettings
{
	public delegate void SettingsChange(string change, object args);

	public static string SoundFxKey = "SoundFx";

	public static string MusicKey = "Music";

	public static string CombatSpeedUpKey = "CombatSpeedUp";

	public SettingsChange OnSettingsChange;

	public bool IPodPlaying
	{
		get
		{
			return false;
		}
		set
		{
			if (OnSettingsChange != null)
			{
				OnSettingsChange("sound", SoundFxOn);
				OnSettingsChange("music", MusicOn);
			}
		}
	}

	public bool SoundFxOn
	{
		get
		{
			return true;
		}
		set
		{
			SetOn(SoundFxKey, value);
		}
	}

	public float SoundFxVolume
	{
		get
		{
			return TWDPlayerPrefs.GetFloat("SoundFxVolume", 1f);
		}
		set
		{
			TWDPlayerPrefs.SetFloat("SoundFxVolume", value);
		}
	}

	public float MusicVolume
	{
		get
		{
			return TWDPlayerPrefs.GetFloat("MusicVolume", 1f);
		}
		set
		{
			TWDPlayerPrefs.SetFloat("MusicVolume", value);
		}
	}

	public bool MusicOn
	{
		get
		{
			return true;
		}
		set
		{
			SetOn(MusicKey, value);
		}
	}

	public bool CombatSpeedUp
	{
		get
		{
			return IsOn(CombatSpeedUpKey, 0);
		}
		set
		{
			SetOn(CombatSpeedUpKey, value);
		}
	}

	public bool VSync
	{
		get
		{
			return TWDPlayerPrefs.GetInt("VSync", 1) == 1;
		}
		set
		{
			if (value)
			{
				TWDPlayerPrefs.SetInt("VSync", 1);
			}
			else
			{
				TWDPlayerPrefs.SetInt("VSync", 0);
			}
		}
	}

	private bool IsOn(string id, int defaultValue = 1)
	{
		if (TWDPlayerPrefs.GetInt(id, defaultValue) == 1)
		{
			return true;
		}
		return false;
	}

	private void SetOn(string id, bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(id, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(id, 0);
		}
		if (OnSettingsChange != null)
		{
			OnSettingsChange(id, on);
		}
	}
}
