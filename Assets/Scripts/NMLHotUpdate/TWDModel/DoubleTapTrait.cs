using System.Collections.Generic;

namespace TWDModel
{
	public class DoubleTapTrait : ActionModifier
	{
		public const int SortOrder = 7;

		public DoubleTapTrait()
		{
		}

		public DoubleTapTrait(FixedPoint chance)
		{
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.IsDead || actor.AIController.IsActorIncapacitated || actor.ExclusiveTimedEffect != null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PostDamageAction postDamageAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (postDamageAction.TargetActor != null && postDamageAction.TargetActor.IsDead && combatModel != null && !combatModel.MissionCompleted && actor == postDamageAction.DamagerActor)
				{
					FixedPoint value = 0.0;
					if (combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseDoubleTapChance", ref value, actor))
					{
						FixedPoint value2 = 0.0;
						combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
						if (combatModel.manager.Player.RollDice(RollDiceType.FreeAttack, value, value2) != PlayerRandomChanceResult.Failed)
						{
							List<ActorModel> list = new List<ActorModel>();
							EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
							for (int i = 0; i < ((combatModel.AllActors != null) ? combatModel.AllActors.Count : 0); i++)
							{
								ActorModel actorModel = combatModel.AllActors[i];
								if (actorModel != null && actor.IsEnemy(actorModel) && weaponEquipment != null && weaponEquipment.Ability.CanAbilityBeTargetedOnGridCell(combatModel, actor, actor.GridCoordinate, actorModel.GridCoordinate))
								{
									list.Add(actorModel);
								}
							}
							ActorModel actorModel2 = ((list.Count > 0) ? combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: false) : null);
							if (weaponEquipment != null && actorModel2 != null)
							{
								GenericAbilityAction genericAbilityAction = new GenericAbilityAction(actor, weaponEquipment.Ability, actorModel2.GridCoordinate, "ActorNotification.DoubleTap", actorModel2);
								addedActions.Add(genericAbilityAction);
								genericAbilityAction.SetSortOrder(7);
							}
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
