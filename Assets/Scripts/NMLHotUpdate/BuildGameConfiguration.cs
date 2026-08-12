using System;
using Client.Connectivity;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildClientConfiguration", menuName = "Game Client Configuration/BuildConfiguration")]
public class BuildGameConfiguration : ScriptableObject
{
	[Serializable]
	public enum OnlineLevelType
	{
		Offline = 0,
		ConnectToServer = 2
	}

	[Serializable]
	public enum LoadGedType
	{
		Default = 0,
		Local = 1
	}

	public string FacebookAppID;

	public string ContentBaseUrl;

	[SerializeField]
	private string GooglePlayStoreURL = "market://details?id=com.nextgames.android.twd";

	[SerializeField]
	private string AppStoreURL = "https://itunes.apple.com/app/id970417047?mt=8";

	public SignalRClient.SignalRClientLogLevel SignalRLogLevel;

	public OnlineLevelType OnlineLevel = OnlineLevelType.ConnectToServer;

	public LoadGedType LoadGedLevel;

	public bool UseBundles;

	public string PrivacyMode;

	public string UserBehavioral;

	public bool AdditionalCustomLogging
	{
		get
		{
			string branch = GameManager.ActiveBranch;
			if (branch.Contains("test") || branch.Equals("develop") || branch.Equals("temp") || branch.Equals("feature"))
			{
				return true;
			}
			return false;
		}
	}

	public string ConnectionUrl
	{
		get
		{
			string text = "drillerservices.com";
			return "https://twd" + GetBranchStarter() + "." + text;
		}
	}

	public string SecondaryConnectionUrl
	{
		get
		{
			string branch = GameManager.ActiveBranch;
			if (branch.Contains("release"))
			{
				return "https://backup-twd.drillerservices.com";
			}
			return ConnectionUrl;
		}
	}

	public bool ShowDebugMenu
	{
		get
		{
			string branch = GameManager.ActiveBranch;
			if (!branch.Equals("test") && !branch.Equals("develop"))
			{
				return branch.Equals("feature");
			}
			return true;
		}
	}

	public bool ShowStartupMenu
	{
		get
		{
			string branch = GameManager.ActiveBranch;
			if (!branch.Equals("test") && !branch.Equals("develop"))
			{
				return branch.Equals("feature");
			}
			return true;
		}
	}

	public bool LowViolence => GameManager.ActiveBranch.Contains("-lv");

	public bool ConnectedToServer => OnlineLevel >= OnlineLevelType.ConnectToServer;

	public bool LoadLocalGed
	{
		get
		{
			if (ConnectedToServer)
			{
				return LoadGedLevel == LoadGedType.Local;
			}
			return false;
		}
	}

	public bool UseCheatIAPs => true;

	public bool UseCheatIAPsIOS =>  !GameManager.ActiveBranch.Contains("release");

	public bool UseCheatIAPsAndroid => !GameManager.ActiveBranch.Contains("release");

	public bool UseOnlyLocalLocalizations
	{
		get
		{
			string branch = GameManager.ActiveBranch;
			if (!branch.Equals("develop") && !branch.Equals("temp") && !branch.Equals("offline"))
			{
				return branch.Equals("feature");
			}
			return true;
		}
	}

	public string BundleURLScheme
	{
		get
		{
			string branch = GameManager.ActiveBranch;
			switch (branch)
			{
			case "develop":
			case "feature":
				return "twdnomanslanddev";
			case "test":
				return "twdnomanslandtst";
			case "test-lv":
				return "twdnomanslandlvtst";
			case "release":
				return "twdnomansland";
			case "release-lv":
				return "twdnomanslandlv";
			case "staging":
				return "twdnomanslandstg";
			case "offline":
				return "twdnomanslandoff";
			case "preview":
				return "twdnomanslandprv";
			case "temp":
				return "twdnomanslandtmp";
			default:
				return "twdnomansland";
			}
		}
	}

	public string StoreURL => "http://getnomansland.com";

	public bool UnityAdsTestMode => GameManager.ActiveBranch.Contains("release");

	private string GetBranchStarter()
	{
		string branch = GameManager.ActiveBranch;
		switch (branch)
		{
		case "develop":
		case "feature":
		case "offline":
			return "dev";
		case "staging":
		case "staging-pay":
			return "-stg";
		case "test":
		case "test-lv":
			return "-tst";
		case "release-lv":
			return "-lv";
		case "preview":
			return "-prv";
		case "temp":
			return "-tmp";
		default:
			return "";
		}
	}
}
