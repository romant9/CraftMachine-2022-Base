using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class EquipPrizeWheelModel : TWDModelObject
	{
		public int LuckyTime { get; set; }

		public Dictionary<string, int> GoldRadioLuckyTimeDict { get; set; }

		public Dictionary<string, bool> GoldRadioPoolViewedDict { get; set; }

		public Dictionary<string, string> GoldRadioPoolDailyViewedDict { get; set; }

		[JsonIgnore]
		public List<EquipPrizeWheelReward> Rewards { get; set; }

		[JsonIgnore]
		public EquipPrizeWheelDefinition CurrentEquipPrizeWheelDefinition { get; set; }

		[JsonIgnore]
		public EquipPrizeType CurrentEquipPrizeType { get; set; }

		private bool RedDotEnabled => base.manager?.GameEconomyData?.ConfigData?.GoldRadioCallNotice ?? true;

		public override void Initialize()
		{
			base.Initialize();
			LuckyTime = 0;
			GoldRadioLuckyTimeDict = new Dictionary<string, int>();
			GoldRadioPoolViewedDict = new Dictionary<string, bool>();
			GoldRadioPoolDailyViewedDict = new Dictionary<string, string>();
		}

		public override bool IsValid()
		{
			return true;
		}

		public void AddReward(EquipPrizeType equipPrizeType, string slotNumber)
		{
			ClearReward();
			List<TdEquipPrizesWheel> trait_after = new List<TdEquipPrizesWheel>();
			int num = ((equipPrizeType != EquipPrizeType.Ten) ? 1 : 10);
			for (int i = 0; i < num; i++)
			{
				EquipPrizeWheelReward equipPrizeWheelReward = RandomRewards(slotNumber, ref trait_after);
				if (equipPrizeWheelReward != null)
				{
					if (Rewards == null)
					{
						Rewards = new List<EquipPrizeWheelReward>();
					}
					Rewards.Add(equipPrizeWheelReward); //remoldskill(Scout_4013)
				}
			}
			if (!IsAuto && OfflineManager.IsUseSendMetrics) base.manager.TdMetrics.AddProperty("equip_prize_delta_detail", trait_after);
		}

		private void ClearReward()
		{
			Rewards?.Clear();
		}

		private EquipPrizeWheelReward RandomRewards(string SlotNumber, ref List<TdEquipPrizesWheel> trait_after)
		{
			List<EquipPrizeWheelReward> list = base.manager.GameEconomyData.EquipPrizeWheelRewards.Where((EquipPrizeWheelReward x) => x.ID == SlotNumber).ToList(); //122
			List<EquipPrizeWheelReward> list2 = list.Where((EquipPrizeWheelReward x) => x.bigPrize > 0).ToList(); //3
			int luckPointThreshold = GetLuckPointThreshold();
			int currentLuckyTime = GetCurrentLuckyTime();
			//EquipPrizeWheelReward equipPrizeWheelReward = base.manager.Player.PlayerRandom.WeightedRandomList(list, 1, x => x.GetWeight(currentLuckyTime)).First();

			EquipPrizeWheelReward equipPrizeWheelReward = base.manager.Player.PlayerRandom.WeightedRandomList((luckPointThreshold <= currentLuckyTime) ? list2 : list, 1, (EquipPrizeWheelReward x) => x.GetWeight(currentLuckyTime)).First();
			Rewards rewards = new Rewards(equipPrizeWheelReward.Reward);
			if (CurrentEquipPrizeWheelDefinition != null && CurrentEquipPrizeWheelDefinition.RadioType == RadioType.GoldRadio && rewards?.RewardsList != null)
			{
				for (int num = 0; num < rewards.RewardsList.Count; num++)
				{
					if (rewards.RewardsList[num] is RewardCurrency { CurrencyType: CurrencyType.SPTraitsUpgradeToken } rewardCurrency)
					{
						rewardCurrency.CanOverflowMax = true;
					}
				}
			}
			currentLuckyTime++;
			if (equipPrizeWheelReward.bigPrize > 0)// || currentLuckyTime > luckPointThreshold)
			{
				currentLuckyTime = 0;
			}

			if (IsAuto)
			{
				bool isIsFineCompared = IsFineCompared(equipPrizeWheelReward);
				if (isIsFineCompared)
				{
					if (NewPhonePopup.Instance.IsWeaponSkillMode)
					{
						NewPhonePopup.Instance.GoldRadioWeaponContainer.IsQuick = false;
					}
					else
					{
						NewPhonePopup.Instance.PhoneWeaponContainer.IsQuick = false;
					}
				}
			}
			SetCurrentLuckyTime(currentLuckyTime);
			TdEquipPrizesWheel tdEquipPrizesWheel = new TdEquipPrizesWheel();
			tdEquipPrizesWheel.delta_name = equipPrizeWheelReward.Reward;
			tdEquipPrizesWheel.delta_rarity = equipPrizeWheelReward.Rarity;
			tdEquipPrizesWheel.delta_luck = currentLuckyTime;
			trait_after.Add(tdEquipPrizesWheel);
			if (!OfflineManager.IsNoAddRewards)
			{
				rewards.Give(base.manager);
			}
			if (rewards?.RewardsList?.FirstOrDefault((IReward x) => x != null && x is RewardRemoldSkill) is RewardRemoldSkill { GivenRewardResult: not null })
			{
				return CreateRuntimeRewardResult(equipPrizeWheelReward, rewards);
			}
			return equipPrizeWheelReward;
		}

		private EquipPrizeWheelReward CreateRuntimeRewardResult(EquipPrizeWheelReward rewardTemplate, Rewards rewardEntries)
		{
			return new EquipPrizeWheelReward
			{
				ID = rewardTemplate.ID,
				Slot = rewardTemplate.Slot,
				Reward = rewardTemplate.Reward,
				bigPrize = rewardTemplate.bigPrize,
				bigPrizeTime = rewardTemplate.bigPrizeTime,
				Rarity = rewardTemplate.Rarity,
				Weight = rewardTemplate.Weight,
				RewardEntries = rewardEntries
			};
		}

		public int GetCurrentLuckyTime()
		{
			if (CurrentEquipPrizeWheelDefinition != null && CurrentEquipPrizeWheelDefinition.RadioType == RadioType.GoldRadio)
			{
				string identifier = CurrentEquipPrizeWheelDefinition.Identifier;
				if (GoldRadioLuckyTimeDict == null)
				{
					GoldRadioLuckyTimeDict = new Dictionary<string, int>();
				}
				if (!GoldRadioLuckyTimeDict.ContainsKey(identifier))
				{
					GoldRadioLuckyTimeDict[identifier] = 0;
				}
				return GoldRadioLuckyTimeDict[identifier];
			}
			return LuckyTime;
		}

		private void SetCurrentLuckyTime(int value)
		{
			if (CurrentEquipPrizeWheelDefinition != null && CurrentEquipPrizeWheelDefinition.RadioType == RadioType.GoldRadio)
			{
				string identifier = CurrentEquipPrizeWheelDefinition.Identifier;
				if (GoldRadioLuckyTimeDict == null)
				{
					GoldRadioLuckyTimeDict = new Dictionary<string, int>();
				}
				GoldRadioLuckyTimeDict[identifier] = value;
			}
			else
			{
				LuckyTime = value;
			}
		}

		private int GetLuckPointThreshold()
		{
			if (CurrentEquipPrizeWheelDefinition != null && CurrentEquipPrizeWheelDefinition.RadioType == RadioType.GoldRadio)
			{
				return base.manager.GameEconomyData.ConfigData.EquipPrizeWheelLuckPoint_GoldRadio;
			}
			return base.manager.GameEconomyData.ConfigData.EquipPrizeWheelLuckPoint;
		}

		public bool ShouldShowGoldRadioPoolRedDot()
		{
			if (base.manager == null || base.manager.GameEconomyData == null)
			{
				return false;
			}
			if (GoldRadioPoolViewedDict == null)
			{
				GoldRadioPoolViewedDict = new Dictionary<string, bool>();
			}
			List<EquipPrizeWheelDefinition> openEquipPrizeWheelDefinition = base.manager.GameEconomyData.GetOpenEquipPrizeWheelDefinition(base.manager.Player.UtcTimeStamp);
			if (openEquipPrizeWheelDefinition == null || openEquipPrizeWheelDefinition.Count == 0)
			{
				return false;
			}
			foreach (EquipPrizeWheelDefinition item in openEquipPrizeWheelDefinition)
			{
				if (item.RadioType != RadioType.GoldRadio)
				{
					continue;
				}
				string identifier = item.Identifier;
				if (!GoldRadioPoolViewedDict.ContainsKey(identifier) || !GoldRadioPoolViewedDict[identifier])
				{
					return true;
				}
				if (!RedDotEnabled)
				{
					continue;
				}
				GoldRadioCallDenifition goldRadioCallDenifitionByID = base.manager.GameEconomyData.GetGoldRadioCallDenifitionByID(identifier);
				if (goldRadioCallDenifitionByID != null && goldRadioCallDenifitionByID.Type == 2)
				{
					string todayUtc8DateKey = GetTodayUtc8DateKey();
					if (GoldRadioPoolDailyViewedDict == null)
					{
						GoldRadioPoolDailyViewedDict = new Dictionary<string, string>();
					}
					if (!GoldRadioPoolDailyViewedDict.ContainsKey(identifier) || GoldRadioPoolDailyViewedDict[identifier] != todayUtc8DateKey)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool ShouldShowRedDotForPool(string identifier)
		{
			if (string.IsNullOrEmpty(identifier))
			{
				return false;
			}
			if (base.manager == null || base.manager.GameEconomyData == null)
			{
				return false;
			}
			if (GoldRadioPoolViewedDict == null)
			{
				GoldRadioPoolViewedDict = new Dictionary<string, bool>();
			}
			List<EquipPrizeWheelDefinition> openEquipPrizeWheelDefinition = base.manager.GameEconomyData.GetOpenEquipPrizeWheelDefinition(base.manager.Player.UtcTimeStamp);
			if (openEquipPrizeWheelDefinition == null || openEquipPrizeWheelDefinition.Count == 0)
			{
				return false;
			}
			EquipPrizeWheelDefinition equipPrizeWheelDefinition = openEquipPrizeWheelDefinition.Find((EquipPrizeWheelDefinition d) => d.Identifier == identifier);
			if (equipPrizeWheelDefinition == null || equipPrizeWheelDefinition.RadioType != RadioType.GoldRadio)
			{
				return false;
			}
			GoldRadioCallDenifition goldRadioCallDenifitionByID = base.manager.GameEconomyData.GetGoldRadioCallDenifitionByID(identifier);
			if (goldRadioCallDenifitionByID != null && goldRadioCallDenifitionByID.Type == 2 && RedDotEnabled)
			{
				string todayUtc8DateKey = GetTodayUtc8DateKey();
				if (GoldRadioPoolDailyViewedDict == null)
				{
					GoldRadioPoolDailyViewedDict = new Dictionary<string, string>();
				}
				if (GoldRadioPoolDailyViewedDict.ContainsKey(identifier))
				{
					return GoldRadioPoolDailyViewedDict[identifier] != todayUtc8DateKey;
				}
				return true;
			}
			if (!GoldRadioPoolViewedDict.ContainsKey(identifier) || !GoldRadioPoolViewedDict[identifier])
			{
				return true;
			}
			return false;
		}

		public List<string> GetPoolsWithRedDot()
		{
			List<string> list = new List<string>();
			if (base.manager == null || base.manager.GameEconomyData == null)
			{
				return list;
			}
			if (GoldRadioPoolViewedDict == null)
			{
				GoldRadioPoolViewedDict = new Dictionary<string, bool>();
			}
			List<EquipPrizeWheelDefinition> openEquipPrizeWheelDefinition = base.manager.GameEconomyData.GetOpenEquipPrizeWheelDefinition(base.manager.Player.UtcTimeStamp);
			if (openEquipPrizeWheelDefinition == null || openEquipPrizeWheelDefinition.Count == 0)
			{
				return list;
			}
			foreach (EquipPrizeWheelDefinition item in openEquipPrizeWheelDefinition)
			{
				if (item.RadioType == RadioType.GoldRadio && (!GoldRadioPoolViewedDict.ContainsKey(item.Identifier) || !GoldRadioPoolViewedDict[item.Identifier]))
				{
					list.Add(item.Identifier);
				}
			}
			return list;
		}

		public void MarkGoldRadioPoolAsViewed(string identifier)
		{
			if (base.manager == null || base.manager.GameEconomyData == null || string.IsNullOrEmpty(identifier))
			{
				return;
			}
			if (GoldRadioPoolViewedDict == null)
			{
				GoldRadioPoolViewedDict = new Dictionary<string, bool>();
			}
			List<EquipPrizeWheelDefinition> openEquipPrizeWheelDefinition = base.manager.GameEconomyData.GetOpenEquipPrizeWheelDefinition(base.manager.Player.UtcTimeStamp);
			if (openEquipPrizeWheelDefinition == null || openEquipPrizeWheelDefinition.Count == 0)
			{
				return;
			}
			EquipPrizeWheelDefinition equipPrizeWheelDefinition = openEquipPrizeWheelDefinition.Find((EquipPrizeWheelDefinition d) => d.Identifier == identifier);
			if (equipPrizeWheelDefinition != null && equipPrizeWheelDefinition.RadioType == RadioType.GoldRadio)
			{
				GoldRadioPoolViewedDict[identifier] = true;
			}
			GoldRadioCallDenifition goldRadioCallDenifitionByID = base.manager.GameEconomyData.GetGoldRadioCallDenifitionByID(identifier);
			if (goldRadioCallDenifitionByID != null && goldRadioCallDenifitionByID.Type == 2 && RedDotEnabled)
			{
				if (GoldRadioPoolDailyViewedDict == null)
				{
					GoldRadioPoolDailyViewedDict = new Dictionary<string, string>();
				}
				GoldRadioPoolDailyViewedDict[identifier] = GetTodayUtc8DateKey();
			}
		}

		private string GetTodayUtc8DateKey()
		{
			DateTime dateTime = DateTime.UtcNow;
			if (base.manager != null && base.manager.Player != null)
			{
				dateTime = base.manager.Player.UtcTime;
			}
			return dateTime.AddHours(8.0).ToString("yyyyMMdd");
		}



		#region myparams
		private bool IsAuto;
		public enum PrizeCounterType
		{
			Any,
			Token,
			TokenPart,
			RemodelPart,
			ApoPart,
			Skill,
			SkillPart,
			Favorites,
			FavoritesPart,
			SkillShooter,
			SkillHunter,
			SkillAssault,
			SkillScout,
			SkillWarrior,
			SkillBruiser,
			SkillShooterPart,
			SkillHunterPart,
			SkillAssaultPart,
			SkillScoutPart,
			SkillWarriorPart,
			SkillBruiserPart
		}

		public void SetIsAuto(bool isAuto)
		{
			IsAuto = isAuto;
		}
		#endregion
		#region mycode
		private bool ChangeCounterStatePhone(PrizeCounterType prizeCounterType)
		{
			if (NewPhonePopup.Instance.IsWeaponSkillMode) return false;

			var rewardType = GetCurrentPrizeCounterType();
			return rewardType == prizeCounterType || rewardType == PrizeCounterType.Any;
		}

		private bool ChangeCounterStateGold(string skillTypeString)
		{
			if (!NewPhonePopup.Instance.IsWeaponSkillMode) return false;

			var rewardType = GetCurrentPrizeCounterType();

			string classLevel;
			string classtwd;
			PrizeCounterType prizeType;
			var classString = skillTypeString.Split('_');
			bool isSkillPart = skillTypeString.StartsWith("SkillToken");

			if (isSkillPart)
			{
				classtwd = classString[1];
				classLevel = classString[2];
				prizeType = GetPrizeType(classtwd, false);
			}
			else
			{
				classtwd = classString[0];
				classLevel = classString[1];
				prizeType = GetPrizeType(classtwd, true);
			}

			var skillType = classtwd + "_" + classLevel;
			bool isBigStar = int.TryParse(classLevel[0].ToString(), out int levelResult) && levelResult >= NewPhonePopup.Instance.GoldRadioWeaponContainer.RewardStarValue;

			if (!isSkillPart && rewardType == PrizeCounterType.Favorites || isSkillPart && rewardType == PrizeCounterType.FavoritesPart)
			{
				return NewPhonePopup.Instance.FavoriteModSkillList.Contains(skillType);
			}
			else
			{
				return isBigStar &&
					(rewardType == prizeType ||
					(!isSkillPart && rewardType == PrizeCounterType.Skill) ||
					(isSkillPart && rewardType == PrizeCounterType.SkillPart));
			}
		}

		private PrizeCounterType GetPrizeType(string prizeString, bool isSkill)
		{
			switch (prizeString)
			{
				case "Shooter":
					return isSkill ? PrizeCounterType.SkillShooter : PrizeCounterType.SkillShooterPart;
				case "Scout":
					return isSkill ? PrizeCounterType.SkillScout : PrizeCounterType.SkillScoutPart;
				case "Hunter":
					return isSkill ? PrizeCounterType.SkillHunter : PrizeCounterType.SkillHunterPart;
				case "Warrior":
					return isSkill ? PrizeCounterType.SkillWarrior : PrizeCounterType.SkillWarriorPart;
				case "Assault":
					return isSkill ? PrizeCounterType.SkillAssault : PrizeCounterType.SkillAssaultPart;
				case "Bruiser":
					return isSkill ? PrizeCounterType.SkillBruiser : PrizeCounterType.SkillBruiserPart;
				default:
					return PrizeCounterType.Any;
			}
		}

		private bool SwitchCurrencyReward(RewardCurrency rewardCurrency)
		{
			switch (rewardCurrency.CurrencyType)
			{
				case CurrencyType.EquipTraitsRemodelToken:
					return ChangeCounterStatePhone(PrizeCounterType.RemodelPart);
				case CurrencyType.ApocalypticEquipToken:
					return ChangeCounterStatePhone(PrizeCounterType.ApoPart);
				case CurrencyType.BulePrintToken:
					return ChangeCounterStatePhone(rewardCurrency.Amount < 50 ? PrizeCounterType.TokenPart : PrizeCounterType.Token);
				default:
					return false;
			}
		}

		private PrizeCounterType GetCurrentPrizeCounterType()
		{
			return NewPhonePopup.Instance.IsWeaponSkillMode ? NewPhonePopup.Instance.GoldRadioWeaponContainer.CurrentRewardTypeSkill : NewPhonePopup.Instance.PhoneWeaponContainer.CurrentRewardType;
		}

		private bool IsFineCompared(EquipPrizeWheelReward reward)
		{
			var _reward = reward.RewardEntries?.RewardsList[0];
			if (_reward is RewardCurrency rewardCurrency)
			{
				var currencyName = rewardCurrency.CurrencyType.ToString();
				return currencyName.Contains("SkillToken") ? ChangeCounterStateGold(currencyName) : SwitchCurrencyReward(rewardCurrency);
			}
			else if (_reward is RewardEquipToken)
			{
				return ChangeCounterStatePhone(PrizeCounterType.Token);
			}
			else if (_reward is RewardRemoldSkill rewardRemoldSkill)
			{
				return ChangeCounterStateGold(rewardRemoldSkill.SpRemoldSkillType);
			}
			return false;
		}
		#endregion
	}
}
