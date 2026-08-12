using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RainbowCatSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			affectedTargets = new List<ActorModel>(2);
			ChargeMeterModel chargeMeter = attachedSurvivor.ChargeMeter;
			if (chargeMeter != null && chargeMeter.ChargeLevel < chargeMeter.MaxLevel)
			{
				attachedSurvivor.AddChargePoints((int)supportModel.GetParameter(0));
				affectedTargets.Add(attachedSurvivor);
			}
			FixedPoint successProbability = supportModel.GetParameter(1) * 0.009999999776482582;
			FixedPoint value = 0.0;
			supportModel.manager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value, attachedSurvivor);
			if (supportModel.manager.Player.RollDice(RollDiceType.GainChargePoint, successProbability, value) != PlayerRandomChanceResult.Failed)
			{
				List<ActorModel> list = new List<ActorModel>(2);
				foreach (ActorModel survivor in supportModel.manager.CombatModel.Survivors)
				{
					ChargeMeterModel chargeMeter2 = survivor.ChargeMeter;
					if (survivor != attachedSurvivor && !survivor.IsDead && chargeMeter2.ChargeLevel < chargeMeter2.MaxLevel)
					{
						list.Add(survivor);
					}
				}
				if (list.Count > 0)
				{
					ActorModel actorModel2 = list[supportModel.manager.Player.PlayerRandom.Next(list.Count)];
					actorModel2.AddChargePoints(1);
					affectedTargets.Add(actorModel2);
				}
			}
			return new ModelAction[0];
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			ModelList<ActorModel> survivors = attachedSurvivor.manager.CombatModel.Survivors;
			if (survivors != null)
			{
				foreach (ActorModel item in survivors)
				{
					ChargeMeterModel chargeMeter = item.ChargeMeter;
					if (chargeMeter != null && chargeMeter.ChargeLevel < chargeMeter.MaxLevel)
					{
						return true;
					}
				}
			}
			return false;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			if (CanExecute(supportModel, attachedSurvivor, target))
			{
				return new SurvivorModel[1] { attachedSurvivor };
			}
			return new ActorModel[0];
		}
	}
}
