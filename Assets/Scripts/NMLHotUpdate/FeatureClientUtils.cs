using TWDModel;

public static class FeatureClientUtils
{
	public static bool IsEnabledForThisClient(this Feature feature)
	{
		if (feature.Enabled && (string.IsNullOrEmpty(feature.UpperVersionLimit) || feature.UpperVersionLimit == "N/A" || new GameVersion(feature.UpperVersionLimit).CompareTo(ClientUtils.ClientVersion) >= 0))
		{
			return true;
		}
		return false;
	}
}
