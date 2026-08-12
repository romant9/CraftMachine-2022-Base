using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel.Badge
{
	public class DebugGiveBadgeCommand : ModelCommand
	{
		public int SlotIndex { get; set; }

		public int Rarity { get; set; }

		public BadgeType BadgeType { get; set; }

		public string EffectId { get; set; }

		public int DebugGiveIncrement { get; set; }

		public DebugGiveBadgeCommand()
		{
		}

		public DebugGiveBadgeCommand(int slotIndex, int rarity, BadgeType badgeType, string effectId)
		{
			SlotIndex = slotIndex;
			Rarity = rarity;
			BadgeType = badgeType;
			EffectId = effectId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager obj = manager as TWDModelManager;
			BadgeModel badgeModel = new BadgeModel(++obj.Player.LootManager.CurrentBadgeAnalyticsId, level: obj.Player.Camp.GetBuilding("Residence")?.Level ?? 1, slotIndex: SlotIndex, rarity: Rarity, type: BadgeType, effectId: EffectId, effectRoll: 0)
			{
				IsDebugGive = true,
				DebugGiveIncrement = DebugGiveIncrement,
				BonusId = "Constant"
			};
			BadgeBonusDefinition badgeBonusDefinition = obj.GameEconomyData.GetBadgeBonusDefinition(badgeModel.BonusId);
			Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), badgeBonusDefinition.ConditionClassName);
			if (!string.IsNullOrEmpty(badgeBonusDefinition.ConditionClassName) && type == null)
			{
				manager.Debug.LogError("Failed to instantiate condition class " + badgeBonusDefinition.ConditionClassName);
			}
			List<string> list = new List<string> { "0" };
			badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, list) as BaseBonusCondition) : null);
			badgeModel.BonusParameters = list;
			badgeModel.Initialize();
			badgeModel.SetManager(manager);
			badgeModel.Start();
			obj.Player.Equipment.AddBadge(badgeModel);
			obj.Player.LastCraftedBadge = badgeModel;
			obj.Player.NotifyChange(LootManagerModel.BadgeCreatedEvent);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
