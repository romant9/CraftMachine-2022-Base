using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class VoltShockTrait : ActionModifier
	{
		private FixedPoint VoltShockPercentage;

		private int VoltShockTurns;

		private int VoldShockAsElectronChargeLayer;

		public VoltShockTrait(FixedPoint voltShockPercentage, int voltShockTurns, int voldShockAsElectronChargeLayer)
		{
			VoltShockPercentage = voltShockPercentage;
			VoltShockTurns = voltShockTurns;
			VoldShockAsElectronChargeLayer = voldShockAsElectronChargeLayer;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityBeforeRemoveActiveTraitAction { AbilityAction: var abilityAction })
			{
				if (abilityAction == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (abilityAction.Actor != null && actor == abilityAction.Actor && !abilityAction.Ability.IsConsumableAbility)
				{
					List<ActorModel> listOfActorsToBeTargetted = actor.manager.CombatModel.AbilityManager.GetListOfActorsToBeTargetted(abilityAction.Ability, abilityAction.Actor, abilityAction.Actor.GridCoordinate, abilityAction.TargetCell);
					if (listOfActorsToBeTargetted == null || listOfActorsToBeTargetted.Count == 0)
					{
						return ActionListClearFlag.Keep;
					}
					FixedPoint value = 0.0;
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					if (base.manager.Player.RollDice(RollDiceType.VoltShock, listOfActorsToBeTargetted.Count * VoltShockPercentage, value) == PlayerRandomChanceResult.Failed)
					{
						return ActionListClearFlag.Keep;
					}
					foreach (ActorModel item in listOfActorsToBeTargetted)
					{
						ClearElectronChargeAndElectricShockTarget(actor, item, addedActions);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void ClearElectronChargeAndElectricShockTarget(ActorModel source, ActorModel target, List<ModelAction> addedActions)
		{
			if (target.IsDisoriented || target.IsDisorientedLock)
			{
				return;
			}
			CombatModel combatModel = source.manager.CombatModel;
			ElectronChargeRelationManager model = combatModel.GetModel<ElectronChargeRelationManager>();
			if (model == null)
			{
				return;
			}
			foreach (ElectronChargeRelation item in model.ExistedElectronChargeRelations.Where((ElectronChargeRelation x) => x.TargetActor == target).ToList())
			{
				combatModel.RemoveModel(item);
			}
			target.NotifyChange("ActorElectronChargeUpdateEvent");
			addedActions.Add(new ElectricShockAction(source, target, VoltShockTurns, VoldShockAsElectronChargeLayer));
		}
	}
}
