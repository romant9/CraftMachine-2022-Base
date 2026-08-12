using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class HwachaSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			CombatModel combatModel = attachedSurvivor.manager.CombatModel;
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, attachedSurvivor);
			FixedPoint successProbability = supportModel.GetParameter(2) / 100.0;
			LinkedList<ModelAction> linkedList = new LinkedList<ModelAction>();
			FixedPoint parameter = supportModel.GetParameter(0);
			int damage = (int)(attachedSurvivor.GetDamageForPreferredWeapon() * supportModel.GetParameter(1) / 100.0);
			ModelRandom playerRandom = supportModel.manager.Player.PlayerRandom;
			List<ActorModel> targetsInternal = GetTargetsInternal(supportModel, target);
			affectedTargets = new LinkedList<ActorModel>();
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

		private List<ActorModel> GetTargetsInternal(SupportModel supportModel, GridCoordinate target)
		{
			List<ActorModel> list = new List<ActorModel>();
			CombatModel combatModel = supportModel.manager.CombatModel;
			ProcessEnemies(combatModel.Walkers, supportModel, target, list);
			ProcessEnemies(combatModel.Raiders, supportModel, target, list);
			return list;
		}

		private void ProcessEnemies(IEnumerable<ActorModel> actors, SupportModel supportModel, GridCoordinate target, List<ActorModel> targets)
		{
			if (actors == null)
			{
				return;
			}
			FixedPoint parameter = supportModel.GetParameter(4);
			FixedPoint fixedPoint = parameter * parameter;
			CombatModel combatModel = supportModel.manager.CombatModel;
			foreach (ActorModel actor in actors)
			{
				if (!actor.IsDead && !actor.IsEnvironmental && target.SquaredDistanceTo(actor.GridCoordinate) <= fixedPoint && combatModel.IsGridCellVisibleByAnySurvivor(actor.GridCoordinate))
				{
					targets.Add(actor);
				}
			}
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetTargetsInternal(supportModel, target).Count > 0;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetTargetsInternal(supportModel, target);
		}
	}
}
