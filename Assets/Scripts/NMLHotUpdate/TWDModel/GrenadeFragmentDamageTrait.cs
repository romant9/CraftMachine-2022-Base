using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class GrenadeFragmentDamageTrait : ActionModifier
	{
		private int GrenadeFragmentNums;

		private FixedPoint AdditionDamagePercentage { get; set; }

		private FixedPoint AdditionAddDamagePercentage { get; set; }

		private FixedPoint GrenadeFragmentPercentage { get; set; }

		public GrenadeFragmentDamageTrait(FixedPoint additionDamagePercentage, FixedPoint additionAddDamagePercentage, FixedPoint grenadeFragmentPercentage)
		{
			AdditionDamagePercentage = additionDamagePercentage;
			AdditionAddDamagePercentage = additionAddDamagePercentage;
			GrenadeFragmentPercentage = grenadeFragmentPercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction { Actor: not null } abilityAction && actor == abilityAction.Actor && !abilityAction.Ability.IsConsumableAbility)
			{
				GrenadeFragmentNums = 0;
				List<ActorModel> listOfActorsToBeTargetted = actor.manager.CombatModel.AbilityManager.GetListOfActorsToBeTargetted(abilityAction.Ability, actor, actor.GridCoordinate, abilityAction.TargetCell);
				GrenadeFragmentNums = listOfActorsToBeTargetted.Count((ActorModel x) => x.BeGrenadeFragmentDamagedByFaction(actor.Faction));
			}
			if (action is PostAbilityExecuteAction { DamagerActor: not null } postAbilityExecuteAction && actor == postAbilityExecuteAction.DamagerActor)
			{
				GrenadeFragmentNums = 0;
			}
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor != null && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.DamageType != DamageType.GrenadeFragmentDamage && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				if (postDamageAction.TargetActor.BeGrenadeFragmentDamagedByFaction(postDamageAction.DamagerActor.Faction))
				{
					FireExplosion(postDamageAction.DamagerActor, postDamageAction.TargetActor);
				}
				else
				{
					CreateGrenadeFragmentDamage(postDamageAction.DamagerActor, postDamageAction.TargetActor);
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void CreateGrenadeFragmentDamage(ActorModel source, ActorModel target)
		{
			TWDModelManager tWDModelManager = source.manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, source);
			if (tWDModelManager.Player.RollDice(RollDiceType.GrenadeFragmentDamage, GrenadeFragmentPercentage, value) != PlayerRandomChanceResult.Failed)
			{
				GrenadeFragmentDamageRelationsManager grenadeFragmentDamageRelationsManager = combatModel.GetModel<GrenadeFragmentDamageRelationsManager>();
				if (grenadeFragmentDamageRelationsManager == null)
				{
					grenadeFragmentDamageRelationsManager = new GrenadeFragmentDamageRelationsManager();
					grenadeFragmentDamageRelationsManager.SetManager(source.manager);
					combatModel.AddModel(grenadeFragmentDamageRelationsManager);
				}
				GrenadeFragmentDamageRelation newRelation = new GrenadeFragmentDamageRelation(source, target, source.Faction, -1, AdditionDamagePercentage, AdditionAddDamagePercentage);
				grenadeFragmentDamageRelationsManager.AddRelation(newRelation);
			}
		}

		private void FireExplosion(ActorModel source, ActorModel target)
		{
			CombatModel combatModel = source.manager.CombatModel;
			if (combatModel.TurnManager.ActiveActor != source || !(source is SurvivorModel survivorModel))
			{
				return;
			}
			GrenadeFragmentDamageRelationsManager model = combatModel.GetModel<GrenadeFragmentDamageRelationsManager>();
			if (model == null)
			{
				return;
			}
			GrenadeFragmentDamageRelation grenadeFragmentDamageRelation = model.ExistedGrenadeFragmentDamageRelationRelations.Find((GrenadeFragmentDamageRelation x) => x.TargetActor == target && x.FoundingFaction == source.Faction);
			if (grenadeFragmentDamageRelation != null)
			{
				FixedPoint additionDamagePercentage = grenadeFragmentDamageRelation.AdditionDamagePercentage;
				if (IsAddAdditionAddDamagePercentage(source, target))
				{
					additionDamagePercentage += grenadeFragmentDamageRelation.AdditionAddDamagePercentage;
				}
				FixedPoint fixedPoint = survivorModel.GetDamageForPreferredWeapon() * additionDamagePercentage;
				for (int num = 0; num < GrenadeFragmentNums; num++)
				{
					CombatHelpers.ExecuteDamage(combatModel, null, target, (int)fixedPoint, 0, DamageType.GrenadeFragmentDamage, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				}
				model.RemoveRelation(combatModel, grenadeFragmentDamageRelation);
			}
		}

		private bool IsAddAdditionAddDamagePercentage(ActorModel source, ActorModel target)
		{
			if (base.manager.CombatModel.Models.OfType<TrapFlameArea>().ToList().Exists((TrapFlameArea x) => x.EffectiveAreaGridCoordinate == target.GridCoordinate))
			{
				return true;
			}
			foreach (TWDModelObject model in base.manager.CombatModel.Models)
			{
				if (model is SufferArea sufferArea && sufferArea.Faction == source.Faction && sufferArea.IsInArea(target.GridCoordinate))
				{
					return true;
				}
			}
			if (base.manager.CombatModel.Models.OfType<PitfallArea>().ToList().Exists((PitfallArea x) => x.PitfallAreaGrids.Contains(target.GridCoordinate)))
			{
				return true;
			}
			return false;
		}
	}
}
