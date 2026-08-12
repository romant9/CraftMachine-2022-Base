using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class RecycleWeaponActivityModel : TWDModelObject, IActivityManagerIntegrationInterface
	{
		public int Identifier;

		public int Type;

		public int RecycledCount;

		[JsonIgnore]
		private RecycleWeaponDefinition _currentDefinition;

		public bool IsCanPopOpenStatus { get; set; }

		[JsonIgnore]
		public Rewards LastRecycleRewards { get; private set; }

		[JsonIgnore]
		public RecycleWeaponDefinition CurrentDefinition
		{
			get
			{
				if (_currentDefinition == null)
				{
					_currentDefinition = base.manager?.GameEconomyData?.GetRecycleWeaponDefinition(Identifier);
				}
				return _currentDefinition;
			}
		}

		public RecycleWeaponActivityModel(int identifier, int type)
		{
			Identifier = identifier;
			Type = type;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Start()
		{
			base.Start();
		}

		public override bool IsValid()
		{
			return true;
		}

		public string GetIntegrationEventId()
		{
			return "RecycleWeapon";
		}

		public bool CanShowInActivityList()
		{
			return IsRecycleWeaponActive();
		}

		public bool IsActivityOpen()
		{
			if (!IsCanPopOpenStatus)
			{
				return false;
			}
			return IsRecycleWeaponActive();
		}

		public bool AreThereAnyUnclaimedReward()
		{
			IsRecycleWeaponActive();
			return false;
		}

		public bool AreThereCanCompleteTask()
		{
			IsRecycleWeaponActive();
			return false;
		}

		public bool RecycleBlueprints(List<string> equipTokenIds)
		{
			if (!CanRecycle() || CurrentDefinition == null || CurrentDefinition.Type != 1)
			{
				return false;
			}
			LastRecycleRewards = new Rewards();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			bool flag = true;
			foreach (string equipTokenId in equipTokenIds)
			{
				if (!RecycleSingleBlueprint(equipTokenId, 1))
				{
					flag = false;
					break;
				}
				if (dictionary.ContainsKey(equipTokenId))
				{
					dictionary[equipTokenId]++;
				}
				else
				{
					dictionary[equipTokenId] = 1;
				}
			}
			foreach (KeyValuePair<string, int> item in dictionary)
			{
				base.manager?.TdMetrics?.SetEventType("RecycleBlueprints").AddProperty("RecycleBlueprintsID", item.Key).AddProperty("RecycleBlueprintsNum", item.Value)
					.Send();
				base.manager?.Metrics?.AddRecycleBlueprints(item.Key, item.Value).Send();
			}
			if (!flag)
			{
				return false;
			}
			MergeSameRewards();
			return true;
		}

		public bool RecycleWeapons(List<EquipmentItemModel> equipments)
		{
			if (!CanRecycle() || CurrentDefinition == null || CurrentDefinition.Type != 2)
			{
				return false;
			}
			LastRecycleRewards = new Rewards();
			foreach (EquipmentItemModel equipment in equipments)
			{
				if (!RecycleSingleWeapon(equipment))
				{
					return false;
				}
			}
			MergeSameRewards();
			return true;
		}

		private bool RecycleSingleBlueprint(string equipTokenId, int count)
		{
			if (count <= 0)
			{
				return false;
			}
			if (RecycledCount + count > CurrentDefinition.Limit)
			{
				return false;
			}
			EquipTokenItemModel equipTokenItemModel = base.manager.Player.EquipTokenContainer.EquipTokenItems.Find((EquipTokenItemModel x) => x.EquipTokenId == equipTokenId);
			if (equipTokenItemModel == null)
			{
				return false;
			}
			if (equipTokenItemModel.Definition.SurvivorClass.ToString() != CurrentDefinition.Object)
			{
				return false;
			}
			if (equipTokenItemModel.OwnedTokensAmount < count)
			{
				return false;
			}
			RecycleWeaponRewardDefinition recycleWeaponRewardDefinition = base.manager.GameEconomyData.GetRecycleWeaponRewardDefinition(CurrentDefinition.Reward, Type);
			if (recycleWeaponRewardDefinition == null)
			{
				return false;
			}
			equipTokenItemModel.AddEquipToken(-count);
			GiveCbpReward(recycleWeaponRewardDefinition.CbpRewards);
			GiveSPSkillPackageRewards(recycleWeaponRewardDefinition, LastRecycleRewards);
			AddRecycledCount(count);
			return true;
		}

		private bool RecycleSingleWeapon(EquipmentItemModel equipment)
		{
			if (equipment == null || equipment.Owner != null)
			{
				return false;
			}
			RecycleWeaponRewardDefinition recycleWeaponRewardDefinition = base.manager.GameEconomyData.GetRecycleWeaponRewardDefinition(CurrentDefinition.Reward, equipment.BreakthroughLevel, Type);
			if (recycleWeaponRewardDefinition == null)
			{
				return false;
			}
			if (base.manager.GameEconomyData.GetEquipTokenDefinitionByRelateEquipId(equipment.EquipmentDefinitionIdentifier) != null)
			{
				GiveCbpReward(recycleWeaponRewardDefinition.CbpRewards);
			}
			GiveSPSkillPackageRewards(recycleWeaponRewardDefinition, LastRecycleRewards);
			Cashier cashier = new Cashier(base.manager);
			int totalCost = equipment.GetScrapCashier.GetTotalCost(CurrencyType.SurvivalPoints);
			if (totalCost > 0)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.Refund);
				cashierItem.SetCost(CurrencyType.SurvivalPoints, totalCost);
				cashier.AddItem(cashierItem);
				LastRecycleRewards.RewardsList.Add(new RewardCurrency
				{
					CurrencyType = CurrencyType.SurvivalPoints,
					Amount = totalCost
				});
			}
			if (recycleWeaponRewardDefinition.ApoToken > 0)
			{
				CashierItem cashierItem2 = new CashierItem(PurchaseType.Refund);
				cashierItem2.SetCost(CurrencyType.ApocalypticEquipToken, recycleWeaponRewardDefinition.ApoToken);
				cashier.AddItem(cashierItem2);
				LastRecycleRewards.RewardsList.Add(new RewardCurrency
				{
					CurrencyType = CurrencyType.ApocalypticEquipToken,
					Amount = recycleWeaponRewardDefinition.ApoToken
				});
			}
			base.manager.Player.Equipment.ScrapEquipmentItem(equipment, deletedBySupport: false, cashier);
			AddRecycledCount(1);
			string equipmentDefinitionIdentifier = equipment.EquipmentDefinitionIdentifier;
			int breakthroughLevel = equipment.BreakthroughLevel;
			base.manager?.TdMetrics?.SetEventType("RecycleWeapon").AddProperty("RecycleWeaponID", equipmentDefinitionIdentifier).AddProperty("RecycleWeaponBreakthroughLv", breakthroughLevel)
				.Send();
			base.manager?.Metrics?.AddRecycleWeapon(equipmentDefinitionIdentifier, breakthroughLevel).Send();
			return true;
		}

		private void GiveSPSkillPackageRewards(RecycleWeaponRewardDefinition rewardDef, Rewards rewards)
		{
			if (rewardDef.SPSkillPackageEntries == null)
			{
				return;
			}
			foreach (SPSkillPackageEntry sPSkillPackageEntry in rewardDef.SPSkillPackageEntries)
			{
				List<RecycleWeaponSPSkillPackage> recycleWeaponSPSkillPackages = base.manager.GameEconomyData.GetRecycleWeaponSPSkillPackages(sPSkillPackageEntry.PackageId);
				if (recycleWeaponSPSkillPackages == null || recycleWeaponSPSkillPackages.Count == 0)
				{
					continue;
				}
				foreach (RecycleWeaponSPSkillPackage item in base.manager.Player.PlayerRandom.WeightedRandomList(recycleWeaponSPSkillPackages, sPSkillPackageEntry.Count, (RecycleWeaponSPSkillPackage x) => x.Weight))
				{
					if (item != null && !string.IsNullOrEmpty(item.Content))
					{
						IReward reward = GiveSPSkillContent(item.Content);
						if (reward != null)
						{
							rewards.RewardsList.Add(reward);
						}
					}
				}
			}
		}

		private IReward GiveSPSkillContent(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return null;
			}
			string spRemoldSkillType = content;
			if (content.StartsWith("remoldskill(") && content.EndsWith(")"))
			{
				spRemoldSkillType = content.Substring("remoldskill(".Length, content.Length - "remoldskill(".Length - 1);
			}
			RewardRemoldSkill rewardRemoldSkill = new RewardRemoldSkill
			{
				SpRemoldSkillType = spRemoldSkillType,
				Amount = 1
			};
			rewardRemoldSkill.Give(base.manager);
			if (rewardRemoldSkill.GivenRewardResult == null)
			{
				return null;
			}
			if (rewardRemoldSkill.GivenRewardResult.RewardType == ModSkillRewardType.NewAcquisition)
			{
				return rewardRemoldSkill;
			}
			if (rewardRemoldSkill.GivenRewardResult.DuplicateRewards == null || rewardRemoldSkill.GivenRewardResult.DuplicateRewards.RewardsList == null || rewardRemoldSkill.GivenRewardResult.DuplicateRewards.RewardsList.Count == 0)
			{
				return null;
			}
			return rewardRemoldSkill.GivenRewardResult.DuplicateRewards.RewardsList[0];
		}

		public bool IsRecycleWeaponActive()
		{
			if (base.manager?.Player == null)
			{
				return false;
			}
			if (CurrentDefinition == null)
			{
				return false;
			}
			return CurrentDefinition.IsActive(base.manager.Player.UtcTimeStamp);
		}

		public bool CanRecycle()
		{
			if (CurrentDefinition == null)
			{
				return false;
			}
			return RecycledCount < CurrentDefinition.Limit;
		}

		public void AddRecycledCount(int count)
		{
			RecycledCount += count;
		}

		private void MergeSameRewards()
		{
			if (LastRecycleRewards == null || LastRecycleRewards.RewardsList == null)
			{
				return;
			}
			List<IReward> rewardsList = LastRecycleRewards.RewardsList;
			bool[] array = new bool[rewardsList.Count];
			List<IReward> list = new List<IReward>();
			for (int i = 0; i < rewardsList.Count; i++)
			{
				if (array[i])
				{
					continue;
				}
				IReward reward = rewardsList[i];
				RewardCurrency rewardCurrency = (reward as RewardCurrency)?.GetClone();
				RewardRemoldSkill rewardRemoldSkill = reward as RewardRemoldSkill;
				if (rewardRemoldSkill != null)
				{
					rewardRemoldSkill = new RewardRemoldSkill
					{
						SpRemoldSkillType = rewardRemoldSkill.SpRemoldSkillType,
						Amount = rewardRemoldSkill.Amount
					};
				}
				for (int j = i + 1; j < rewardsList.Count; j++)
				{
					if (array[j])
					{
						continue;
					}
					IReward reward2 = rewardsList[j];
					if (rewardCurrency != null)
					{
						if (reward2 is RewardCurrency rewardCurrency2 && rewardCurrency2.CurrencyType == rewardCurrency.CurrencyType)
						{
							rewardCurrency.Amount += rewardCurrency2.Amount;
							array[j] = true;
						}
					}
					else if (rewardRemoldSkill != null && reward2 is RewardRemoldSkill rewardRemoldSkill2 && rewardRemoldSkill2.SpRemoldSkillType == rewardRemoldSkill.SpRemoldSkillType)
					{
						rewardRemoldSkill.Amount += rewardRemoldSkill2.Amount;
						array[j] = true;
					}
				}
				if (rewardCurrency != null)
				{
					list.Add(rewardCurrency);
				}
				else if (rewardRemoldSkill != null)
				{
					list.Add(rewardRemoldSkill);
				}
				else
				{
					list.Add(reward);
				}
			}
			LastRecycleRewards.RewardsList = list;
		}

		private void GiveCbpReward(Rewards rewards)
		{
			IReward reward = rewards.RewardsList[0];
			reward.Give(base.manager);
			LastRecycleRewards.RewardsList.Add(reward);
		}
	}
}
