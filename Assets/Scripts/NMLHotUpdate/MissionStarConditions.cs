using System;
using System.Text;

[Serializable]
public class MissionStarConditions
{
	public MissionStarCondition[] Conditions;

	public MissionStarConditions(MissionStarCondition[] conditions)
	{
		Conditions = conditions;
	}

	public override int GetHashCode()
	{
		int num = 0;
		if (Conditions != null)
		{
			for (int i = 0; i < Conditions.Length; i++)
			{
				num ^= Conditions[i].GetHashCode();
			}
		}
		return num;
	}

	public string ConvertToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Conditions.Length; i++)
		{
			if (i != 0)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append(Conditions[i].ConvertToString());
		}
		return stringBuilder.ToString();
	}

	public static MissionStarConditions ConvertFromString(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return new MissionStarConditions(new MissionStarCondition[0]);
		}
		string[] array = s.Split(new char[1] { ',' });
		MissionStarCondition[] array2 = new MissionStarCondition[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = MissionStarCondition.ConvertFromString(array[i]);
		}
		return new MissionStarConditions(array2);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MissionStarConditions missionStarConditions))
		{
			return false;
		}
		if (Conditions == null && missionStarConditions.Conditions == null)
		{
			return true;
		}
		if (Conditions == null && missionStarConditions.Conditions != null)
		{
			return false;
		}
		if (Conditions != null && missionStarConditions.Conditions == null)
		{
			return false;
		}
		if (Conditions.Length != missionStarConditions.Conditions.Length)
		{
			return false;
		}
		for (int i = 0; i < Conditions.Length; i++)
		{
			if (!Conditions[i].Equals(missionStarConditions.Conditions[i]))
			{
				return false;
			}
		}
		return true;
	}
}
