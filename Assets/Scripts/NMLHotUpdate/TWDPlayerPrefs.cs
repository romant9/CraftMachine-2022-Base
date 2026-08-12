using UnityEngine;

public class TWDPlayerPrefs
{
	public static void Save()
	{
		PlayerPrefs.Save();
	}

	public static void DeleteKey(string key)
	{
		if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
	}

	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		return PlayerPrefs.GetInt(key, defaultValue);
	}

	public static float GetFloat(string key, float defaultValue = 0f)
	{
		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	public static string GetString(string key, string defaultValue = "")
	{
		return PlayerPrefs.GetString(key, defaultValue);
	}

	public static void SetInt(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
	}

	public static void SetFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
	}

	public static void SetString(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
	}

	public static bool TryGetValue(string key, out string value)
	{
		value = PlayerPrefs.GetString(key);
		return string.IsNullOrEmpty(value);
	}

	public static bool GetBool(string key)
	{
		return bool.TryParse(key, out bool result) && result;
	}

	public static int GetInt(string key)
	{
		return int.TryParse(key, out int result) ? result : 0;
	}
}
