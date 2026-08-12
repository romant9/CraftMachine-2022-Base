using System;
using UnityEngine;

public static class UserPrefsKeys
{
	//Supa User
	public const string Key_User_Mail = "User_Mail";
	public static string User_Mail { get { return TWDPlayerPrefs.GetString("User_Mail"); } set { TWDPlayerPrefs.SetString("User_Mail", value); } }
	public const string Key_User_Pass = "User_Pass";
	public static string User_Pass { get { return TWDPlayerPrefs.GetString("User_Pass"); } set { TWDPlayerPrefs.SetString("User_Pass", value); } }

	//Request
	public const string Key_InstallationID = "InstallationId";
	public static string InstallationID { get { return TWDPlayerPrefs.GetString("InstallationId"); } set { TWDPlayerPrefs.SetString("InstallationId", value); } }
	public const string Key_UserAccountName = "UserAccountName";
	public static string UserAccountName { get { return TWDPlayerPrefs.GetString("UserAccountName"); } set { TWDPlayerPrefs.SetString("UserAccountName", value); } }

	//Player
	public const string Key_UserId = "UserId";
	public static string UserId => TWDPlayerPrefs.GetString("UserId"); // (GoogleID) - G02-D05-db8da...
	public const string Key_ContentBaseUrl = "ContentBaseUrl";

	//string - идентификатор в базе supabase
	public const string Key_Supa_ID = "SupaID";
	public static string Supa_ID { get { return TWDPlayerPrefs.GetString("SupaID"); } set { TWDPlayerPrefs.SetString("SupaID", value); } }

	//string - код игрока в игре и в гильдии - 997273bf5...
	public const string Key_Player_HashID = "HashID";
	public static string Player_HashID { get { return TWDPlayerPrefs.GetString("HashID"); } set { TWDPlayerPrefs.SetString("HashID", value); } }

	//string - уровень игрока
	public const string Key_Player_Level = "PlayerLevel";
	public static int Player_Level { get { return TWDPlayerPrefs.HasKey("PlayerLevel") ? TWDPlayerPrefs.GetInt("PlayerLevel") : 1; } set { TWDPlayerPrefs.SetInt("PlayerLevel", value); } }

	//string (UserId) - код EOS при линковке и при ответе с signalR - G02-D05-db8da...
	public const string Key_Player_GoogleID = "GoogleID";
	public static string Player_GoogleID { get { return TWDPlayerPrefs.GetString("GoogleID"); } set { TWDPlayerPrefs.SetString("GoogleID", value); } }

	//string - имя игрока
	public const string Key_Player_Name = "PlayerName";
	public static string Player_Name { get { return TWDPlayerPrefs.GetString("PlayerName"); } set { TWDPlayerPrefs.SetString("PlayerName", value); } }

	//string - Epic Account ID in the site 1c6b97e...
	public const string Key_Player_EpicAccountID = "EpicAccountID";
	public static string Player_EpicAccountID { get { return TWDPlayerPrefs.GetString("EpicAccountID"); } set { TWDPlayerPrefs.SetString("EpicAccountID", value); } }

	public const string Key_Player_TrialCount = "TrialCount";
	public static int Player_TrialCount { get { return TWDPlayerPrefs.HasKey("TrialCount") ? TWDPlayerPrefs.GetInt("TrialCount") : 15; } set { TWDPlayerPrefs.SetInt("TrialCount", value); } }

	public const string Key_Player_FirstRun = "FirstRun";
	public static string Player_FirstRun { get { return TWDPlayerPrefs.GetString("FirstRun"); } set { TWDPlayerPrefs.SetString("FirstRun", value); } }

	public const string Key_Player_LastRun = "LastRun";
	public static string Player_LastRun { get { return TWDPlayerPrefs.GetString("LastRun"); } set { TWDPlayerPrefs.SetString("LastRun", value); } }

	public const string Key_Player_TimesRun = "TimesRun";
	public static string Player_TimesRun { get { return TWDPlayerPrefs.GetString("TimesRun"); } set { TWDPlayerPrefs.SetString("TimesRun", value); } }

	public const string Key_Player_TimesConnect = "TimesConnect";
	public static string Player_TimesConnect { get { return TWDPlayerPrefs.GetString("TimesConnect"); } set { TWDPlayerPrefs.SetString("TimesConnect", value); } }

	public const string Key_Player_Regged = "Regged";
	public static string Player_Regged { get { return TWDPlayerPrefs.GetString("Regged"); } set { TWDPlayerPrefs.SetString("Regged", value); } }

	public const string Key_Player_Blocked = "Blocked";
	public static string Player_Blocked { get { return TWDPlayerPrefs.GetString("Blocked"); } set { TWDPlayerPrefs.SetString("Blocked", value); } }

