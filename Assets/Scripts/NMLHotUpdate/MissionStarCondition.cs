using System;
using System.Collections.Generic;

[Serializable]
public class MissionStarCondition
{
	public MissionStarsType Type;

	public string Parameter;

	private static Dictionary<string, MissionStarsType> LetterToTypeMapping = new Dictionary<string, MissionStarsType>
	{
		{
			"C",
			MissionStarsType.CompleteMission
		},
		{
			"K",
			MissionStarsType.KillXWalkers
		},
		{
			"M",
			MissionStarsType.MaxTurns
		},
		{
			"H",
			MissionStarsType.NoHitTaken
		},
		{
			"S",
			MissionStarsType.NoStruggle
		}
	};

	public override int GetHashCode()
	{
		return (int)Type ^ Parameter.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		MissionStarCondition missionStarCondition = obj as MissionStarCondition;
		if (Type == missionStarCondition.Type)
		{
			return Parameter.Equals(missionStarCondition.Parameter);
		}
		return false;
	}

	public string ConvertToString()
	{
		foreach (KeyValuePair<string, MissionStarsType> item in LetterToTypeMapping)
		{
			if (item.Value == Type)
			{
				if (string.IsNullOrEmpty(Parameter))
				{
					return item.Key;
				}
				return item.Key + Parameter;
			}
		}
		throw new Exception("Cannot convert " + Type.ToString() + " to string");
	}

	public static MissionStarCondition ConvertFromString(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return null;
		}
		MissionStarCondition missionStarCondition = new MissionStarCondition();
		string key = s.Substring(0, 1);
		if (LetterToTypeMapping.TryGetValue(key, out missionStarCondition.Type))
		{
			if (s.Length > 1)
			{
				missionStarCondition.Parameter = s.Substring(1);
			}
			return missionStarCondition;
		}
		throw new ArgumentException("Cannot convert from string: " + s);
	}
}
