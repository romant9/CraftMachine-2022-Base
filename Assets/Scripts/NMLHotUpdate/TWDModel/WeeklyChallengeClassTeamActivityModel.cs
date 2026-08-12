using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class WeeklyChallengeClassTeamActivityModel : TWDModelObject, IActivityManagerIntegrationInterface
	{
		public const string ClassTeamCloseExchangeRewards = "ClassTeamCloseExchangeRewards";

		public int Id { get; set; }

		public WeeklyChallengeClassTeamShopModel Shop { get; set; }

		public bool IsCanPopOpenStatus { get; set; }

		[JsonIgnore]
		public Dictionary<CurrencyType, int> LastBattleRewards { get; private set; }

		public List<LootEntry> PendingSkipClaimRewards { get; set; }

		[JsonIgnore]
		public ClassTeamDefinition CurrentDefinition
		{
			get
			{
				if (base.manager == null || base.manager.GameEconomyData == null)
				{
					return null;
				}
				return base.manager.GameEconomyData.GetClassTeamDefinition(Id);
			}
		}

		[JsonIgnore]
		public bool IsActive
		{
			get
			{
				if (Id <= 0 || CurrentDefinition == null)
				{
					return false;
				}
				if (base.manager == null || base.manager.Player == null)
				{
					return false;
				}
				long utcTimeStamp = base.manager.Player.UtcTimeStamp;
				if (utcTimeStamp >= CurrentDefinition.StartTimeMilliseconds)
				{
					return utcTimeStamp < CurrentDefinition.EndTimeMilliseconds;
				}
				return false;
			}
		}

		public void ClearLastBattleReward()
		{
			LastBattleRewards = new Dictionary<CurrencyType, int>();
		}

		public void QueueSkipClaimRewards(Rewards template, int times)
		{
			if (template == null || times <= 0 || base.manager == null || base.manager.Player == null || template.RewardsList == null || template.RewardsList.Count == 0)
			{
				return;
			}
			if (PendingSkipClaimRewards == null)
			{
				PendingSkipClaimRewards = new List<LootEntry>();
			}
			for (int i = 0; i < times; i++)
			{
				for (int j = 0; j < template.RewardsList.Count; j++)
				{
					LootEntry lootEntry = CreatePendingLootEntry(template.RewardsList[j]);
					if (lootEntry != null)
					{
						PendingSkipClaimRewards.Add(lootEntry);
					}
				}
			}
			RecordBattleReward(template, times);
		}

		public void ClaimPendingSkipRewards(List<LootEntry> lootEntries)
		{
			if (PendingSkipClaimRewards == null || PendingSkipClaimRewards.Count == 0 || base.manager == null || base.manager.Player == null)
			{
				return;
			}
			for (int i = 0; i < PendingSkipClaimRewards.Count; i++)
			{
				LootEntry lootEntry = PendingSkipClaimRewards[i];
				if (lootEntry != null)
				{
					base.manager.Player.LootManager.GiveLoot(lootEntry);
					lootEntries?.Add(lootEntry);
				}
			}
			PendingSkipClaimRewards.Clear();
		}

		private LootEntry CreatePendingLootEntry(IReward reward)
		{
			if (reward == null || base.manager == null || base.manager.Player == null)
			{
				return null;
			}
			LootEntry lootEntry;
			if (reward.Type == RewardType.TradeCrate && reward is RewardTradeCrate rewardTradeCrate)
			{
				lootEntry = base.manager.Player.LootManager.CreateTradeCrateLoot(rewardTradeCrate.TradeCrateId, DropEventDefinition.DropEventType.MissionChallenge, ignoreCummulativeProbability: true, "WeeklyChallengeClassTeamReward");
			}
			else if (reward.Type != RewardType.Equipment || !(reward is RewardEquipment reward2))
			{
				lootEntry = ((reward.Type != RewardType.Avatars) ? base.manager.Player.LootManager.CreateCurrencyLoot(reward, DropType.Gold, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency) : base.manager.Player.LootManager.CreateAvatarsLootEntry(reward, DropType.Gold));
			}
			else
			{
				lootEntry = base.manager.Player.LootManager.CreateConsumablesLoot(reward2, DropType.Gold);
				if (lootEntry != null && base.manager.GameEconomyData != null)
				{
					lootEntry.DropEventDefinition = base.manager.GameEconomyData.GetDropEvent(DropEventDefinition.DropEventType.MissionChallenge, DropEventDefinition.DropEventContext.Normal, DropEventDefinition.DropEventTag.ChallengeCrateGold);
				}
			}
			if (lootEntry != null)
			{
				lootEntry.Type = LootEntryType.None;
			}
			return lootEntry;
		}

		public void RecordBattleReward(Rewards template, int times)
		{
			if (template == null || times <= 0)
			{
				return;
			}
			if (LastBattleRewards == null)
			{
				LastBattleRewards = new Dictionary<CurrencyType, int>();
			}
			List<IReward> rewardsOfType = template.GetRewardsOfType(RewardType.Currency);
			if (rewardsOfType == null)
			{
				return;
			}
			for (int i = 0; i < rewardsOfType.Count; i++)
			{
				if (rewardsOfType[i] is RewardCurrency rewardCurrency)
				{
					int num = rewardCurrency.Amount * times;
					if (LastBattleRewards.ContainsKey(rewardCurrency.CurrencyType))
					{
						LastBattleRewards[rewardCurrency.CurrencyType] += num;
					}
					else
					{
						LastBattleRewards[rewardCurrency.CurrencyType] = num;
					}
				}
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			Id = -1;
			PendingSkipClaimRewards = new List<LootEntry>();
			Shop = new WeeklyChallengeClassTeamShopModel();
			Shop.SetManager(base.manager);
			Shop.Initialize();
		}

		public override void Start()
		{
			if (Shop == null)
			{
				Shop = new WeeklyChallengeClassTeamShopModel();
				Shop.SetManager(base.manager);
				Shop.Initialize();
			}
			if (PendingSkipClaimRewards == null)
			{
				PendingSkipClaimRewards = new List<LootEntry>();
			}
			base.Start();
			CurrentDefinition?.InitializeRewards(base.manager);
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (base.manager == null || base.manager.Player == null || base.manager.GameEconomyData == null || Shop == null)
			{
				return;
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			ClassTeamDefinition currentClassTeamDefinition = base.manager.GameEconomyData.GetCurrentClassTeamDefinition(utcTimeStamp);
			if (currentClassTeamDefinition == null)
			{
				if (Id > 0)
				{
					Shop.ExchangeOnClose(Id);
					NotifyCloseExchangeRewards();
					base.Debug.LogInfo($"WeeklyChallengeClassTeamActivityModel Ended Id:{Id}, now:{utcTimeStamp}");
					Id = -1;
				}
			}
			else if (currentClassTeamDefinition.ChallengeID != Id)
			{
				if (Id > 0)
				{
					Shop.ExchangeOnClose(Id);
					NotifyCloseExchangeRewards();
					base.Debug.LogInfo($"WeeklyChallengeClassTeamActivityModel Switching Id:{Id} -> {currentClassTeamDefinition.ChallengeID}, now:{utcTimeStamp}");
				}
				Id = currentClassTeamDefinition.ChallengeID;
				currentClassTeamDefinition.InitializeRewards(base.manager);
				Shop.ResetForNewActivity();
				IsCanPopOpenStatus = true;
				base.Debug.LogInfo($"WeeklyChallengeClassTeamActivityModel Started Id:{Id}, now:{utcTimeStamp}");
			}
		}

		private void NotifyCloseExchangeRewards()
		{
			if (Shop != null && Shop.LastCloseExchangeRewards != null && Shop.LastCloseExchangeRewards.Count > 0)
			{
				NotifyChange("ClassTeamCloseExchangeRewards");
			}
		}

		public string GetIntegrationEventId()
		{
			return "WeeklyChallengeClassTeamChallenge";
		}

		public bool CanShowInActivityList()
		{
			return IsActive;
		}

		public bool IsActivityOpen()
		{
			return IsCanPopOpenStatus;
		}

		public bool AreThereAnyUnclaimedReward()
		{
			return false;
		}

		public bool AreThereCanCompleteTask()
		{
			if (IsActive && Shop != null)
			{
				return Shop.HasAnyExchangeRedPoint();
			}
			return false;
		}
	}
}
