using System;
using System.Collections.Generic;
using System.Text;

public class LocalDataUtility
{
	protected static int maximumSegmentSize = 262144;

	public static void Save(string key, string value)
	{
		string[] array = SplitString(value, maximumSegmentSize);
		TWDPlayerPrefs.SetInt(key + "_parts", array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			TWDPlayerPrefs.SetString(key + "_" + i, array[i]);
		}
	}

	public static string Load(string key)
	{
		int num = TWDPlayerPrefs.GetInt(key + "_parts");
		if (num == 0)
		{
			return "";
		}
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = TWDPlayerPrefs.GetString(key + "_" + i);
		}
		return JoinString(array);
	}

	private static string[] SplitString(string str, int segmentSize)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < str.Length; i += segmentSize)
		{
			list.Add(str.Substring(i, Math.Min(str.Length - i, segmentSize)));
		}
		return list.ToArray();
	}

	private static string JoinString(string[] parts)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < parts.Length; i++)
		{
			stringBuilder.Append(parts[i]);
		}
		return stringBuilder.ToString();
	}
}
