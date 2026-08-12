using UnityEngine;

public class PlatformInfo
{
	private static PlatformInfoBase info;

	public static TargetPlatform CurrentPlatform => TargetPlatform.Unknown;

	public static MarketPlace CurrentMarketPlace
	{
		get
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return MarketPlace.AppStore;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				return MarketPlace.GooglePlay;
			}
			return MarketPlace.None;
		}
	}

	static PlatformInfo()
	{
		if (CurrentPlatform == TargetPlatform.Android)
		{
			info = new PlatformInfoAndroid();
		}
		else if (CurrentPlatform == TargetPlatform.iOS)
		{
			info = new PlatformInfoIOS();
		}
	}

	public static bool HasFlag(PlatformFlag flag)
	{
		if (info != null)
		{
			return info.HasFlag(flag);
		}
		return false;
	}

	public static bool GetLimitedScreenSize(out int w, out int h)
	{
		if (info != null)
		{
			return info.GetLimitedScreenSize(out w, out h);
		}
		w = (h = 0);
		return false;
	}
}
