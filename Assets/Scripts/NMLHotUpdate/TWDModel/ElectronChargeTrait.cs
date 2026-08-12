using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class ElectronChargeTrait : ActionModifier
	{
		private FixedPoint MainTargetGainElectronPercentage;

		private int ElectronTurns;

		private FixedPoint SecondaryTargetGainElectronPercentage;

		private int SecondaryTargetElectronTurns;

		private int MaxLayer;

		private int ElectricShockTurns;

		public ElectronChargeTrait(FixedPoint mainTargetGainElectronPercentage, int electronTurns, FixedPoint secondaryTargetGainElectronPercentage, int secondaryTargetElectronTurns, int maxLayer, int electricShockTurns)
		{
			MainTargetGainElectronPercentage = mainTargetGainElectronPercentage;
			ElectronTurns = electronTurns;
			SecondaryTargetGainElectronPercentage = secondaryTargetGainElectronPercentage;
			SecondaryTargetElectronTurns = secondaryTargetElectronTurns;
			MaxLayer = maxLayer;
			ElectricShockTurns = electricShockTurns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				FixedPoint fixedPoint = 0.0;
				int num = 0;
				if (postDamageAction.DamageAction.IsMainTarget)
				{
					fixedPoint = MainTargetGainElectronPercentage;
					num = ElectronTurns;
				}
				else
				{
					fixedPoint = SecondaryTargetGainElectronPercentage;
					num = SecondaryTargetElectronTurns;
				}
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.ElectronCharge, fixedPoint, value) == PlayerRandomChanceResult.Failed)
				{
					return ActionListClearFlag.Keep;
				}
				CreateRelation(actor, postDamageAction.TargetActor, num, addedActions);
			}
			return ActionListClearFlag.Keep;
		}

		private void CreateRelation(ActorModel source, ActorModel target, int electronTurns, List<ModelAction> addedActions)
		{
			if (!target.IsElectricShocked)
			{
				CombatModel combatModel = source.manager.CombatModel;
				ElectronChargeRelationManager electronChargeRelationManager = combatModel.GetModel<ElectronChargeRelationManager>();
				if (electronChargeRelationManager == null)
				{
					electronChargeRelationManager = new ElectronChargeRelationManager();
					electronChargeRelationManager.SetManager(source.manager);
					combatModel.AddModel(electronChargeRelationManager);
				}
				ElectronChargeRelation newRelation = new ElectronChargeRelation(target, source.Faction, combatModel.TurnManager.TurnCount + electronTurns, MaxLayer, electronTurns);
				electronChargeRelationManager.AddRelation(newRelation);
				ClearElectronChargeAndElectricShockTarget(source, target, addedActions);
			}
		}

		private void ClearElectronChargeAndElectricShockTarget(ActorModel source, ActorModel target, List<ModelAction> addedActions)
		{
			if (target.IsElectricShocked || target.IsDisoriented || target.IsDisorientedLock)
			{
				return;
			}
			CombatModel combatModel = source.manager.CombatModel;
			ElectronChargeRelationManager model = combatModel.GetModel<ElectronChargeRelationManager>();
			if (model == null)
			{
				return;
			}
			ElectronChargeRelation electronChargeRelation = model.ExistedElectronChargeRelations.Find((ElectronChargeRelation x) => x.TargetActor == target && x.FoundingFaction == source.Faction);
			if (electronChargeRelation == null || electronChargeRelation.CurrentLayer < electronChargeRelation.MaxLayer)
			{
				return;
			}
			foreach (ElectronChargeRelation item in model.ExistedElectronChargeRelations.Where((ElectronChargeRelation x) => x.TargetActor == target).ToList())
			{
				combatModel.RemoveModel(item);
			}
			target.NotifyChange("ActorElectronChargeUpdateEvent");
			addedActions.Add(new ElectricShockAction(source, target, ElectricShockTurns, electronChargeRelation.MaxLayer));
		}
	}
}
