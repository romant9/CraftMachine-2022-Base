using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel.ContentTypes;
using TWDModel;
using UnityEngine;

public class NotificationUtils
{
	private class NotificationData
	{
		public string Text;

		public double InSeconds;

		public string UserData;

		public int IconBadgeNumber;
	}

	private static List<NotificationData> notificationDatas = new List<NotificationData>();

	public const int MaxReminderNotificationCount = 10;

	private static void SaveNotification(string text, double inSeconds, string userData, int iconBadgeNumber = 1)
	{
		NotificationData notificationData = new NotificationData();
		notificationData.Text = text;
		notificationData.InSeconds = inSeconds;
		notificationData.UserData = userData;
		notificationData.IconBadgeNumber = iconBadgeNumber;
		notificationDatas.Add(notificationData);
	}

	private static void ScheduleNotification(NotificationData notificationData)
	{
		if (!(notificationData.InSeconds < 5.0))
		{
			DateTime.UtcNow.AddMilliseconds(notificationData.InSeconds * 1000.0).ToLocalTime();
		}
	}

	private static void CancelNotification(string userData)
	{
	}

	public static void CancelAllLocalNotifications()
	{
		notificationDatas.Clear();
	}

	public static void ResetAppBadgeIcon()
	{
	}

	public static void ScheduleSavedNotifications()
	{
		notificationDatas.Sort((NotificationData x, NotificationData y) => x.InSeconds.CompareTo(y.InSeconds));
		int num = 0;
		for (int num2 = 0; num2 < notificationDatas.Count; num2++)
		{
			NotificationData notificationData = notificationDatas[num2];
			if (notificationData.IconBadgeNumber != -1)
			{
				num += notificationData.IconBadgeNumber;
			}
			notificationData.IconBadgeNumber = num;
			ScheduleNotification(notificationData);
		}
	}

	public static void ScheduleNotificationForBuilding(BuildingModel building)
	{
		if (building != null && building.IsUpgrading)
		{
			SaveNotification(GetBuildingUpgradedText(building.BuildingType.Name), (double)building.UpgradeTimer / 1000.0, "Building");
		}
	}

	public static void ScheduleNotificationForSurvivor(SurvivorModel survivor)
	{
		if (survivor != null && survivor.TimedActionModel != null && survivor.IsUpgrading())
		{
			SaveNotification(GetSurvivorUpgradedText(survivor), (double)survivor.TimedActionModel.MillisecondsTillCompletion / 1000.0, "Survivor");
		}
	}

	public static void ScheduletNotificationForEquipment(EquipmentItemModel equipmentItem)
	{
		if (equipmentItem != null && equipmentItem.TimedActionModel != null && equipmentItem.IsUpgrading())
		{
			SaveNotification(GetEquipmentUpgradedText(equipmentItem), (double)equipmentItem.TimedActionModel.MillisecondsTillCompletion / 1000.0, "Equipment");
		}
	}

	public static string GetBuildingUpgradedText(BuildingModel building)
	{
		return LocalizationManager.GetText("LocalNotification.BuildingUpgraded{BuildingName}", HelpersLocalization.GetBuildingName(building));
	}

	public static string GetBuildingUpgradedText(string buildingType)
	{
		return LocalizationManager.GetText("LocalNotification.BuildingUpgraded{BuildingName}", HelpersLocalization.GetBuildingName(buildingType));
	}

	public static string GetSurvivorUpgradedText(SurvivorModel survivor)
	{
		return LocalizationManager.GetText("LocalNotification.SurvivorUpgraded{SurvivorName}", survivor.Name);
	}

	public static string GetEquipmentUpgradedText(EquipmentItemModel equipmentItem)
	{
		return LocalizationManager.GetText("LocalNotification.EquipmentUpgraded{EquipmentrName}", HelpersLocalization.GetEquipmentName(equipmentItem));
	}

