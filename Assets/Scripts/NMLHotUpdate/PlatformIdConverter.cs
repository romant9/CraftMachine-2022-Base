public class PlatformIdConverter
{
	public static string ToPlatformAchievementId(string mainAchievementId)
	{
		if (GameConfiguration.Instance.Config.LowViolence)
		{
			return mainAchievementId + "_LV";
		}
		return mainAchievementId;
	}

	public static string ToPlatformProductId(string mainProductId)
	{
		if (GameConfiguration.Instance.Config.LowViolence)
		{
			return mainProductId + "_LV";
		}
		return mainProductId;
	}

	public static string ToMainAchievementId(string platformAchievementId)
	{
		if (GameConfiguration.Instance.Config.LowViolence && platformAchievementId.EndsWith("_LV"))
		{
			return platformAchievementId.Substring(0, platformAchievementId.Length - 3);
		}
		return platformAchievementId;
	}

	public static string ToMainProductId(string platformProductId)
	{
		if (GameConfiguration.Instance.Config.LowViolence && platformProductId.EndsWith("_LV"))
		{
			return platformProductId.Substring(0, platformProductId.Length - 3);
		}
		return platformProductId;
	}
}
