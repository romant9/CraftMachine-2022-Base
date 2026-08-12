using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class MissionStatistics : TWDModelObject
	{
		public const int MULTIKILL_HISTOGRAM_SIZE = 10;

		public List<int> TurnsForMedKits;

		public List<int> TurnsForGrenade;

		public List<int> TurnsForFlare;

		public List<int> TurnsForBlastGrenade;

		public List<int> TurnsForGore;

		public int MedKitsUsed;

		public int GrenadesUsed;

		public int FlaresUsed;

		public int BlastGrenadesUsed;

		public int GoreUsed;

		public int RegularCardsCollected { get; set; }

		public int SilverCardsCollected { get; set; }

		public int GoldenCardsCollected { get; set; }

		public int EasyWalkersKilled { get; set; }

		public int MediumWalkersKilled { get; set; }

		public int HardWalkersKilled { get; set; }

		public List<int> WalkersKilledByType { get; set; }

		public int[] MultiKillHistogram { get; set; }

		public int WalkersSpawned { get; set; }

		public int CollectedLoot { get; set; }

		public int CollectedSp { get; set; }

		public int BonusSp { get; set; }

		public int ActualSurvivalPointsAdded { get; set; }

		public int CollectedSupplies { get; set; }

		public int ActualSuppliesAdded { get; set; }

		[JsonIgnore]
		public int WalkersKilled => EasyWalkersKilled + MediumWalkersKilled + HardWalkersKilled;

		public int RaidersKilled { get; set; }

		public int StruggleCount { get; set; }

		public ECombatResult LastCombatResult { get; set; }

		public CurrencyType LastCurrencyRewardType { get; set; }

		public int LastCurrencyRewardAmount { get; set; }

		public EquipmentItemModel LastEquipmentReward { get; set; }

		public int Stars { get; set; }

		public int SurvivalMissionCompletions { get; set; }

		public int SurvivalFullCompletions { get; set; }

		public int MissionsCompleted { get; set; }

		public int DeadlyMissionsCompleted { get; set; }

		public int MissionsFailed { get; set; }

		public int DeadlyMissionsFailed { get; set; }

		public int MissionsFled { get; set; }

		public int DeadlyMissionsFled { get; set; }

		public int WeeklyChallengeMissionsCompleted { get; set; }

		public int WeeklyChallengeMissionsFailed { get; set; }

		public int WeeklyChallengeMissionsFled { get; set; }

		public int SurvivorsDied { get; set; }

		public int Turns { get; set; }

		public int BattlePassCurrencyEarned { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void AddCardCollected(DropType dropType)
		{
			if (dropType == DropType.Regular)
			{
				RegularCardsCollected++;
			}
			else if (dropType == DropType.Regular)
			{
				SilverCardsCollected++;
			}
			else if (dropType == DropType.Regular)
			{
				GoldenCardsCollected++;
			}
			NotifyChange("CardsCollected");
		}

		public int GetMultiKillCount(int multiKillCount)
		{
			if (MultiKillHistogram != null && MultiKillHistogram.Length != 0)
			{
				int num = UtilsMath.Clamp(multiKillCount, 0, MultiKillHistogram.Length - 1);
				return MultiKillHistogram[num];
			}
			return 0;
		}

		public void AddMultiWalkerKill(int multiKillCount)
		{
			if (MultiKillHistogram == null)
			{
				MultiKillHistogram = new int[10];
			}
			int num = UtilsMath.Clamp(multiKillCount, 0, 9);
			MultiKillHistogram[num]++;
		}

		public void AddWalkersKilled(int walkerLevel, string actorDefinitionID)
		{
			WalkerType walkerType = WalkerType.WalkerNormal;
			try
			{
				walkerType = (WalkerType)Enum.Parse(typeof(WalkerType), actorDefinitionID);
			}
			catch (Exception)
			{
			}
			MissionGenerationData missionGenerationData = base.manager.GameEconomyData.GetMissionGenerationData(base.manager.Player.SelectedMissionDifficulty);
			if (missionGenerationData != null)
			{
				if (walkerLevel < missionGenerationData.MinWalkerLevel)
				{
					EasyWalkersKilled++;
				}
				else if (walkerLevel < missionGenerationData.MaxWalkerLevel)
				{
					MediumWalkersKilled++;
				}
				else
				{
					HardWalkersKilled++;
				}
			}
			if (WalkersKilledByType == null)
			{
				WalkersKilledByType = new List<int>();
			}
			while (WalkersKilledByType.Count <= (int)walkerType)
			{
				WalkersKilledByType.Add(0);
			}
			WalkersKilledByType[(int)walkerType]++;
			if (base.manager?.Player != null)
			{
				base.manager.Player.NotifyWalkersKilled(1);
			}
			NotifyChange("WalkersKilled");
		}

		public int GetWalkersKilledByType(WalkerType walkerType)
		{
			if (WalkersKilledByType == null || (int)walkerType >= WalkersKilledByType.Count)
			{
				return 0;
			}
			return WalkersKilledByType[(int)walkerType];
		}

		public void AddWalkersSpawned()
		{
			WalkersSpawned++;
			NotifyChange("WalkersSpawned");
		}

		public void AddCollectedLoot()
		{
			CollectedLoot++;
			NotifyChange("LootCollected");
		}

		public void AddCollectedSurvivalPoints(int amount)
		{
			CollectedSp += amount;
			NotifyChange("CollectedSp");
		}

		public void AddBonusSP(int amount)
		{
			BonusSp += amount;
			NotifyChange("CollectedSp");
		}

		public void AddActualSurvivalPointsAdded(int amount)
		{
			ActualSurvivalPointsAdded += amount;
		}

		public void AddCollectedSupplies(int amount)
		{
			CollectedSupplies += amount;
			NotifyChange("CollectedSupplies");
		}

		public void AddActualSuppliesAdded(int amount)
		{
			ActualSuppliesAdded += amount;
		}

		public int GetSuppliesOverflow()
		{
			return CollectedSupplies - ActualSuppliesAdded;
		}

		public int GetSPOverflow()
		{
			return CollectedSp - ActualSurvivalPointsAdded;
		}

		public void AddRaidersKilled()
		{
			RaidersKilled++;
			NotifyChange("RaidersKilled");
		}

		public void AddStruggleCount()
		{
			StruggleCount++;
			NotifyChange("StruggleCount");
		}

		public void SetCombatResult(ECombatResult result, bool isDeadly, bool isWeeklyChallenge, bool notify = true)
		{
			LastCombatResult = result;
			SetMissionCompleted(isDeadly, result, isWeeklyChallenge, notify);
		}

		public void AddReward(CurrencyType currencyType, int amount)
		{
			LastCurrencyRewardType = currencyType;
			LastCurrencyRewardAmount = amount;
			LastEquipmentReward = null;
		}

		public void AddReward(EquipmentItemModel equipmentModel)
		{
			LastCurrencyRewardType = CurrencyType.None;
			LastCurrencyRewardAmount = 0;
			LastEquipmentReward = equipmentModel;
		}

		public void AddStars(int amount)
		{
			Stars += amount;
			NotifyChange("Stars");
		}

		public void AddSurvivalMissionCompletions(int amount)
		{
			SurvivalMissionCompletions += amount;
			NotifyChange("SurvivalMissionCompletions");
		}

		public void AddSurvivalFullCompletions(int amount)
		{
			SurvivalFullCompletions += amount;
			NotifyChange("SurvivalFullCompletions");
		}

		public void SetMissionCompleted(bool deadly, ECombatResult result, bool isWeeklyChallenge, bool notify)
		{
			DeadlyMissionsCompleted = 0;
			MissionsCompleted = 0;
			DeadlyMissionsFailed = 0;
			MissionsFailed = 0;
			DeadlyMissionsFled = 0;
			MissionsFled = 0;
			WeeklyChallengeMissionsCompleted = 0;
			WeeklyChallengeMissionsFailed = 0;
			WeeklyChallengeMissionsFled = 0;
			switch (result)
			{
			case ECombatResult.Successful:
				if (deadly)
				{
					DeadlyMissionsCompleted = 1;
				}
				else
				{
					MissionsCompleted = 1;
				}
				break;
			case ECombatResult.Failed:
				if (deadly)
				{
					DeadlyMissionsFailed = 1;
				}
				else
				{
					MissionsFailed = 1;
				}
				break;
			case ECombatResult.Flee:
				if (deadly)
				{
					DeadlyMissionsFled = 1;
				}
				else
				{
					MissionsFled = 1;
				}
				break;
			}
			if (isWeeklyChallenge)
			{
				switch (result)
				{
				case ECombatResult.Successful:
					WeeklyChallengeMissionsCompleted = 1;
					break;
				case ECombatResult.Failed:
					WeeklyChallengeMissionsFailed = 1;
					break;
				case ECombatResult.Flee:
					WeeklyChallengeMissionsFled = 1;
					break;
				}
			}
			if (notify)
			{
				NotifyChange("MissionsCompleted");
			}
		}

		public void AddSurvivorDied()
		{
			SurvivorsDied++;
		}

		public void AddTurn()
		{
			Turns++;
		}

		public void AddConsumableUsed(EquipmentItemModel consumable, int turn)
		{
			if (consumable.Definition.Category == EquipmentCategory.Utility)
			{
				switch (ConsumableUtils.IdToConsumableType(consumable.Definition.ID))
				{
				case EquipmentModel.ConsumableType.Grenade:
					IncrementConsumable(ref GrenadesUsed, ref TurnsForGrenade, turn);
					break;
				case EquipmentModel.ConsumableType.MedKit:
					IncrementConsumable(ref MedKitsUsed, ref TurnsForMedKits, turn);
					break;
				case EquipmentModel.ConsumableType.Flare:
					IncrementConsumable(ref FlaresUsed, ref TurnsForFlare, turn);
					break;
				case EquipmentModel.ConsumableType.BlastGrenade:
					IncrementConsumable(ref BlastGrenadesUsed, ref TurnsForBlastGrenade, turn);
					break;
				case EquipmentModel.ConsumableType.Gore:
					IncrementConsumable(ref GoreUsed, ref TurnsForGore, turn);
					break;
				default:
					throw new NotImplementedException();
				}
			}
		}

		private void IncrementConsumable(ref int counter, ref List<int> turns, int newTurn)
		{
			counter++;
			if (turns == null)
			{
				turns = new List<int>();
			}
			turns.Add(newTurn);
		}

		public int GetLastTurnForConsumable(string consumableId)
		{
			switch (ConsumableUtils.IdToConsumableType(consumableId))
			{
			case EquipmentModel.ConsumableType.Grenade:
			{
				List<int> turnsForGrenade = TurnsForGrenade;
				if (turnsForGrenade == null || !turnsForGrenade.Any())
				{
					return -1;
				}
				return TurnsForGrenade.Max();
			}
			case EquipmentModel.ConsumableType.MedKit:
			{
				List<int> turnsForMedKits = TurnsForMedKits;
				if (turnsForMedKits == null || !turnsForMedKits.Any())
				{
					return -1;
				}
				return TurnsForMedKits.Max();
			}
			case EquipmentModel.ConsumableType.Flare:
			{
				List<int> turnsForFlare = TurnsForFlare;
				if (turnsForFlare == null || !turnsForFlare.Any())
				{
					return -1;
				}
				return TurnsForFlare.Max();
			}
			case EquipmentModel.ConsumableType.BlastGrenade:
			{
				List<int> turnsForBlastGrenade = TurnsForBlastGrenade;
				if (turnsForBlastGrenade == null || !turnsForBlastGrenade.Any())
				{
					return -1;
				}
				return TurnsForBlastGrenade.Max();
			}
			case EquipmentModel.ConsumableType.Gore:
			{
				List<int> turnsForGore = TurnsForGore;
				if (turnsForGore == null || !turnsForGore.Any())
				{
					return -1;
				}
				return TurnsForGore.Max();
			}
			default:
				throw new NotImplementedException();
			}
		}

		public bool HaveConsumablesBeenUsed()
		{
			if (MedKitsUsed <= 0 && GrenadesUsed <= 0 && FlaresUsed <= 0 && BlastGrenadesUsed <= 0)
			{
				return GoreUsed > 0;
			}
			return true;
		}

		public void AddBattlePassCurrency(int amount)
		{
			BattlePassCurrencyEarned += amount;
		}

		public static MissionStatistics operator +(MissionStatistics a, MissionStatistics b)
		{
			MissionStatistics missionStatistics = new MissionStatistics();
			missionStatistics.EasyWalkersKilled = a.EasyWalkersKilled + b.EasyWalkersKilled;
			missionStatistics.MediumWalkersKilled = a.MediumWalkersKilled + b.MediumWalkersKilled;
			missionStatistics.HardWalkersKilled = a.HardWalkersKilled + b.HardWalkersKilled;
			missionStatistics.RaidersKilled = a.RaidersKilled + b.RaidersKilled;
			missionStatistics.WalkersSpawned = a.WalkersSpawned + b.WalkersSpawned;
			missionStatistics.CollectedLoot = a.CollectedLoot + b.CollectedLoot;
			missionStatistics.CollectedSp = a.CollectedSp + b.CollectedSp;
			missionStatistics.BonusSp = a.BonusSp + b.BonusSp;
			missionStatistics.CollectedSupplies = a.CollectedSupplies + b.CollectedSupplies;
			missionStatistics.RegularCardsCollected = a.RegularCardsCollected + b.RegularCardsCollected;
			missionStatistics.SilverCardsCollected = a.SilverCardsCollected + b.SilverCardsCollected;
			missionStatistics.GoldenCardsCollected = a.GoldenCardsCollected + b.GoldenCardsCollected;
			missionStatistics.StruggleCount = a.StruggleCount + b.StruggleCount;
			missionStatistics.Stars = a.Stars + b.Stars;
			missionStatistics.Turns = a.Turns + b.Turns;
			missionStatistics.SurvivorsDied = a.SurvivorsDied + b.SurvivorsDied;
			missionStatistics.MissionsCompleted = a.MissionsCompleted + b.MissionsCompleted;
			missionStatistics.DeadlyMissionsCompleted = a.DeadlyMissionsCompleted + b.DeadlyMissionsCompleted;
			missionStatistics.MissionsFailed = a.MissionsFailed + b.MissionsFailed;
			missionStatistics.DeadlyMissionsFailed = a.DeadlyMissionsFailed + b.DeadlyMissionsFailed;
			missionStatistics.MissionsFled = a.MissionsFled + b.MissionsFled;
			missionStatistics.DeadlyMissionsFled = a.DeadlyMissionsFled + b.DeadlyMissionsFled;
			missionStatistics.WeeklyChallengeMissionsCompleted = a.WeeklyChallengeMissionsCompleted + b.WeeklyChallengeMissionsCompleted;
			missionStatistics.WeeklyChallengeMissionsFailed = a.WeeklyChallengeMissionsFailed + b.WeeklyChallengeMissionsFailed;
			missionStatistics.WeeklyChallengeMissionsFled = a.WeeklyChallengeMissionsFled + b.WeeklyChallengeMissionsFled;
			missionStatistics.SurvivalFullCompletions = a.SurvivalFullCompletions + b.SurvivalFullCompletions;
			missionStatistics.SurvivalMissionCompletions = a.SurvivalMissionCompletions + b.SurvivalMissionCompletions;
			missionStatistics.WalkersKilledByType = new List<int>();
			int num = ((a.WalkersKilledByType != null) ? a.WalkersKilledByType.Count : 0);
			int num2 = ((b.WalkersKilledByType != null) ? b.WalkersKilledByType.Count : 0);
			for (int i = 0; i < Math.Max(num, num2); i++)
			{
				int num3 = ((i < num) ? a.WalkersKilledByType[i] : 0);
				int num4 = ((i < num2) ? b.WalkersKilledByType[i] : 0);
				missionStatistics.WalkersKilledByType.Add(num3 + num4);
			}
			int num5 = UtilsMath.Max((a.MultiKillHistogram != null) ? a.MultiKillHistogram.Length : 0, (b.MultiKillHistogram != null) ? b.MultiKillHistogram.Length : 0);
			if (num5 > 0)
			{
				missionStatistics.MultiKillHistogram = new int[num5];
				for (int j = 0; j < num5; j++)
				{
					int num6 = ((a.MultiKillHistogram != null && j < a.MultiKillHistogram.Length) ? a.MultiKillHistogram[j] : 0);
					int num7 = ((b.MultiKillHistogram != null && j < b.MultiKillHistogram.Length) ? b.MultiKillHistogram[j] : 0);
					missionStatistics.MultiKillHistogram[j] = num6 + num7;
				}
			}
			missionStatistics.LastCombatResult = b.LastCombatResult;
			missionStatistics.GrenadesUsed = a.GrenadesUsed + b.GrenadesUsed;
			missionStatistics.MedKitsUsed = a.MedKitsUsed + b.MedKitsUsed;
			missionStatistics.FlaresUsed = a.FlaresUsed + b.FlaresUsed;
			missionStatistics.BlastGrenadesUsed = a.BlastGrenadesUsed + b.BlastGrenadesUsed;
			missionStatistics.GoreUsed = a.GoreUsed + b.GoreUsed;
			missionStatistics.TurnsForMedKits = new List<int>();
			if (a.TurnsForMedKits != null)
			{
				missionStatistics.TurnsForMedKits.AddRange(a.TurnsForMedKits);
			}
			if (b.TurnsForMedKits != null)
			{
				missionStatistics.TurnsForMedKits.AddRange(b.TurnsForMedKits);
			}
			missionStatistics.TurnsForGrenade = new List<int>();
			if (a.TurnsForGrenade != null)
			{
				missionStatistics.TurnsForGrenade.AddRange(a.TurnsForGrenade);
			}
			if (b.TurnsForGrenade != null)
			{
				missionStatistics.TurnsForGrenade.AddRange(b.TurnsForGrenade);
			}
			missionStatistics.TurnsForFlare = new List<int>();
			if (a.TurnsForFlare != null)
			{
				missionStatistics.TurnsForFlare.AddRange(a.TurnsForFlare);
			}
			if (b.TurnsForFlare != null)
			{
				missionStatistics.TurnsForFlare.AddRange(b.TurnsForFlare);
			}
			missionStatistics.TurnsForBlastGrenade = new List<int>();
			if (a.TurnsForBlastGrenade != null)
			{
				missionStatistics.TurnsForBlastGrenade.AddRange(a.TurnsForBlastGrenade);
			}
			if (b.TurnsForBlastGrenade != null)
			{
				missionStatistics.TurnsForBlastGrenade.AddRange(b.TurnsForBlastGrenade);
			}
			missionStatistics.TurnsForGore = new List<int>();
			if (a.TurnsForGore != null)
			{
				missionStatistics.TurnsForGore.AddRange(a.TurnsForGore);
			}
			if (b.TurnsForGore != null)
			{
				missionStatistics.TurnsForGore.AddRange(b.TurnsForGore);
			}
			return missionStatistics;
		}
	}
}