	public const string Key_Player_GuildID = "GuildID";
	public static string Player_GuildID { get { return TWDPlayerPrefs.GetString("GuildID"); } set { TWDPlayerPrefs.SetString("GuildID", value); } }

	public const string Key_Player_GuildName = "GuildName";
	public static string Player_GuildName { get { return TWDPlayerPrefs.GetString("GuildName"); } set { TWDPlayerPrefs.SetString("GuildName", value); } }

	public const string Key_Player_Wishes = "Wishes";
	public static string Player_Wishes { get { return TWDPlayerPrefs.GetString("Wishes"); } set { TWDPlayerPrefs.SetString("Wishes", value); } }

	public const string Key_Player_Feedback = "Feedback";
	public static string Player_Feedback { get { return TWDPlayerPrefs.GetString("Feedback"); } set { TWDPlayerPrefs.SetString("Feedback", value); } }

	public const string Key_Player_RegCode = "RegCode";
	public static string Player_RegCode { get { return TWDPlayerPrefs.GetString("RegCode"); } set { TWDPlayerPrefs.SetString("RegCode", value); } }

	public const string Key_Player_ProGuild = "ProGuild";
	public static string Player_ProGuild { get { return TWDPlayerPrefs.GetString("ProGuild"); } set { TWDPlayerPrefs.SetString("ProGuild", value); } }

	public const string Key_Player_ProLink = "ProLink";
	public static string Player_ProLink { get { return TWDPlayerPrefs.GetString("ProLink"); } set { TWDPlayerPrefs.SetString("ProLink", value); } }

	public const string Key_Player_Anonymous = "Anonymous";
	public static string Player_Anonymous { get { return TWDPlayerPrefs.GetString("Anonymous"); } set { TWDPlayerPrefs.SetString("Anonymous", value); } }

	public const string Key_Player_Pin_HashID = "PinHashID";
	public static string Player_Pin_HashID { get { return TWDPlayerPrefs.GetString("PinHashID"); } set { TWDPlayerPrefs.SetString("PinHashID", value); } }

	public const string Key_Player_Pin_GoogleID = "PinGoogleID";
	public static string Player_Pin_GoogleID { get { return TWDPlayerPrefs.GetString("PinGoogleID"); } set { TWDPlayerPrefs.SetString("PinGoogleID", value); } }

	public const string Key_Player_Pin_Name = "PinName";
	public static string Player_Pin_Name { get { return TWDPlayerPrefs.GetString("PinName"); } set { TWDPlayerPrefs.SetString("PinName", value); } }

	public const string Key_Player_Pin_EpicAccountID = "PinEpicAccountID";
	public static string Player_Pin_EpicAccountID { get { return TWDPlayerPrefs.GetString("PinEpicAccountID"); } set { TWDPlayerPrefs.SetString("PinEpicAccountID", value); } }

	public const string Key_Game_Version = "GameVersion";
	public static string Game_Version { get { return TWDPlayerPrefs.GetString("GameVersion"); } set { TWDPlayerPrefs.SetString("GameVersion", value); } }

	public const string Key_Data_Url = "DataUrl";
	public static string Data_Url { get { return TWDPlayerPrefs.GetString("DataUrl"); } set { TWDPlayerPrefs.SetString("DataUrl", value); } }
	public static int Data_Url_Index { get { return TWDPlayerPrefs.GetInt("DataUrlIndex"); } set { TWDPlayerPrefs.SetInt("DataUrlIndex", value); } }

	//Mod
	public const string Key_PlusOneFix = "PlusOneFix"; //bool
	public const string Key_IsOpenSevenDays = "IsOpenSevenDays"; //bool
	public const string Key_IsFreeAll = "IsFreeAll"; //bool
	public const string Key_LoadGuildsFromStart = "IsLoadGuilds"; //bool
	public const string Key_LoadLocalGed = "IsLoadLocalGed"; //bool
	public const string Key_IsVPN = "IsVPN";

	//The Game
	public const string Key_RunLocationID = "RunLocationID"; //string
	public const string Key_StreamingAssetsPath = "StreamingAssetsPath"; //bool
	public const string Key_IsOffAnalyticsManager = "IsOffAnalyticsManager"; //bool

	//
	public const string Key_LastState = "LastState";
	public const string Key_ContentFileID = "ContentFileID";
	public const string Key_ContentSheetID = "ContentSheetID";

	public const string TimeFormat = "yyyy-MM-dd HH:mm";

