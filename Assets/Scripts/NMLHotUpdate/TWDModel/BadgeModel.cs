using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TWDModel
{
	public class BadgeModel : TWDModelObject
	{
		public const int NumberOfSlots = 6;

		public int RerollsSlot;

		public int RerollsSet;

		public int RerollsBonus;

		public List<int> HistorySlots;

		public List<BadgeType> HistorySet;

		public List<(string BonusId, List<string> BonusParameters)> HistoryBonus;

		public int Rarity { get; private set; }

		public int SlotIndex { get; private set; }

		public BadgeType Type { get; private set; }

		public string EffectId { get; private set; }

		public int EffectRoll { get; private set; }

		public int AnalyticsId { get; private set; }

		public string BonusId { get; set; }

		public List<string> BonusParameters { get; set; }

		public bool IsDebugGive { get; set; }

		public FixedPoint DebugGiveIncrement { get; set; }

		[JsonIgnore]
		public BonusCondition BonusCondition { get; set; }

		public int Level { get; set; }

		[JsonIgnore]
		public FixedPoint Increment
		{
			get
			{
				if (IsDebugGive)
				{
					return DebugGiveIncrement;
				}
				List<int> strengthForRarity = base.manager.GameEconomyData.GetBadgeEffectDefinition(EffectId, Level).GetStrengthForRarity(Rarity);
				return Math.Round((float)strengthForRarity[0] + (float)(strengthForRarity[1] - strengthForRarity[0]) * ((float)EffectRoll / 100f));
			}
		}

		public string GenerateName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Type);
			stringBuilder.Append("_");
			stringBuilder.Append(Rarity);
			stringBuilder.Append("_");
			stringBuilder.Append(EffectId);
			stringBuilder.Append("_");
			stringBuilder.Append(SlotIndex);
			stringBuilder.Append("_");
			stringBuilder.Append(EffectRoll);
			stringBuilder.Append("_");
			stringBuilder.Append(BonusId);
			foreach (string bonusParameter in BonusParameters)
			{
				stringBuilder.Append("_");
				stringBuilder.Append(bonusParameter);
			}
			return stringBuilder.ToString();
		}

		public BadgeModel()
		{
		}

		public BadgeModel(int analyticsId, int slotIndex, int rarity, BadgeType type, string effectId, int effectRoll, int level)
		{
			AnalyticsId = analyticsId;
			SlotIndex = slotIndex;
			Rarity = rarity;
			Type = type;
			EffectId = effectId;
			EffectRoll = effectRoll;
			Level = level;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void CreateBonusCondition(BadgeBonusDefinition definition)
		{
			if (definition != null)
			{
				Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), definition.ConditionClassName);
				BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, BonusParameters) as BaseBonusCondition) : null);
			}
		}

		public Cashier GetScrapCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.ReclaimBadge);
			BuildingUpgradeLevel buildingUpgradeLevel = base.manager.GameEconomyData.GetBuildingUpgradeLevel("Council", base.manager.Player.CouncilLevel);
			if (buildingUpgradeLevel == null)
			{
				base.Debug.LogWarning("Missing council update level info for " + base.manager.Player.CouncilLevel);
				return null;
			}
			if (buildingUpgradeLevel.BadgeScrapXP > 0)
			{
				cashierItem = new CashierItem(PurchaseType.CraftBadge);
				cashierItem.SetCost(CurrencyType.SurvivalPoints, buildingUpgradeLevel.BadgeScrapXP);
				cashier.AddItem(cashierItem);
			}
			return cashier;
		}

		public FixedPoint GetBadgeSetBonus()
		{
			return (float)base.manager.GameEconomyData.ConfigData.BadgeSetBonus / 100f;
		}

		public int GetBadgeRerolls(BadgeReroll reroll)
		{
			return reroll switch
			{
				BadgeReroll.Slot => RerollsSlot,
				BadgeReroll.Set => RerollsSet,
				BadgeReroll.Bonus => RerollsBonus,
				_ => -1,
			};
		}

		public bool BonusHistoryContain(BadgeModel badgeRerolled)
		{
			for (int i = 0; i < HistoryBonus.Count; i++)
			{
				if (BonusesAreEqual(HistoryBonus[i].BonusId, HistoryBonus[i].BonusParameters, badgeRerolled))
				{
					return true;
				}
			}
			return false;
		}

		public void AddBonusToHistory()
		{
			if (HistoryBonus == null)
			{
				HistoryBonus = new List<(string, List<string>)>();
			}
			HistoryBonus.Add((BonusId, BonusParameters));
			if (HistoryBonus.Count > 10)
			{
				HistoryBonus.RemoveAt(0);
			}
		}

		private bool BonusesAreEqual(string bonusId, List<string> bonusParameters, BadgeModel badge2)
		{
			if (bonusId != badge2.BonusId || bonusParameters.Count != badge2.BonusParameters.Count)
			{
				return false;
			}
			for (int i = 0; i < bonusParameters.Count; i++)
			{
				if (bonusParameters[i] != badge2.BonusParameters[i])
				{
					return false;
				}
			}
			return true;
		}


		#region myparams
		[JsonIgnore]
		public int Strength { get; set; }

		[JsonIgnore]
		public bool IsFavorite { get; set; }
		#endregion

		#region mycode
		public void AddSlotToHistory(int num)
		{
			HistorySlots ??= new List<int>();
			if (HistorySlots.Count == 5)
			{
				HistorySlots.Clear();
			}
			HistorySlots.Add(num);
		}

		public void AddSetToHistory(BadgeType badgeType)
		{
			HistorySet ??= new List<BadgeType>();
			if (HistorySet.Count == 4)
			{
				HistorySet.Clear();
			}
			HistorySet.Add(badgeType);
		}
		#endregion
	}
}
