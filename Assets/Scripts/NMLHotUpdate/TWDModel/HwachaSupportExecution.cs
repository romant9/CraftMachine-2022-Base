using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class HwachaSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			LinkedList<ModelAction> linkedList = new LinkedList<ModelAction>();
			affectedTargets = new LinkedList<ActorModel>();
			CombatModel combatModel = attachedSurvivor?.manager?.CombatModel;
			if (supportModel?.definition == null || supportModel.manager?.Player == null || attachedSurvivor?.manager?.Player == null || combatModel?.AbilityManager == null)
			{
				return linkedList;
			}
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, attachedSurvivor);
			FixedPoint successProbability = supportModel.GetParameter(2) / 100.0;
			FixedPoint parameter = supportModel.GetParameter(0);
			int damage = (int)(attachedSurvivor.GetDamageForPreferredWeapon() * supportModel.GetParameter(1) / 100.0);
			ModelRandom playerRandom = supportModel.manager.Player.PlayerRandom;
			List<ActorModel> targetsInternal = GetTargetsInternal(supportModel, attachedSurvivor, target);
			PlayerModel player = attachedSurvivor.manager.Player;
			for (int i = 0; i < parameter; i++)
			{
				if (targetsInternal.Count <= 0)
				{
					break;
				}
				ActorModel randomElement = playerRandom.GetRandomElement(targetsInternal, remove: true);
				CombatHelpers.ExecuteDamage(combatModel, attachedSurvivor, randomElement, damage, 0, DamageType.Ranged, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, supportModel, noChargeGain: true);
				if (player.RollDice(RollDiceType.Burn, successProbability, value) != PlayerRandomChanceResult.Failed)
				{
					linkedList.AddLast(new BurningOutAction(attachedSurvivor, randomElement, onRedHealthBar: false, supportModel, () => damage));
				}
				affectedTargets.Add(randomElement);
			}
			return linkedList;
		}

		private List<ActorModel> GetTargetsInternal(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			List<ActorModel> list = new List<ActorModel>();
			CombatModel combatModel = supportModel?.manager?.CombatModel;
			if (combatModel == null || combatModel.Grid == null || supportModel.definition == null || attachedSurvivor == null || !target.IsValid || !combatModel.Grid.IsCoordinateValid(target))
			{
				return list;
			}
			FixedPoint fixedPoint = supportModel.GetParameter(4);
			ActorModel actorModel = combatModel.GetOccupier(target);
			if (actorModel == null || combatModel.AbilityManager == null || !actorModel.HasDamageAreaBlock || actorModel.IsDead || actorModel.IsEnvironmental || !actorModel.IsEnemy(attachedSurvivor) || ((combatModel.Walkers == null || !combatModel.Walkers.Contains(actorModel)) && (combatModel.Raiders == null || !combatModel.Raiders.Contains(actorModel))))
			{
				actorModel = null;
			}
			if (actorModel != null)
			{
				fixedPoint = combatModel.AbilityManager.GetDamageAreaBlockEffectiveSupportRadius(target, (int)fixedPoint);
			}
			ProcessEnemies(combatModel.Walkers, combatModel, target, fixedPoint, actorModel, list);
			ProcessEnemies(combatModel.Raiders, combatModel, target, fixedPoint, actorModel, list);
			return list;
		}

		private void ProcessEnemies(IEnumerable<ActorModel> actors, CombatModel combatModel, GridCoordinate target, FixedPoint radius, ActorModel damageAreaBlockMainTarget, List<ActorModel> targets)
		{
			if (actors == null)
			{
				return;
			}
			FixedPoint fixedPoint = radius * radius;
			foreach (ActorModel actor in actors)
			{
				if (actor != null && !actor.IsDead && !actor.IsEnvironmental)
				{
					GridCoordinate other = ((actor == damageAreaBlockMainTarget) ? target : actor.GridCoordinate);
					if (target.SquaredDistanceTo(other) <= fixedPoint && combatModel.IsGridCellVisibleByAnySurvivor(actor.GridCoordinate))
					{
						targets.Add(actor);
					}
				}
			}
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetTargetsInternal(supportModel, attachedSurvivor, target).Count > 0;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetTargetsInternal(supportModel, attachedSurvivor, target);
		}
	}
}