	public static void AddGasFullNotification()
	{
		if (GameManager.Instance.playerModel.Camp != null && GameManager.Instance.playerModel.Camp.GetCarExplorationLevel() > 0)
		{
			CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.ReplayToken);
			if (currency.MillisecondsToFullRecharge != 0L)
			{
				CancelGasFullNotification();
				SaveNotification(LocalizationManager.GetText("LocalNotification.GasFull"), (double)currency.MillisecondsToFullRecharge / 1000.0, "Gas");
			}
		}
	}

	public static void CancelGasFullNotification()
	{
		CancelNotification("Gas");
	}

	public static void AddFreeCallNotification()
	{
		if (GameManager.Instance.playerModel == null)
		{
			return;
		}
		PhoneCallModel phoneCall = GameManager.Instance.playerModel.PhoneCall;
		if (phoneCall == null)
		{
			return;
		}
		for (int i = 0; i < phoneCall.MillisecondsTillFreeCall.Length; i++)
		{
			if (phoneCall.MillisecondsTillFreeCall[i] > 0)
			{
				SaveNotification(LocalizationManager.GetText("LocalNotification.FreeCall" + i), (double)phoneCall.MillisecondsTillFreeCall[i] / 1000.0 + (double)GameManager.Instance.gameEconomyData.ConfigData.FreeCallNotificationDelay, "FreeCall" + i);
			}
		}
	}

	public static void AddAdsRefreshedNotification()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && !playerModel.IsVideoAdRewardAvailable(AdUsage.CinemaReward))
		{
			long videoAdAvailabilityTimeByType = playerModel.GetVideoAdAvailabilityTimeByType(AdUsage.CinemaReward);
			videoAdAvailabilityTimeByType += 10000;
			SaveNotification(LocalizationManager.GetText("LocalNotification.CinemaAdsRefreshed"), (double)videoAdAvailabilityTimeByType / 1000.0, "CinemaAdsRefreshed");
		}
	}

	public static void AddLootKeyRefreshNotification()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return;
		}
		int lootKeySoftCap = playerModel.ActivityManager.GetLootKeySoftCap(playerModel.gameEconomyData.ConfigData);
		if (playerModel.GetCurrencyAmount(CurrencyType.LootKeys) < lootKeySoftCap && playerModel.LootKeysFirstSpentTime > 0)
		{
			long num = playerModel.gameEconomyData.ConfigData.LootKeyRefreshRate - (playerModel.UtcTimeStamp - playerModel.LootKeysFirstSpentTime);
			if (num > 10000)
			{
				SaveNotification(LocalizationManager.GetText("LocalNotification.LootKeyRefresh"), (double)num / 1000.0, "LootKeyRefresh");
			}
		}
	}

	public static void AddOutpostSeasonEnding()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return;
		}
		OutpostSeason currentOutpostSeason = playerModel.CurrentOutpostSeason;
		if (currentOutpostSeason != null)
		{
			long num = currentOutpostSeason.EndTimeMilliseconds - playerModel.UtcTimeStamp;
			long num2 = 86400000L;
			if (num > num2)
			{
				SaveNotification(LocalizationManager.GetText("LocalNotification.OneDayToSeasonChange"), (double)(num - num2) / 1000.0, "OneDayToSeasonChange");
			}
		}
	}

	public static void AddChallengeNotifications()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return;
		}
		GuildModel guildModel = playerModel.GuildModel;
		if (guildModel == null || guildModel.PreviousChallengeStars <= 0)
		{
			return;
		}
		WeeklyChallengeModel weeklyChallenge = playerModel.WeeklyChallenge;
		if (weeklyChallenge == null)
		{
			return;
		}
		bool flag = false;
		if (!weeklyChallenge.Finished)
		{
			long num = weeklyChallenge.CurrentDefinition.EndTimeMilliseconds - playerModel.UtcTimeStamp;
			long num2 = 86400000L;
			if (num > num2)
			{
				flag = true;
				SaveNotification(LocalizationManager.GetText("LocalNotification.OneDayToChallengeEnd"), (double)(num - num2) / 1000.0, "OneDayToChallengeEnd");
			}
		}
		WeeklyChallenge nextWeeklyChallenge = weeklyChallenge.NextWeeklyChallenge;
		if (nextWeeklyChallenge != null && !flag)
		{
			long num3 = nextWeeklyChallenge.StartTimeMilliseconds - playerModel.UtcTimeStamp;
			SaveNotification(LocalizationManager.GetText("LocalNotification.NewChallengeStarted"), (double)num3 / 1000.0, "NewChallengeStarted");
		}
	}

	public static void AddGuildBattleNotifications()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return;
		}
		GuildModel guildModel = playerModel.GuildModel;
		if (guildModel == null || GuildWarHelper.IsLockedByCouncilLevelOrTutorial())
		{
			return;
		}
		long num = 0L;
		if (!GuildWarHelper.IsWarOngoing() && guildModel.GuildWarModel.FindNextGuildWar(playerModel.UtcTimeStamp) != null)
		{
			long num2 = GuildWarHelper.GetTimeLeftToNextWar() + GvGModelHelper.NotificationDelayInMilliseconds(guildModel.Id, GameManager.Instance.gameEconomyData.GuildWarConfig.NotificationDelayInSeconds);
			SaveNotification(LocalizationManager.GetText("LocalNotification.GvgWarStarted"), (double)num2 / 1000.0, "GvgWarStarted");
		}
		if (GuildWarHelper.IsSeasonOngoing() && GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.GuildBattleRP) > 0)
		{
			long num3 = 86400000L;
			long timeLeftToNextSeason = GuildWarHelper.GetTimeLeftToNextSeason();
			if (timeLeftToNextSeason > num3 * 3)
			{
				int num4 = 3;
				num = timeLeftToNextSeason - num3 * 3;
				SaveNotification(LocalizationManager.GetText("LocalNotification.GvgSeasonPreEnd{daysLeft}", num4), (double)num / 1000.0, "GvgSeasonPreEnd");
			}
			if (timeLeftToNextSeason > num3)
			{
				num = timeLeftToNextSeason - num3;
				SaveNotification(LocalizationManager.GetText("LocalNotification.GvgSeasonPreEndNow"), (double)num / 1000.0, "GvgSeasonPreEnd");
			}
		}
	}

	public static void AddReminderNotifications()
	{
		CancelReminderNotifications();
		ConfigData configData = GameManager.Instance.gameEconomyData.ConfigData;
		if (configData.NotifyPlayersAfterDays == null)
		{
			return;
		}
		DateTime now = DateTime.Now;
		DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, configData.NotifyPlayersAtLocalHour, 0, 0);
		int num = Mathf.Min(configData.NotifyPlayersAfterDays.Count, 10);
		for (int i = 0; i < num; i++)
		{
			int num2 = configData.NotifyPlayersAfterDays[i];
			if (num2 > 0)
			{
				TimeSpan timeSpan = dateTime.AddDays(num2).Subtract(now);
				int num3 = i;
				if (num3 >= configData.NotifyPlayersLocalizationKey.Count)
				{
					num3 = configData.NotifyPlayersLocalizationKey.Count - 1;
				}
				SaveNotification(LocalizationManager.GetText(configData.NotifyPlayersLocalizationKey[num3]), timeSpan.TotalSeconds, "Reminder_" + i, 0);
			}
		}
	}

	public static void CancelReminderNotifications()
	{
		for (int i = 0; i < 10; i++)
		{
			CancelNotification("Reminder_" + i);
		}
	}

	public static void AddTradeGoodShopNotification()
	{
		ConfigData configData = GameManager.Instance.gameEconomyData.ConfigData;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (configData.TradeGoodShopRestockNotificationEnabled)
		{
			long result = 0L;
			if (long.TryParse(TWDPlayerPrefs.GetString("TradeGoodShopVisitTime", "0"), out result) && result > playerModel.LastTradeShopRefreshTime)
			{
				SaveNotification(LocalizationManager.GetText("LocalNotification.TradegoodsRestocked"), playerModel.GetTimeLeftToTradeShopRefresh() / 1000, "TradeGoodShopRestock");
			}
		}
	}

	public static void AddBlackMarketNotifications()
	{
		if (GameManager.Instance.Blackboard.IsToggleOn("Toggle.BlackMarketNotifications"))
		{
			string textId = "LocalNotification.NewBlackMarketDeals";
			PlayerModel playerModel = GameManager.Instance.playerModel;
			for (int i = 0; i < GameManager.Instance.playerModel.BlackMarket.Slots.Length; i++)
			{
				BlackMarketHeroSlot blackMarketHeroSlot = GameManager.Instance.playerModel.BlackMarket.Slots[i];
				SaveNotification(LocalizationManager.GetText(textId), (double)(blackMarketHeroSlot.NextUpdate - playerModel.UtcTimeStamp) / 1000.0, "BlackMarketRefresh" + i);
			}
		}
	}

	public static void ScheduleDailyLoginNotifications()
	{
		DailyLoginCampaignModel dailyLoginCalendar = GameManager.Instance.playerModel.DailyLoginCalendar;
		if (dailyLoginCalendar != null && dailyLoginCalendar.IsInitialized && !dailyLoginCalendar.IsCompleted)
		{
			float num = 600000f;
			float num2 = (float)dailyLoginCalendar.NextRewardTime - num - (float)GameManager.Instance.playerModel.UtcTimeStamp;
			if (dailyLoginCalendar.CanClaimRewardForActiveDay() || num2 < 0f)
			{
				SaveNotification(LocalizationManager.GetText("LocalNotification.DailyLoginCalendarReady"), 3600.0, "DailyLoginCalendar");
			}
			else
			{
				SaveNotification(LocalizationManager.GetText("LocalNotification.DailyLoginCalendar"), num2 / 1000f, "DailyLoginCalendar");
			}
		}
	}

	public static void AddBattlePassNotifications()
	{
		BattlePassModel battlePass = GameManager.Instance.playerModel.BattlePass;
		if (battlePass == null || !battlePass.IsSeasonActive)
		{
			return;
		}
		long currentSeasonStartDate = battlePass.CurrentSeasonStartDate;
		List<BattlePassNotificationDefinition> list = (battlePass.IsBeginnerBattlePass ? GameManager.Instance.gameEconomyData.BattlePassNotificationDefinitions.Where((BattlePassNotificationDefinition x) => x.BattlePassType == "Beginner").ToList() : GameManager.Instance.gameEconomyData.BattlePassNotificationDefinitions.Where((BattlePassNotificationDefinition x) => x.BattlePassType == "Normal").ToList());
		for (int num = 0; num < list.Count; num++)
		{
			BattlePassNotificationDefinition battlePassNotificationDefinition = list[num];
			long num2 = currentSeasonStartDate + (long)(battlePassNotificationDefinition?.IntervalFromSeasonStart * UtilsDateTime.DayInMilliseconds).Value;
			if (num2 - GameManager.Instance.playerModel.UtcTimeStamp > 0)
			{
				SaveNotification(LocalizationManager.GetText(battlePassNotificationDefinition?.LocalisationKey), (num2 - GameManager.Instance.playerModel.UtcTimeStamp) / 1000, "BattlePass_" + num);
			}
		}
	}
}
