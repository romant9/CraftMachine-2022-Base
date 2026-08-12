using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class BadgeContainerModel : TWDModelObject
	{
		public ModelList<BadgeModel> Badges { get; protected set; }

		public BadgeContainerModel()
		{
			Badges = new ModelList<BadgeModel>();
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override bool IsValid()
		{
			return true;
		}

		public bool HasSetBonus(BadgeType badgeType)
		{
			int num = 0;
			for (int i = 0; i < ((Badges != null) ? Badges.Count : 0); i++)
			{
				if (Badges[i].Type == badgeType)
				{
					num++;
				}
				if (num >= 4)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasAnySetBonus()
		{
			for (int i = 0; i < ((Badges != null) ? Badges.Count : 0); i++)
			{
				if (HasSetBonus(Badges[i].Type))
				{
					return true;
				}
			}
			return false;
		}

		public List<ModelModifier> CreateBadgeModifiers(BadgeModel badgeModel, FixedPoint? bonus)
		{
			BadgeEffectDefinition badgeEffectDefinition = base.gameEconomyData.GetBadgeEffectDefinition(badgeModel.EffectId, badgeModel.Level);
			if (badgeEffectDefinition != null)
			{
				FixedPoint increment = badgeModel.Increment;
				if (bonus.HasValue && bonus > 0L)
				{
					increment += FixedPoint.Max(1L, FixedPoint.Round(increment * (bonus.Value / 100.0)));
				}
				if (HasSetBonus(badgeModel.Type))
				{
					increment += increment * badgeModel.GetBadgeSetBonus();
				}
				TraitDefinition traitDefinition = new TraitDefinition
				{
					Identifier = UpgradeTraitsData.CompileUpgradeTraitIdentifier(badgeEffectDefinition.TraitId, 0, isLocked: false),
					ConstructionParameters = new List<string> { increment.ToString() }
				};
				return new ActorTraitContainerModel().CreateTraitModifiers(traitDefinition, new FixedPoint(1.0), null);
			}
			return null;
		}

		public void SetupForCombat()
		{
		}

		public BadgeModel GetBadge(int slotIndex)
		{
			for (int i = 0; i < ((Badges != null) ? Badges.Count : 0); i++)
			{
				if (Badges[i].SlotIndex == slotIndex)
				{
					return Badges[i];
				}
			}
			return null;
		}

		public void SetBadge(BadgeModel badge)
		{
			BadgeModel badgeModel = null;
			for (int i = 0; i < ((Badges != null) ? Badges.Count : 0); i++)
			{
				if (Badges[i].SlotIndex == badge.SlotIndex)
				{
					badgeModel = Badges[i];
					break;
				}
			}
			if (badgeModel != null)
			{
				Badges.Remove(badgeModel);
			}
			Badges.Add(badge);
			badge.CreateBonusCondition(base.manager.GameEconomyData.GetBadgeBonusDefinition(badge.BonusId));
		}

		public int GetEquippableBadgeSlotCount()
		{
			int num = 0;
			if (base.manager != null)
			{
				for (int i = 0; i < 6; i++)
				{
					if (GetBadge(i) == null && base.manager.Player.Equipment.ContainsBadgeWithSlotIndex(i))
					{
						num++;
					}
				}
			}
			return num;
		}

		public int GetSimilarBadgeCount(BadgeModel badge, BadgeModel excludingBadge = null)
		{
			BadgeEffectDefinition badgeEffectDefinition = base.manager.GameEconomyData.GetBadgeEffectDefinition(badge.EffectId, badge.Level);
			if (badgeEffectDefinition == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < ((Badges != null) ? Badges.Count : 0); i++)
			{
				BadgeEffectDefinition badgeEffectDefinition2 = base.manager.GameEconomyData.GetBadgeEffectDefinition(Badges[i].EffectId, Badges[i].Level);
				if (badgeEffectDefinition2 != null && badgeEffectDefinition2.Category == badgeEffectDefinition.Category && Badges[i] != excludingBadge)
				{
					num++;
				}
			}
			return num;
		}
	}
}
