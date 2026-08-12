using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class DashTrait : ActionModifier
	{
		private int canMarkAmount;

		private int Turns;

		private bool isTriger = true;

		public DashTrait(int CanMarkAmount, int turns)
		{
			canMarkAmount = CanMarkAmount;
			Turns = turns;
		}

		private List<ActorModel> GetSortedActors(ActorModel actorModel, in List<ActorModel> targets)
		{
			targets.StableSort(delegate(ActorModel actor1, ActorModel actor2)
			{
				int num = actorModel.GridCoordinate.ChebyshevDistance(actor1.GridCoordinate);
				int num2 = actorModel.GridCoordinate.ChebyshevDistance(actor2.GridCoordinate);
				if (num == num2)
				{
					if (actor1.Definition.IsSpecial && !actor2.Definition.IsSpecial)
					{
						return -1;
					}
					if (!actor1.Definition.IsSpecial && actor2.Definition.IsSpecial)
					{
						return 1;
					}
				}
				FixedVec2 fixedVec = actorModel.GridCoordinate.ToVector2() - actor1.GridCoordinate.ToVector2();
				FixedVec2 fixedVec2 = actorModel.GridCoordinate.ToVector2() - actor2.GridCoordinate.ToVector2();
				return (fixedVec.SqrMagnitude >= fixedVec2.SqrMagnitude) ? 1 : (-1);
			});
			return targets;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PreAttackAction { Interrupted: not false } preAttackAction && preAttackAction.TargetActor == actor)
			{
				isTriger = false;
			}
			if (action is OverwatchAttackAction { Interrupted: not false } overwatchAttackAction && overwatchAttackAction.TargetActor == actor)
			{
				isTriger = false;
			}
			if (action is AbilityAction abilityAction && actor != null && abilityAction.Actor == actor)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				ActorModel occupier = combatModel.GetOccupier(abilityAction.TargetCell);
				if (abilityAction.Ability.IsChargeAttack && !abilityAction.Ability.IsConsumableAbility && occupier != null && !occupier.IsDead)
				{
					if (actor.Faction == Faction.Survivor)
					{
						if (combatModel.DashSurvivalFlagActor != null)
						{
							combatModel.DashSurvivalFlagActor.SurvivalDashFlagTurns = 0;
							combatModel.DashSurvivalFlagActor.NotifyChange("SurvivalDashFlagUpdate");
						}
						combatModel.DashSurvivalFlagActor = occupier;
						combatModel.DashSurvivalFlagActor.SurvivalDashFlagTurns = Turns;
						combatModel.DashSurvivalFlagActor.NotifyChange("SurvivalDashFlagUpdate");
					}
					else if (actor.Faction == Faction.Raider)
					{
						if (combatModel.DashRaiderFlagActor != null)
						{
							combatModel.DashRaiderFlagActor.RaiderDashFlagTurns = 0;
							combatModel.DashRaiderFlagActor.NotifyChange("RaiderDashFlagUpdate");
						}
						combatModel.DashRaiderFlagActor = occupier;
						combatModel.DashRaiderFlagActor.RaiderDashFlagTurns = Turns;
						combatModel.DashRaiderFlagActor.NotifyChange("RaiderDashFlagUpdate");
					}
				}
			}
			if (action is PostMoveSuccessAction postMoveSuccessAction && postMoveSuccessAction.Actor == actor)
			{
				if (!isTriger)
				{
					isTriger = true;
					return ActionListClearFlag.Keep;
				}
				if (actor.SelectedEquipment == actor.GetConsumableEquipment() || actor.SelectedEquipment.Definition.Category == EquipmentCategory.Utility)
				{
					return ActionListClearFlag.Keep;
				}
				if (actor.IsInvisible || actor.IsCamouflaged)
				{
					return ActionListClearFlag.Keep;
				}
				if (base.manager.Player.GetAttackTargetMissionModel() is MapMissionModel { IsInWeeklySurvival: not false } && !actor.dashTraitValidFlag)
				{
					actor.dashTraitValidFlag = true;
					return ActionListClearFlag.Keep;
				}
				CombatModel combatModel2 = actor.manager.CombatModel;
				bool flag = false;
				if (actor.Faction == Faction.Survivor)
				{
					if (combatModel2.DashSurvivalFlagActor != null && combatModel2.DashSurvivalFlagActor.IsSurvivalDashFlag && !combatModel2.DashSurvivalFlagActor.IsDead)
					{
						flag = true;
						EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
						if (weaponEquipment != null)
						{
							actor.dashTraitAttackFlag = true;
							for (int i = 0; i < canMarkAmount; i++)
							{
								if (actor.SelectedAbility.CanAbilityBePerformedOnGridCell(base.manager.CombatModel, actor, actor.GridCoordinate, combatModel2.DashSurvivalFlagActor.GridCoordinate) == AbilityResult.Success)
								{
									AbilityAction item = new AbilityAction(actor, weaponEquipment.Ability, combatModel2.DashSurvivalFlagActor.GridCoordinate, combatModel2.DashSurvivalFlagActor, OOTType.AutoAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
									addedActions.Add(item);
								}
							}
						}
					}
				}
				else if (actor.Faction == Faction.Raider && combatModel2.DashRaiderFlagActor != null && combatModel2.DashRaiderFlagActor.IsRaiderDashFlag && !combatModel2.DashSurvivalFlagActor.IsDead)
				{
					flag = true;
					EquipmentItemModel weaponEquipment2 = actor.GetWeaponEquipment();
					if (weaponEquipment2 != null)
					{
						actor.dashTraitAttackFlag = true;
						for (int j = 0; j < canMarkAmount; j++)
						{
							if (actor.SelectedAbility.CanAbilityBePerformedOnGridCell(base.manager.CombatModel, actor, actor.GridCoordinate, combatModel2.DashRaiderFlagActor.GridCoordinate) == AbilityResult.Success)
							{
								AbilityAction item2 = new AbilityAction(actor, weaponEquipment2.Ability, combatModel2.DashRaiderFlagActor.GridCoordinate, combatModel2.DashRaiderFlagActor, OOTType.AutoAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
								addedActions.Add(item2);
							}
						}
					}
				}
				if (!flag)
				{
					List<ActorModel> targets = new List<ActorModel>();
					foreach (ActorModel allActor in combatModel2.GetAllActors())
					{
						if (allActor.IsEnemy(actor) && !allActor.IsEnvironmental && !allActor.IsDead)
						{
							targets.Add(allActor);
						}
					}
					if (targets.Count == 0)
					{
						return ActionListClearFlag.Keep;
					}
					GetSortedActors(actor, in targets);
					for (int k = 0; k < Math.Min(canMarkAmount, targets.Count); k++)
					{
						ActorModel actorModel = targets[k];
						if (actorModel != null)
						{
							EquipmentItemModel weaponEquipment3 = actor.GetWeaponEquipment();
							if (weaponEquipment3 != null && actor.SelectedAbility.CanAbilityBePerformedOnGridCell(base.manager.CombatModel, actor, actor.GridCoordinate, actorModel.GridCoordinate) == AbilityResult.Success)
							{
								actor.dashTraitAttackFlag = true;
								AbilityAction item3 = new AbilityAction(actor, weaponEquipment3.Ability, actorModel.GridCoordinate, actorModel, OOTType.AutoAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
								addedActions.Add(item3);
							}
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
