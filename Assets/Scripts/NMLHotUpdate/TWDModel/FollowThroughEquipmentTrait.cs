using System.Collections.Generic;

namespace TWDModel
{
	public class FollowThroughEquipmentTrait : ActionModifier
	{
		private readonly FixedPoint multiplier;

		private ActorModel attackTarget;

		public FollowThroughEquipmentTrait(FixedPoint mult)
		{
			multiplier = mult;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction)
			{
				if (postDamageAction.DamagerActor == actor && postDamageAction.DamageAction.DamageType == DamageType.Melee && postDamageAction.TargetActor.IsDead && postDamageAction.TargetActor.Faction != Faction.Environmental && !postDamageAction.DamagerActor.FollowThroughTriggeredInAttack && attackTarget == postDamageAction.TargetActor && base.manager.CombatModel.AbilityManager.AbilityUnderApplication != null && postDamageAction.DamageAction.SourceSupport == null)
				{
					attackTarget = null;
					FixedPoint value = 0.0;
					AbilityManagerModel abilityManager = base.manager.CombatModel.AbilityManager;
					List<ActorModel> listOfActorsToBeTargetted = abilityManager.GetListOfActorsToBeTargetted(abilityManager.AbilityUnderApplication, postDamageAction.DamagerActor, postDamageAction.DamagerActor.GridCoordinate, postDamageAction.TargetActor.GridCoordinate);
					abilityManager.VisitParameter("ExtendProbability", ref value, actor);
					CombatHelpers.FollowThrough(postDamageAction.DamageAction, value, multiplier, addedActions, requireSourceActorNeighbour: false, null, skipAbilityTargetable: true, listOfActorsToBeTargetted)?.NotifyChange("WeaponAbilityVisited", postDamageAction.DamagerActor?.GetWeaponEquipment()?.EquipmentDefinitionIdentifier);
				}
			}
			else if (action is FireWeaponAction fireWeaponAction && fireWeaponAction.SourceActor == actor)
			{
				attackTarget = base.manager.CombatModel.Occupiers[fireWeaponAction.TargetGridCoordinate];
				if (fireWeaponAction.WeaponAbility.IsConsumableAbility)
				{
					ActorModel actorModel = attackTarget;
					if (actorModel == null || actorModel.IsEnvironmental)
					{
						attackTarget = null;
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
