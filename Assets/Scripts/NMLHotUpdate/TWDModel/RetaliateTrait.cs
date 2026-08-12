using System.Collections.Generic;

namespace TWDModel
{
	public class RetaliateTrait : ActionModifier
	{
		private FixedPoint multiplier = 1.0;

		public const int SortOrder = 5;

		public RetaliateTrait()
		{
		}

		public RetaliateTrait(FixedPoint damageMultiplier)
		{
			multiplier = damageMultiplier;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!actor.CanPerformOOT || actor.dashTraitAttackFlag)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PostDamageAction postDamageAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (postDamageAction.TargetActor == actor && combatModel != null && !combatModel.MissionCompleted)
				{
					if (postDamageAction.DamagerActor == null)
					{
						base.Debug.LogError(" 无效 postDamageAction , DamagerActor 为空");
						return ActionListClearFlag.Keep;
					}
					if (!postDamageAction.DamageAction.BodyShot && postDamageAction.DamagerActor?.SelectedAbility.PushEffect != null)
					{
						return ActionListClearFlag.Keep;
					}
					if (postDamageAction.DamagerActor?.GetWeaponEquipment()?.HasTemporaryTrait("CoupDeGraceActive") == true)
					{
						return ActionListClearFlag.Keep;
					}
					if (postDamageAction.DamageAction.SourceSupport != null)
					{
						return ActionListClearFlag.Keep;
					}
					if (postDamageAction.DamagerActor.IsCamouflaged)
					{
						return ActionListClearFlag.Keep;
					}
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					if (weaponEquipment != null && postDamageAction.DamagerActor != null)
					{
						FixedPoint value = multiplier;
						combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseRetaliateDamage", ref value, actor);
						combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseEquipmentRetaliateDamage", ref value, actor);
						weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, value * 100.0);
						AbilityAction abilityAction = new AbilityAction(actor, weaponEquipment.Ability, postDamageAction.DamagerActor.GridCoordinate, postDamageAction.DamagerActor, OOTType.Retaliation, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
						abilityAction.SetSortOrder(5);
						addedActions.Add(abilityAction);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
