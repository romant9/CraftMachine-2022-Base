using System.Collections.Generic;

namespace TWDModel
{
	public class NotebookSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			LinkedList<ModelAction> result = new LinkedList<ModelAction>();
			affectedTargets = new LinkedList<ActorModel>();
			if (supportModel == null || supportModel.manager == null || supportModel.manager.CombatModel == null)
			{
				return result;
			}
			if (supportModel.manager.CombatModel.IsEndlessBattleMission)
			{
				supportModel.manager.CombatModel.EndlessModeCombatModel.CurrentWaveDuration += (int)supportModel.GetParameter(0);
			}
			else
			{
				ThreatMeterModel threatMeter = supportModel.manager.CombatModel.ThreatMeter;
				if (threatMeter != null)
				{
					int num = (int)supportModel.GetParameter(0);
					int turnCounter = threatMeter.TurnCounter;
					if (turnCounter <= 0)
					{
						threatMeter.SetTurnCount(threatMeter.InitialTurnCountToWave + 1 + num);
						threatMeter.ResetThreatSpawnPoints();
					}
					else
					{
						threatMeter.SetTurnCount(turnCounter + num);
					}
				}
			}
			attachedSurvivor.AddChargePoints((int)supportModel.GetParameter(1));
			supportModel.manager.CombatModel.NotifyChange("FlushthreatTurn");
			foreach (ActorModel factionActor in supportModel.manager.CombatModel.GetFactionActors(attachedSurvivor.Faction))
			{
				if (factionActor.ModelId != attachedSurvivor.ModelId)
				{
					FixedPoint fixedPoint = supportModel.GetParameter(2) * 0.01;
					if (fixedPoint > 0.0 && supportModel.manager.Player.RollDice(RollDiceType.GainChargePoint, fixedPoint) != PlayerRandomChanceResult.Failed && factionActor.ChargeMeter != null)
					{
						factionActor.AddChargePoints((int)supportModel.GetParameter(3));
						affectedTargets.Add(factionActor);
					}
				}
			}
			return result;
		}

		private bool IsViable(ActorModel actor, CombatModel combatModel)
		{
			if (!actor.IsDead && combatModel.IsGridCellVisibleByAnySurvivor(actor.GridCoordinate))
			{
				return !actor.IsEnvironmental;
			}
			return false;
		}

		private bool HasAnyViableTargets(IEnumerable<ActorModel> possibleTargets, CombatModel combatModel)
		{
			if (possibleTargets == null)
			{
				return false;
			}
			foreach (ActorModel possibleTarget in possibleTargets)
			{
				if (IsViable(possibleTarget, combatModel))
				{
					return true;
				}
			}
			return false;
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return true;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			List<ActorModel> list = supportModel?.manager?.CombatModel?.GetFactionActors(attachedSurvivor.Faction);
			if (list == null)
			{
				return new List<ActorModel>();
			}
			return list;
		}
	}
}
