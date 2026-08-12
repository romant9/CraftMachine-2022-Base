using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class MissionStatisticsBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public MissionStatistics Model { get; set; }

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

		public List<int> TurnsForMedKits { get; set; }

		public List<int> TurnsForGrenade { get; set; }

		public List<int> TurnsForFlare { get; set; }

		public List<int> TurnsForBlastGrenade { get; set; }

		public List<int> TurnsForGore { get; set; }

		public int MedKitsUsed { get; set; }

		public int GrenadesUsed { get; set; }

		public int FlaresUsed { get; set; }

		public int BlastGrenadesUsed { get; set; }

		public int GoreUsed { get; set; }

		public int BattlePassCurrencyEarned { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(MissionStatistics model)
		{
			Model = model;
			RegularCardsCollected = model.RegularCardsCollected;
			SilverCardsCollected = model.SilverCardsCollected;
			GoldenCardsCollected = model.GoldenCardsCollected;
			EasyWalkersKilled = model.EasyWalkersKilled;
			MediumWalkersKilled = model.MediumWalkersKilled;
			HardWalkersKilled = model.HardWalkersKilled;
			WalkersKilledByType = ((model.WalkersKilledByType == null) ? null : new List<int>(model.WalkersKilledByType));
			MultiKillHistogram = ((model.MultiKillHistogram == null) ? null : new int[10]);
			if (model.MultiKillHistogram != null)
			{
				for (int i = 0; i < 10; i++)
				{
					MultiKillHistogram[i] = model.MultiKillHistogram[i];
				}
			}
			WalkersSpawned = model.WalkersSpawned;
			CollectedLoot = model.CollectedLoot;
			CollectedSp = model.CollectedSp;
			BonusSp = model.BonusSp;
			ActualSurvivalPointsAdded = model.ActualSurvivalPointsAdded;
			CollectedSupplies = model.CollectedSupplies;
			ActualSuppliesAdded = model.ActualSuppliesAdded;
			RaidersKilled = model.RaidersKilled;
			StruggleCount = model.StruggleCount;
			LastCombatResult = model.LastCombatResult;
			LastCurrencyRewardType = model.LastCurrencyRewardType;
			LastCurrencyRewardAmount = model.LastCurrencyRewardAmount;
			LastEquipmentReward = model.LastEquipmentReward;
			Stars = model.Stars;
			SurvivalMissionCompletions = model.SurvivalMissionCompletions;
			SurvivalFullCompletions = model.SurvivalFullCompletions;
			MissionsCompleted = model.MissionsCompleted;
			DeadlyMissionsCompleted = model.DeadlyMissionsCompleted;
			MissionsFailed = model.MissionsFailed;
			DeadlyMissionsFailed = model.DeadlyMissionsFailed;
			MissionsFled = model.MissionsFled;
			DeadlyMissionsFled = model.DeadlyMissionsFled;
			WeeklyChallengeMissionsCompleted = model.WeeklyChallengeMissionsCompleted;
			WeeklyChallengeMissionsFailed = model.WeeklyChallengeMissionsFailed;
			WeeklyChallengeMissionsFled = model.WeeklyChallengeMissionsFled;
			SurvivorsDied = model.SurvivorsDied;
			Turns = model.Turns;
			TurnsForMedKits = ((model.TurnsForMedKits == null) ? null : new List<int>(model.TurnsForMedKits));
			TurnsForGrenade = ((model.TurnsForGrenade == null) ? null : new List<int>(model.TurnsForGrenade));
			TurnsForFlare = ((model.TurnsForFlare == null) ? null : new List<int>(model.TurnsForFlare));
			TurnsForBlastGrenade = ((model.TurnsForBlastGrenade == null) ? null : new List<int>(model.TurnsForBlastGrenade));
			TurnsForGore = ((model.TurnsForGore == null) ? null : new List<int>(model.TurnsForGore));
			MedKitsUsed = model.MedKitsUsed;
			GrenadesUsed = model.GrenadesUsed;
			FlaresUsed = model.FlaresUsed;
			BlastGrenadesUsed = model.BlastGrenadesUsed;
			GoreUsed = model.GoreUsed;
			BattlePassCurrencyEarned = model.BattlePassCurrencyEarned;
		}

		public void BackUp()
		{
			Model.RegularCardsCollected = RegularCardsCollected;
			Model.SilverCardsCollected = SilverCardsCollected;
			Model.GoldenCardsCollected = GoldenCardsCollected;
			Model.EasyWalkersKilled = EasyWalkersKilled;
			Model.MediumWalkersKilled = MediumWalkersKilled;
			Model.HardWalkersKilled = HardWalkersKilled;
			Model.WalkersKilledByType = ((WalkersKilledByType == null) ? null : new List<int>(WalkersKilledByType));
			Model.MultiKillHistogram = ((MultiKillHistogram == null) ? null : new int[10]);
			if (MultiKillHistogram != null)
			{
				for (int i = 0; i < 10; i++)
				{
					Model.MultiKillHistogram[i] = MultiKillHistogram[i];
				}
			}
			Model.WalkersSpawned = WalkersSpawned;
			Model.CollectedLoot = CollectedLoot;
			Model.CollectedSp = CollectedSp;
			Model.BonusSp = BonusSp;
			Model.ActualSurvivalPointsAdded = ActualSurvivalPointsAdded;
			Model.CollectedSupplies = CollectedSupplies;
			Model.ActualSuppliesAdded = ActualSuppliesAdded;
			Model.RaidersKilled = RaidersKilled;
			Model.StruggleCount = StruggleCount;
			Model.LastCombatResult = LastCombatResult;
			Model.LastCurrencyRewardType = LastCurrencyRewardType;
			Model.LastCurrencyRewardAmount = LastCurrencyRewardAmount;
			Model.LastEquipmentReward = LastEquipmentReward;
			Model.Stars = Stars;
			Model.SurvivalMissionCompletions = SurvivalMissionCompletions;
			Model.SurvivalFullCompletions = SurvivalFullCompletions;
			Model.MissionsCompleted = MissionsCompleted;
			Model.DeadlyMissionsCompleted = DeadlyMissionsCompleted;
			Model.MissionsFailed = MissionsFailed;
			Model.DeadlyMissionsFailed = DeadlyMissionsFailed;
			Model.MissionsFled = MissionsFled;
			Model.DeadlyMissionsFled = DeadlyMissionsFled;
			Model.WeeklyChallengeMissionsCompleted = WeeklyChallengeMissionsCompleted;
			Model.WeeklyChallengeMissionsFailed = WeeklyChallengeMissionsFailed;
			Model.WeeklyChallengeMissionsFled = WeeklyChallengeMissionsFled;
			Model.SurvivorsDied = SurvivorsDied;
			Model.Turns = Turns;
			Model.TurnsForMedKits = ((TurnsForMedKits == null) ? null : new List<int>(TurnsForMedKits));
			Model.TurnsForGrenade = ((TurnsForGrenade == null) ? null : new List<int>(TurnsForGrenade));
			Model.TurnsForFlare = ((TurnsForFlare == null) ? null : new List<int>(TurnsForFlare));
			Model.TurnsForBlastGrenade = ((TurnsForBlastGrenade == null) ? null : new List<int>(TurnsForBlastGrenade));
			Model.TurnsForGore = ((TurnsForGore == null) ? null : new List<int>(TurnsForGore));
			Model.MedKitsUsed = MedKitsUsed;
			Model.GrenadesUsed = GrenadesUsed;
			Model.FlaresUsed = FlaresUsed;
			Model.BlastGrenadesUsed = BlastGrenadesUsed;
			Model.GoreUsed = GoreUsed;
			Model.BattlePassCurrencyEarned = BattlePassCurrencyEarned;
		}
	}
}
