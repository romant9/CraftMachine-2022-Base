using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class BlackboardModel : TWDModelObject
	{
		public const string OutpostGiftSurvivorsGiven = "Toggle.OutpostGiftSurvivorsGiven";

		public const string ToggleGoreDisabled = "Toggle.GoreDisabled";

		public const string ToggleGoreUsed = "Toggle.GoreUsed";

		public const string ToggleGameCenterConnected = "Toggle.GameCenterConnected";

		public const string ToggleAutoCoverDisabled = "Toggle.AutoCoverDisabled";

		public const string ToggleChallengeUnlockedSeen = "Toggle.ChallengeUnlockedSeen";

		public const string ToggleChallengeTutorialSeen = "Toggle.ChallengeTutorialSeen";

		public const string ToggleOutpostUnlockedSeen = "Toggle.ToggleOutpostUnlockedSeen";

		public const string ToggleOutpostEditUnlockedSeen = "Toggle.ToggleOutpostEditUnlockedSeen";

		public const string ToggleUpdateInfoPopupShown = "Toggle.ToggleUpdateInfoPopupShown";

		public const string ToggleUpdateGiftReceived = "Toggle.ToggleUpdateGiftReceived";

		public const string ToggleSurvivalUnlockedSeen = "Toggle.SurvivalUnlockedSeen";

		public const string ToggleResidenceSeen = "Toggle.ResidenceSeen";

		public const string ToggleSurvivorBadgesSeen = "Toggle.SurvivorBadgesSeen";

		public const string ToggleGuildBattleUnlockedSeen = "Toggle.GuildBattleUnlockedSeen";

		public const string ToggleAdsCompensationReceived = "Toggle.ToggleAdsCompensationReceived";

		public const string ToggleEndlessModeUnlockedSeen = "Toggle.EndlessModeUnlockedSeen";

		public const string ToggleChallengeHighlightExpired = "Toggle.ToggleWeeklyChallengeHighlight";

		public const string ToggleSurvivalHighlightExpired = "Toggle.ToggleWeeklySurvivalHighlight";

		public const string ToggleSeasonHighlightExpired = "Toggle.ToggleSeasonHighlight";

		public const string ToggleScavengeHighlightExpired = "Toggle.ToggleScavengeHighlight";

		public const string ToggleEndlessModeHighlightExpired = "Toggle.ToggleEndlessModeHighlightExpired";

		public const string Toggle60FPSModeEnabled = "Toggle.Toggle60FPSModeEnabled";

		public const string ToggleCombatGridEnabled = "Toggle.ToggleCombatGridEnabled";

		public const string ToggleCombatCameraEnabled = "Toggle.ToggleCombatCameraEnabled";

		public const string CounterCompletedOutpostAttacks = "Counter.CompletedOutpostAttacks";

		public const string CounterSessionPlayed = "Counter.SessionPlayed";

		public const string CounterSessionDaysPlayed = "Counter.SessionDaysPlayed";

		public const string CounterSurvivorsAccepted = "Counter.Survivors.Accepted";

		public const string CounterSuppliesCollected = "Counter.Supplies.Collected";

		public const string CounterSurvivalPointsCollected = "Counter.SurvivalPoints.Collected";

		public const string CounterGasCollected = "Counter.Gas.Collected";

		public const string CounterNumberMissionPlayed = "Counter.NumberMissionPlayed";

		public const string CounterDefendWalkersKilled = "Counter.DefendWalkersKilled";

		public const string CounterNumberMissionCompletedNoDamage = "Counter.NumberMissionCompletedNoDamage";

		public const string CounterNumberMissionCompletedNoStruggle = "Counter.NumberMissionCompletedNoStruggle";

		public const string CounterNumberMissionsCompletedNoWalkerKills = "Counter.NumberMissionsCompletedNoWalkerKills";

		public const string CounterNumberGrindMissionCompleted = "Counter.NumberGrindMissionCompleted";

		public const string CounterNumberChallengeMissionCompleted = "Counter.NumberChallengeMissionCompleted";

		public const string CounterNumberSurvivalMissionCompleted = "Counter.NumberSurvivalMissionCompleted";

		public const string CounterNumberStoryMissionCompleted = "Counter.NumberStoryMissionCompleted";

		public const string CounterSameClassMissionComplete = "Counter.SameClassMissionComplete";

		public const string CounterPhonesUsed = "Counter.PhonesUsed";

		public const string CounterCrappyCallsInARow = "Counter.CrappyCallsInARow";

		public const string CounterPhoneCallSilver = "Counter.PhoneCallSilver";

		public const string CounterPhoneCallGold = "Counter.PhoneCallGold";

		public const string CounterPhonesFirstTimeGoldCall = "Counter.PhonesFirstTimeGoldCall";

		public const string CounterNumberPhoneCallsMade = "Counter.NumberPhoneCallsMade";

		public const string CounterOutpostInfluenceTierPrefix = "Counter.OutpostInfluenceTier.";

		public const string CounterPromptedUnlocksPerActor = "Counter.PromptedUnlocksPerActor";

		public const string CounterSameEquipmentTypeMissionComplete = "Counter.SameEquipmentTypeMissionComplete";

		public const string ToggleEpisodeVideoWatched = "Toggle.EpisodeVideoWatched";

		public const string ToggleShowNewCampLocation = "Toggle.NewCamp.Show";

		public const string ToggleCampMoved = "Toggle.CampMoved";

		public const string SmartTutorialShown = "Toggle.SmartTutorialShown";

		public const string PendingCrossbowToBeGiven = "Toggle.PendingCrossbowToBeGiven";

		public const string EpisodeSeen = "Toggle.Episode.";

		public const string NewChallengesSeen = "NewChallengesSeen";

		public const string NewChallengeMasterMissionSeen = "NewChallengeMasterMissionSeen";

		public const string NewChallengePlightSeen = "NewChallengePlightSeen";

		public const string NewChallengeApocalypticSeen = "NewChallengeApocalypticSeen";

		public const string NewSurvivalSeen = "NewSurvivalSeen";

		public const string HasSeenGuildBattleStart = "HasSeenGuildBattleStart";

		public const string HasSeenGuildBattleEnd = "HasSeenGuildBattleEnd";

		public const string HasSeenGuildWarStart = "HasSeenGuildWarStart";

		public const string HasSeenSeasonStart = "HasSeenSeasonStart";

		public const string HasSeenSeasonEnd = "HasSeenSeasonEnd";

		public const string HasSeenGuildBattleWelcome = "HasSeenGuildBattleWelcome";

		public const string HasSeenGvGBetaNotice = "HasSeenGvGBetaNotice";

		public const string HasSeenGvGCalendarInfo = "HasSeenGvGCalendarInfo";

		public const string HasSeenWhatsNewInGuildWars = "HasSeenWhatsNewInGuildWars";

		public const string BuyJustEnoughGasForMission = "BuyJustEnoughGasForMission";

		public const string ToggleBlackMarketNotifications = "Toggle.BlackMarketNotifications";

		public const string ToggleBlackMarketSlotUpdated = "Toggle.ToggleBlackMarketSlotUpdated";

		public const string HasSeenGvGSeasonReset = "HasSeenGvGSeasonReset";

		public const string ToggleEndlessModeFTUEFirstBloodTutorial = "ToggleEndlessModeFTUEFirstBloodTutorial";

		public const string ToggleEndlessModeFTUESpecialWalkerTutorial = "ToggleEndlessModeFTUESpecialWalkerTutorial";

		public const string ToggleEndlessModeFTUEWavesTutorial = "ToggleEndlessModeFTUEWavesTutorial";

		public const string ToggleEndlessModeFTUEHubReturnTutorial = "TootleEndlessModeFTUEHubReturnTutorial";

		public const string ToggleEndlessModeIntroductionPopup = "ToggleEndlessModeIntroductionPopup";

		public Dictionary<string, bool> UnlockValues { get; private set; }

		public Dictionary<string, bool> ToggleValues { get; private set; }

		public Dictionary<string, int> CounterValues { get; private set; }

		public event BlackboardChangedHandler BlackboardChanged;

		public static string GetOutpostInfluenceTierCounterKey(string tierId)
		{
			return "Counter.OutpostInfluenceTier." + tierId;
		}

		public static string GetSameClassMissionCompleteKey(SurvivorClass survivorClass)
		{
			return "Counter.SameClassMissionComplete." + survivorClass;
		}

		public static string GetPromptedUnlocksPerActorKey(string actorId)
		{
			return "Counter.PromptedUnlocksPerActor." + ((!string.IsNullOrEmpty(actorId)) ? actorId : "");
		}

		public static string GetSameEquipmentTypeMissionCompleteKey(EquipmentCategory equipmentCategory)
		{
			return "Counter.SameEquipmentTypeMissionComplete." + equipmentCategory;
		}

		public static string GetEpisodeVideoWatchedKey(string episodeId)
		{
			return "Toggle.EpisodeVideoWatched." + episodeId;
		}

		public BlackboardModel()
		{
			UnlockValues = new Dictionary<string, bool>();
			ToggleValues = new Dictionary<string, bool>();
			CounterValues = new Dictionary<string, int>();
		}

		private void NotifyChange(BlackboardEntryType type, string key)
		{
			this.BlackboardChanged?.Invoke(type, key);
		}

		public bool IsToggleOn(string toggleKey)
		{
			bool value = false;
			ToggleValues.TryGetValue(toggleKey, out value);
			return value;
		}

		public bool IsToggleAll(string toggleKey)
		{
			return AreAllBooleanValues(toggleKey, ToggleValues);
		}

		public bool IsToggleAny(string toggleKey)
		{
			return IsAnyBooleanValue(toggleKey, ToggleValues);
		}

		public bool IsUnlocked(string unlockKey)
		{
			bool value = false;
			UnlockValues.TryGetValue(unlockKey, out value);
			return value;
		}

		public bool IsUnlockedAll(string unlockKey)
		{
			return AreAllBooleanValues(unlockKey, UnlockValues);
		}

		public bool IsUnlockedAny(string unlockKey)
		{
			return IsAnyBooleanValue(unlockKey, UnlockValues);
		}

		private List<bool> GetBooleanValues(string key, Dictionary<string, bool> dictionary)
		{
			List<bool> list = new List<bool>();
			string[] array = key.Split('.');
			foreach (KeyValuePair<string, bool> item in dictionary)
			{
				bool flag = true;
				string[] array2 = item.Key.Split('.');
				for (int i = 0; i < array2.Length && i < array.Length && !(array[i] == "*"); i++)
				{
					if (!(array[i] == "?") && array[i] != array2[i])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(item.Value);
				}
			}
			return list;
		}

		private bool AreAllBooleanValues(string key, Dictionary<string, bool> dictionary)
		{
			foreach (bool booleanValue in GetBooleanValues(key, dictionary))
			{
				if (!booleanValue)
				{
					return false;
				}
			}
			return true;
		}

		private bool IsAnyBooleanValue(string key, Dictionary<string, bool> dictionary)
		{
			foreach (bool booleanValue in GetBooleanValues(key, dictionary))
			{
				if (booleanValue)
				{
					return true;
				}
			}
			return false;
		}

		public List<int> GetCounterValues(string counterKey)
		{
			List<int> list = new List<int>();
			string[] array = counterKey.Split('.');
			foreach (KeyValuePair<string, int> counterValue in CounterValues)
			{
				bool flag = true;
				string[] array2 = counterValue.Key.Split('.');
				for (int i = 0; i < array2.Length && i < array.Length && !(array[i] == "*"); i++)
				{
					if (!(array[i] == "?") && array[i] != array2[i])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(counterValue.Value);
				}
			}
			return list;
		}

		public int GetMaxCounterValue(string counterKey)
		{
			List<int> counterValues = GetCounterValues(counterKey);
			int num = 0;
			foreach (int item in counterValues)
			{
				num = ((item > num) ? item : num);
			}
			return num;
		}

		public int GetMinCounterValue(string counterKey)
		{
			List<int> counterValues = GetCounterValues(counterKey);
			int num = int.MaxValue;
			foreach (int item in counterValues)
			{
				num = ((item < num) ? item : num);
			}
			return num;
		}

		public int GetSumCounterValue(string counterKey)
		{
			List<int> counterValues = GetCounterValues(counterKey);
			int num = 0;
			foreach (int item in counterValues)
			{
				num += item;
			}
			return num;
		}

		public int GetCounter(string counterKey, int defaultValue = 0)
		{
			int value = defaultValue;
			CounterValues.TryGetValue(counterKey, out value);
			return value;
		}

		public bool HasCounter(string counterKey)
		{
			return CounterValues.ContainsKey(counterKey);
		}

		public void SetToggle(string toggleKey)
		{
			if (toggleKey.Contains("*") || toggleKey.Contains("?"))
			{
				throw new Exception("Cannot set toggle with wildcard key");
			}
			ToggleValues[toggleKey] = true;
			NotifyChange(BlackboardEntryType.Toggle, toggleKey);
		}

		public void ClearToggle(string toggleKey)
		{
			if (toggleKey.Contains("*") || toggleKey.Contains("?"))
			{
				throw new Exception("Cannot clear toggle with wildcard key");
			}
			ToggleValues[toggleKey] = false;
			NotifyChange(BlackboardEntryType.Toggle, toggleKey);
		}

		public void Unlock(string unlockKey)
		{
			if (unlockKey.Contains("*") || unlockKey.Contains("?"))
			{
				throw new Exception("Cannot set unlock with wildcard key");
			}
			UnlockValues[unlockKey] = true;
			NotifyChange(BlackboardEntryType.Unlock, unlockKey);
		}

		public void SetCounter(string counterKey, int value)
		{
			if (counterKey.Contains("*") || counterKey.Contains("?"))
			{
				throw new Exception("Cannot set counter with wildcard key");
			}
			CounterValues[counterKey] = value;
			NotifyChange(BlackboardEntryType.Counter, counterKey);
		}

		public void IncreaseCounter(string counterKey, int amount = 1)
		{
			if (counterKey.Contains("*") || counterKey.Contains("?"))
			{
				throw new Exception("Cannot increase counter with wildcard key");
			}
			SetCounter(counterKey, GetCounter(counterKey) + amount);
			NotifyChange(BlackboardEntryType.Counter, counterKey);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