	public static string UserDeviceName
	{
#if MOBILE
		get { return SystemInfo.operatingSystemFamily + '\n' + SystemInfo.deviceModel + '\n' + SystemInfo.deviceName; }
		//public virtual string UserName { [Android.Runtime.Register("getUserName", "()Ljava/lang/String;", "GetGetUserNameHandler")] get; }
#else
		get { return SystemInfo.operatingSystemFamily + '\n' + SystemInfo.deviceModel + '\n' + SystemInfo.deviceName; }
		//get { return Environment.UserName; }
#endif
	}

#if UNITY_POSTGRES
	public static User GetNullUser()
	{
		bool isRegged = false;
		bool isBlocked = false;
		bool isProGuild = false;
		bool isProLink = false;

		bool IsPinned = false;
		if (TWDPlayerPrefs.TryGetValue(Player_Pin_HashID, out string pin_HashID))
		{
			IsPinned = true;
			isRegged = TWDPlayerPrefs.TryGetValue(Player_Pin_Regged, out string pregged) && bool.Parse(pregged);
			isBlocked = TWDPlayerPrefs.TryGetValue(Player_Pin_Blocked, out string pblocked) && bool.Parse(pblocked);
			isProGuild = TWDPlayerPrefs.TryGetValue(Player_Pin_ProGuild, out string pproGuild) && bool.Parse(pproGuild);
			isProLink = TWDPlayerPrefs.TryGetValue(Player_Pin_ProLink, out string pproLink) && bool.Parse(pproLink);
		}

		return new User()
		{
			HashID =  TWDPlayerPrefs.GetString(Player_HashID) ?? "null",
			PlayerName = TWDPlayerPrefs.GetString(Player_Name) ?? "null",
			FirstRun = TWDPlayerPrefs.GetString(Player_FirstRun) ?? MyTools.DateTimeToTimeString(DateTime.Now.ToLocalTime()),
			LastRun = TWDPlayerPrefs.GetString(Player_LastRun) ?? MyTools.DateTimeToTimeString(DateTime.Now.ToLocalTime()),
			TimesRun = TWDPlayerPrefs.GetInt(Player_TimesRun),
			TimesConnect = TWDPlayerPrefs.GetInt(Player_TimesConnect),

			Regged = IsPinned ? isRegged : TWDPlayerPrefs.TryGetValue(Player_Regged, out string regged) && bool.Parse(regged),
			Blocked = IsPinned ? isBlocked : TWDPlayerPrefs.TryGetValue(Player_Blocked, out string blocked) && bool.Parse(blocked),
			ProGuild = IsPinned ? isProGuild : TWDPlayerPrefs.TryGetValue(Player_ProGuild, out string proGuild) && bool.Parse(proGuild),
			ProLink = IsPinned ? isProLink : TWDPlayerPrefs.TryGetValue(Player_ProLink, out string proLink) && bool.Parse(proLink),

			GoogleId = TWDPlayerPrefs.GetString(Player_GoogleID) ?? "null",
			EpicId = TWDPlayerPrefs.GetString(Player_EpicAccountID) ?? "null",
			DeviceInfo = UserDeviceName,
			GuilName = TWDPlayerPrefs.GetString(Player_GuildName) ?? "null",
			GuildId = TWDPlayerPrefs.GetString(Player_GuildID) ?? "null",
			Country = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
			Whishes = "null",
			Feedback = "null",
			ClientVersion = OfflineManager.ClientVersion,
			ModVersion = Application.version,
			RegCode = TWDPlayerPrefs.TryGetValue(Player_HashID, out string hashID) ? GeneratedCode(hashID) : 0,
			Linked_HashID = IsPinned ? pin_HashID : "null"
		};
	}

	public static User GetPostgreUser(List<object> data)
	{
		return new User()
		{
			HashID = data[0] as string,
			PlayerName = data[1] as string,
			FirstRun = data[2] as string,
			LastRun = data[3] as string,
			TimesRun = int.Parse(data[4] as string),
			TimesConnect = int.Parse(data[5] as string),

			Regged = bool.Parse(data[6] as string),
			Blocked = bool.Parse(data[7] as string),
			ProGuild = bool.Parse(data[8] as string),
			ProLink = bool.Parse(data[9] as string),

			GoogleId = data[10] as string,
			EpicId = data[11] as string,
			DeviceInfo = data[12] as string,
			GuilName = data[13] as string,
			GuildId = data[14] as string,
			Country = data[15] as string,
			Whishes = data[16] as string,
			Feedback = data[17] as string,
			ClientVersion = data[18] as string,
			ModVersion = data[19] as string,
			RegCode = int.Parse(data[20] as string),
			Linked_HashID = data[21] as string
		};
	}

#endif

	public static long GeneratedCode(string hashID)
	{
		DebugTWD.Log("HashId: " + hashID);

		long state = Math.Abs(hashID.GetHashCode());
		DebugTWD.Log("State: " + state);

		for (int i = 0; i < 3; i++)
		{
			state = (state * 1103515245 + 12345) & 0x7FFFFFFF;
		}

		DebugTWD.Log("RegCode: " + state);
		return state;
	}
}
