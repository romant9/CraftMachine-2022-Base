using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CampaignModel : TWDModelObject, IActivityManagerIntegrationInterface
	{
		private static long CheckCampaignStartInterval = 1000L;

		public int Id { get; set; }

		public bool Active { get; set; }

		public bool isBetweenEndAndRewardTime { get; set; }

		public bool CanClaimRewards { get; set; }

		public ModelList<CampaignRewardModelItem> UnclaimedPastRewards { get; set; }

		public ModelList<CampaignRewardModelItem> Rewards { get; set; }

		public int CampaignTokens { get; set; }

		public bool IsCanPopOpenStatus { get; set; }

		private long NewCampaignsCheckTimer { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			Rewards = new ModelList<CampaignRewardModelItem>();
			UnclaimedPastRewards = new ModelList<CampaignRewardModelItem>();
		}

		public override void Start()
		{
			base.Start();
			NewCampaignsCheckTimer = CheckCampaignStartInterval;
			StartRewards();
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			NewCampaignsCheckTimer -= deltaTime;
			if (NewCampaignsCheckTimer >= 0)
			{
				return;
			}
			CampaignDefinition campaignDefinition = base.manager.GameEconomyData.GetCampaignDefinition(base.manager.Player.UtcTimeStamp);
			if (campaignDefinition != null && base.manager.Player.UtcTimeStamp >= campaignDefinition.EndTimeMilliseconds && base.manager.Player.UtcTimeStamp <= campaignDefinition.RewardsAvailableMilliseconds)
			{
				isBetweenEndAndRewardTime = true;
				TryStartExistingCampaign();
			}
			if (!Active && !CanClaimRewards && !isBetweenEndAndRewardTime)
			{
				TryStartNewCampaign();
			}
			else
			{
				if (Active || isBetweenEndAndRewardTime)
				{
					CheckForAddRewards();
				}
				TryEndCampaign();
			}
			NewCampaignsCheckTimer = CheckCampaignStartInterval;
		}

		public void OnTokensChanged(ModelObject m, string changed, object args)
		{
			if (Active || isBetweenEndAndRewardTime)
			{
				CheckForAddRewards();
			}
		}

		public List<CampaignRewardsDefinition> GetRewardsBetween(int campaignId, int fromControl, int toControl)
		{
			List<CampaignRewardsDefinition> list = new List<CampaignRewardsDefinition>();
			for (int i = 0; i < ((base.gameEconomyData.CampaignRewardsDefinitions != null) ? base.gameEconomyData.CampaignRewardsDefinitions.Length : 0); i++)
			{
				CampaignRewardsDefinition campaignRewardsDefinition = base.gameEconomyData.CampaignRewardsDefinitions[i];
				if (campaignRewardsDefinition.Id == campaignId && campaignRewardsDefinition.Control > fromControl && campaignRewardsDefinition.Control <= toControl)
				{
					list.Add(campaignRewardsDefinition);
				}
			}
			return list;
		}

		public bool TryClaimReward(int modelId)
		{
			CampaignRewardModelItem model = base.manager.GetModel<CampaignRewardModelItem>(modelId);
			if (!model.Claimed)
			{
				model.ClaimReward();
			}
			return model.Claimed;
		}

		public int CalculateCurrentUnclaimedCount()
		{
			if (Rewards == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < Rewards.Count; i++)
			{
				if (Rewards[i] != null && !Rewards[i].Claimed)
				{
					num++;
				}
			}
			return num;
		}

		public bool ContainsPastCampaignRewards()
		{
			if (UnclaimedPastRewards == null)
			{
				return false;
			}
			for (int i = 0; i < UnclaimedPastRewards.Count; i++)
			{
				if (!UnclaimedPastRewards[i].Claimed)
				{
					return true;
				}
			}
			return false;
		}

		public bool TryRetrieveUnclaimedRewardsString(ref string output)
		{
			ModelList<CampaignRewardModelItem> unclaimedPastRewards = UnclaimedPastRewards;
			if (unclaimedPastRewards == null || unclaimedPastRewards.Count == 0)
			{
				return false;
			}
			if (output == null)
			{
				output = "";
			}
			for (int i = 0; i < unclaimedPastRewards.Count; i++)
			{
				if (unclaimedPastRewards[i] != null && unclaimedPastRewards[i].Rewards != null && unclaimedPastRewards[i].Rewards.RewardsList != null && !unclaimedPastRewards[i].Claimed)
				{
					if (i > 0)
					{
						output += ";";
					}
					output += unclaimedPastRewards[i].RewardsDefinition.Reward;
				}
			}
			return !string.IsNullOrEmpty(output);
		}

		public bool TryRetrieveClaimedUnclaimedEquipment(ref ModelList<EquipmentItemModel> equipmentList)
		{
			ModelList<CampaignRewardModelItem> unclaimedPastRewards = UnclaimedPastRewards;
			if (unclaimedPastRewards == null || unclaimedPastRewards.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < unclaimedPastRewards.Count; i++)
			{
				if (unclaimedPastRewards[i] != null && unclaimedPastRewards[i].Rewards != null && unclaimedPastRewards[i].Claimed && unclaimedPastRewards[i].LastRewardedEquipment != null && !unclaimedPastRewards[i].LastRewardedEquipment.IsConsumable)
				{
					equipmentList.Add(unclaimedPastRewards[i].LastRewardedEquipment);
				}
			}
			return equipmentList.Count > 0;
		}

		public bool TryRetrieveClaimedUnclaimedEquipToken(ref ModelList<EquipTokenItemModel> equipTokenItemList)
		{
			ModelList<CampaignRewardModelItem> unclaimedPastRewards = UnclaimedPastRewards;
			if (unclaimedPastRewards == null || unclaimedPastRewards.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < unclaimedPastRewards.Count; i++)
			{
				if (unclaimedPastRewards[i] != null && unclaimedPastRewards[i].Rewards != null && unclaimedPastRewards[i].Claimed && unclaimedPastRewards[i].LastRewardedEquipmentToken != null)
				{
					equipTokenItemList.Add(unclaimedPastRewards[i].LastRewardedEquipmentToken);
				}
			}
			return equipTokenItemList.Count > 0;
		}

		public CampaignDefinition GetCurrentCampaignDefinition()
		{
			return base.manager.GameEconomyData.GetCampaignDefinition(Id);
		}

		private void StartRewards()
		{
			for (int i = 0; i < ((Rewards != null) ? Rewards.Count : 0); i++)
			{
				CampaignRewardsDefinition campaignRewardDefinition = base.manager.GameEconomyData.GetCampaignRewardDefinition(Id, Rewards[i].Control);
				Rewards[i].GenerateRewards(campaignRewardDefinition);
			}
			for (int j = 0; j < ((UnclaimedPastRewards != null) ? UnclaimedPastRewards.Count : 0); j++)
			{
				CampaignRewardsDefinition campaignRewardDefinition2 = base.manager.GameEconomyData.GetCampaignRewardDefinition(Id, UnclaimedPastRewards[j].Control);
				UnclaimedPastRewards[j].GenerateRewards(campaignRewardDefinition2);
			}
		}

		private bool TryStartNewCampaign()
		{
			SaveUnclaimedRewards();
			CampaignDefinition campaignDefinition = base.manager.GameEconomyData.GetCampaignDefinition(base.manager.Player.UtcTimeStamp);
			if (campaignDefinition != null)
			{
				List<CampaignRewardsDefinition> campaignRewards = base.manager.GameEconomyData.GetCampaignRewards(campaignDefinition.Id);
				SetUpRewards(campaignRewards);
				Id = campaignDefinition.Id;
				CampaignTokens = 0;
				ResetCampaignTokenCurrency();
				Active = true;
				CanClaimRewards = true;
				IsCanPopOpenStatus = true;
			}
			return true;
		}

		private void TryStartExistingCampaign()
		{
			CampaignDefinition campaignDefinition = base.manager.GameEconomyData.GetCampaignDefinition(base.manager.Player.UtcTimeStamp);
			if (campaignDefinition != null)
			{
				List<CampaignRewardsDefinition> campaignRewards = base.manager.GameEconomyData.GetCampaignRewards(campaignDefinition.Id);
				SetUpRewards(campaignRewards);
				Id = campaignDefinition.Id;
				CanClaimRewards = true;
			}
		}

		private void SaveUnclaimedRewards()
		{
			CampaignDefinition currentCampaignDefinition = GetCurrentCampaignDefinition();
			if (Rewards == null || Rewards.Count == 0 || currentCampaignDefinition == null)
			{
				return;
			}
			if (!currentCampaignDefinition.DisableAutoCollectPostCampaign)
			{
				if (UnclaimedPastRewards == null)
				{
					UnclaimedPastRewards = new ModelList<CampaignRewardModelItem>();
					UnclaimedPastRewards.SetManager(base.manager);
				}
				for (int i = 0; i < Rewards.Count; i++)
				{
					if (Rewards[i] != null && !Rewards[i].Claimed)
					{
						UnclaimedPastRewards.Add(Rewards[i]);
					}
				}
			}
			Rewards.Clear();
		}

		private void ResetCampaignTokenCurrency()
		{
			base.manager.Player.GetCurrency(CurrencyType.CampaignToken)?.SetValue(0);
		}

		private void SetUpRewards(List<CampaignRewardsDefinition> rewards)
		{
			for (int i = 0; i < rewards.Count; i++)
			{
				rewards[i].RewardEntries = new Rewards(rewards[i].Reward, base.manager, base.manager.GetPlayer().Level);
			}
		}

		private void TryEndCampaign()
		{
			if (Active)
			{
				CampaignDefinition campaignDefinition = base.manager.GameEconomyData.GetCampaignDefinition(Id);
				if (campaignDefinition != null && base.manager.Player.UtcTimeStamp >= campaignDefinition.EndTimeMilliseconds)
				{
					Active = false;
				}
			}
			else if (CanClaimRewards)
			{
				CampaignDefinition campaignDefinition2 = base.manager.GameEconomyData.GetCampaignDefinition(Id);
				if (campaignDefinition2 != null && base.manager.Player.UtcTimeStamp >= campaignDefinition2.RewardsAvailableMilliseconds)
				{
					CanClaimRewards = false;
					isBetweenEndAndRewardTime = false;
				}
			}
		}

		private void CheckForAddRewards()
		{
			CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.CampaignToken);
			if (currency != null)
			{
				List<CampaignRewardsDefinition> rewardsBetween = GetRewardsBetween(Id, CampaignTokens, currency.Value);
				for (int i = 0; i < rewardsBetween.Count; i++)
				{
					CampaignRewardsDefinition rewardDefinition = rewardsBetween[i];
					AddReward(rewardDefinition);
				}
				CampaignTokens = currency.Value;
			}
		}

		private void AddReward(CampaignRewardsDefinition rewardDefinition)
		{
			if (rewardDefinition != null)
			{
				CampaignRewardModelItem campaignRewardModelItem = new CampaignRewardModelItem();
				campaignRewardModelItem.SetManager(base.manager);
				campaignRewardModelItem.Initialize();
				campaignRewardModelItem.Start();
				campaignRewardModelItem.Control = rewardDefinition.Control;
				campaignRewardModelItem.GenerateRewards(rewardDefinition);
				Rewards.Add(campaignRewardModelItem);
			}
		}

		public string GetIntegrationEventId()
		{
			return "Campaign";
		}

		public bool CanShowInActivityList()
		{
			if (!Active)
			{
				return CanClaimRewards;
			}
			return true;
		}

		public bool AreThereAnyUnclaimedReward()
		{
			if (Rewards != null)
			{
				for (int i = 0; i < Rewards.Count; i++)
				{
					CampaignRewardModelItem campaignRewardModelItem = Rewards[i];
					if (campaignRewardModelItem != null && campaignRewardModelItem.Claimable)
					{
						return true;
					}
				}
			}
			if (UnclaimedPastRewards != null)
			{
				for (int j = 0; j < UnclaimedPastRewards.Count; j++)
				{
					CampaignRewardModelItem campaignRewardModelItem2 = UnclaimedPastRewards[j];
					if (campaignRewardModelItem2 != null && campaignRewardModelItem2.Claimable)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool AreThereCanCompleteTask()
		{
			return false;
		}

		public bool IsActivityOpen()
		{
			return IsCanPopOpenStatus;
		}
	}
}
