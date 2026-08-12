using System.Collections.Generic;

namespace TWDModel
{
	public class PastaSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			attachedSurvivor.PastaTurns = (int)supportModel.GetParameter(2);
			attachedSurvivor.PastaCurrentTurn = true;
			long duration = (long)supportModel.GetParameter(2);
			attachedSurvivor.AddTemporaryTrait("PastaSupportActive", default(FixedPoint), null, duration);
			LinkedList<ModelAction> result = new LinkedList<ModelAction>();
			affectedTargets = new SurvivorModel[1] { attachedSurvivor };
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
			return new List<ActorModel> { attachedSurvivor };
		}
	}
}
