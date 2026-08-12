using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class OutpostCombat : TWDModelObject
	{
		public struct CageWalkerInfo
		{
			public WalkerType WalkerType;

			public int WalkerLevel;

			public int AssignedCount;
		}

		public string DefenderHashedId;

		public string DefenderName;

		public int DefenderInitialRankingScore;

		public int DefenderInitialTradeGoods;

		public int DefenderOutpostLevel;

		public int DefenderPlayerLevel;

		public int DefenderCouncilLevel;

		public string DefenderCountry;

		public string DefenderGuildId;

		public bool CombatStarted;

		public bool PVPResultResolved;

		public List<CageWalkerInfo> CageWalkerInfos;

		public bool IsFake;

		public int AttackerInfluenceGain;

		public int AttackerInfluenceLoss;

		public int DefenderInfluenceGain;

		public int DefenderInfluenceLoss;

		public int RealTradeGoodsGain;

		public int FakeTradeGoodsGain;

		public string FakeName;

		[IgnoreModelProperty]
		public ModelList<SurvivorModel> DefendingSurvivors { get; set; }

		[IgnoreModelProperty]
		public ModelList<OutpostWalkerModel> DefendingWalkers { get; set; }

		[JsonIgnore]
		public string DefenderDisplayName
		{
			get
			{
				if (!IsFake)
				{
					return DefenderName;
				}
				return FakeName;
			}
		}

		[JsonIgnore]
		public int TradeGoodsGain
		{
			get
			{
				if (!IsFake)
				{
					return RealTradeGoodsGain;
				}
				return FakeTradeGoodsGain;
			}
		}

		public string IdForAnalytics { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		private static int GetBuildingLevel(PlayerModel defender, string buildingTypeName)
		{
			string buildingLevelBlackboardKey = BuildingModel.GetBuildingLevelBlackboardKey(buildingTypeName, 0);
			return defender.Blackboard.GetCounter(buildingLevelBlackboardKey);
		}

		public void Initialize(PlayerModel defender)
		{
			defender.SetManager(base.manager);
			DefenderHashedId = defender.HashedId;
			DefenderName = defender.Name;
			DefenderInitialRankingScore = defender.RankingScore;
			DefenderInitialTradeGoods = defender.GetCurrency(CurrencyType.Outpost).Value;
			DefenderOutpostLevel = GetBuildingLevel(defender, "Outpost");
			DefenderCouncilLevel = GetBuildingLevel(defender, "Council");
			DefenderCountry = defender.Country;
			DefenderGuildId = defender.GuildId;
			DefendingSurvivors = new ModelList<SurvivorModel>();
			for (int i = 0; i < defender.SurvivorContainer.OutpostDefendingSurvivors.Count; i++)
			{
				SurvivorModel survivorModel = defender.SurvivorContainer.OutpostDefendingSurvivors[i];
				survivorModel.Faction = Faction.Raider;
				DefendingSurvivors.Add(survivorModel);
			}
			ApplyAttributeSystems(defender);
			DefendingWalkers = new ModelList<OutpostWalkerModel>();
			for (int j = 0; j < defender.OutpostModel.WalkerModels.Count; j++)
			{
				DefendingWalkers.Add(defender.OutpostModel.WalkerModels[j]);
			}
			defender.OutpostModel.UpdateCageEnabledWalkers();
			CageWalkerInfos = new List<CageWalkerInfo>();
			for (int k = 0; k < defender.OutpostModel.CageEnabledWalkerModels.Count; k++)
			{
				OutpostWalkerModel outpostWalkerModel = defender.OutpostModel.CageEnabledWalkerModels[k];
				CageWalkerInfo item = default(CageWalkerInfo);
				item.WalkerType = (WalkerType)Enum.Parse(typeof(WalkerType), outpostWalkerModel.ActorDefinition.ID);
				item.WalkerLevel = outpostWalkerModel.Level;
				item.AssignedCount = defender.OutpostModel.StoredLevelModel.GetTotalWalkersAssigned(item.WalkerType);
				CageWalkerInfos.Add(item);
			}
			CalculateRewards(base.manager, defender);
			if (base.manager.Player != null)
			{
				IdForAnalytics = ModelHelpers.MD5Sum(base.manager.Player.HashedId + defender.HashedId + base.manager.Player.UtcTimeStamp);
			}
			if (base.manager != null && base.manager.Player != null)
			{
				base.manager.Metrics.AddSpend().AddResources(base.manager.Player.OutpostModel.GetRaidCashier()).AddStart()
					.AddMission()
					.AddPvp()
					.AddPvpAttacker()
					.AddPvpDefender(defender)
					.Send();
			}
		}

		private void ApplyAttributeSystems(PlayerModel defender)
		{
			SnapshotCombatAttributeData(defender);
		}

		private void SnapshotCombatAttributeData(PlayerModel defender)
		{
			for (int i = 0; i < defender.SurvivorContainer.OutpostDefendingSurvivors.Count; i++)
			{
				ActorModel actorModel = defender.SurvivorContainer.OutpostDefendingSurvivors[i];
				Dictionary<AttributeType, FixedPoint> dictionary = new Dictionary<AttributeType, FixedPoint>();
				for (int j = 100; j < 109; j++)
				{
					AttributeType key = (AttributeType)j;
					dictionary[key] = 0.0;
				}
				if (defender.SurvivalManualManager != null)
				{
					dictionary[AttributeType.Attack] += defender.SurvivalManualManager.GetPrivateAttack(actorModel);
					dictionary[AttributeType.AttackRatio] += defender.SurvivalManualManager.GetPrivateAttackRatio(actorModel);
					dictionary[AttributeType.Critical] += defender.SurvivalManualManager.GetPrivateCritical(actorModel);
					dictionary[AttributeType.DmgCriticalRatio] += defender.SurvivalManualManager.GetPrivateDmgCriticalRatio(actorModel);
					dictionary[AttributeType.DmgTotalRefRatio] += defender.SurvivalManualManager.GetPrivateDmgTotalRefRatio(actorModel);
					dictionary[AttributeType.Attack] += defender.SurvivalManualManager.GetSystemAttack();
					dictionary[AttributeType.AttackRatio] += defender.SurvivalManualManager.GetAttributeAttackRatio();
					dictionary[AttributeType.HitrateMelee] += defender.SurvivalManualManager.GetAttributeHitrateMelee();
					dictionary[AttributeType.HitrateRange] += defender.SurvivalManualManager.GetAttributeHitrateRange();
					dictionary[AttributeType.CriticalRef] += defender.SurvivalManualManager.GetAttributeCriticalRef();
					dictionary[AttributeType.DmgCriticalRatioRef] += defender.SurvivalManualManager.GetAttributeDmgCriticalRatioRef();
				}
				actorModel.CombatAttributeSnapshots = dictionary;
			}
		}

		public void SetFake()
		{
			IsFake = true;
			FakeName = FakeNameGenerator.GetFakeName(base.manager, DefenderHashedId);
		}

		public static int GetTradeGoodsReward(GameEconomyData gameEconomyData, int defenderLevel, int defenderOutpostPower, string attackedHashedId, string defenderHashedId, bool isFake = false)
		{
			OutpostRewardInfo outpostReward = gameEconomyData.GetOutpostReward(defenderLevel, OutpostRewardLevelType.DefenderLevel);
			OutpostRewardInfo outpostReward2 = gameEconomyData.GetOutpostReward(defenderOutpostPower, OutpostRewardLevelType.PowerLevel);
			if (outpostReward == null || outpostReward2 == null)
			{
				return 0;
			}
			ModelRandom modelRandom = new ModelRandom((int)ModelHelpers.MD5SumLong(attackedHashedId + defenderHashedId));
			int num = ((outpostReward != null) ? modelRandom.GetRandomInRange(outpostReward.MinReward, outpostReward.MaxReward) : 0);
			int num2 = ((outpostReward2 != null) ? modelRandom.GetRandomInRange(outpostReward2.MinReward, outpostReward2.MaxReward) : 0);
			int num3 = num + num2;
			if (isFake)
			{
				num3 = num3 * gameEconomyData.ConfigData.OutpostFakeOpponentResourceRewardPercentage / 100;
			}
			return num3;
		}

		public OutpostWalkerModel GetWalkerModel(string walkerId)
		{
			for (int i = 0; i < DefendingWalkers.Count; i++)
			{
				if (DefendingWalkers[i].Id == walkerId)
				{
					return DefendingWalkers[i];
				}
			}
			return null;
		}

		private void CalculateRewards(TWDModelManager manager, PlayerModel defender)
		{
			GameEconomyData gameEconomyData = manager.GameEconomyData;
			PlayerModel player = manager.Player;
			int rankingScoreChange = player.GetRankingScoreChange(player.RankingScore, defender.RankingScore);
			int num = player.GetRankingScoreChange(defender.RankingScore, player.RankingScore) * gameEconomyData.ConfigData.OutpostDefendersWonInfluencePercentage / 100;
			AttackerInfluenceGain = UtilsMath.Max(rankingScoreChange * player.TierAttackWinMultiplier / 100, gameEconomyData.ConfigData.OutpostMinimumInfluenceReward);
			AttackerInfluenceLoss = num * player.TierAttackLossMultiplier / 100;
			DefenderInfluenceGain = num * player.GetTierDefenderWinMultiplier(defender) / 100;
			DefenderInfluenceLoss = rankingScoreChange * player.GetTierDefenderLossMultiplier(defender) / 100;
			RealTradeGoodsGain = GetTradeGoodsReward(gameEconomyData, defender.Level, defender.OutpostPower, player.HashedId, defender.HashedId);
			FakeTradeGoodsGain = RealTradeGoodsGain * gameEconomyData.ConfigData.OutpostFakeOpponentResourceRewardPercentage / 100;
		}
	}
}
