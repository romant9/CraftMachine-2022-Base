using BaseModel;

namespace TWDModel
{
	public class EquipmentActiveLightCommand : ModelCommand
	{
		public int TargetActorId { get; private set; }

		public EquipmentActiveLightCommand()
		{
		}

		public EquipmentActiveLightCommand(ActorModel actor, int targetActorId)
			: base(actor)
		{
			TargetActorId = targetActorId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			ActorModel model2 = manager.GetModel<ActorModel>(TargetActorId);
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (model == null && model2 == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			if (model.TurnComplete)
			{
				manager.Debug.LogError("[Cheat Alert] MoveCommand failed. Actor already completed its turn");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			FixedPoint fixedPoint = 0.0;
			FixedPoint value = 0.0;
			if (model.HasAnyLevelTrait("Equipment_Active_Light"))
			{
				model2.ResetActiveLight();
				if (model2.Faction == Faction.Walker)
				{
					combatModel.AbilityManager.VisitParameter("AbilityModifierLightChanceNotToBeOverwatchedByWalkers", ref value, model);
				}
				else if (model2.Faction == Faction.Raider)
				{
					combatModel.AbilityManager.VisitParameter("AbilityModifierLightChanceNotToBeHumanEnemies", ref value, model);
				}
				fixedPoint = 0.0;
				if (value != 0.0 && combatModel.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, value, fixedPoint) != PlayerRandomChanceResult.Failed)
				{
					model2.SetActiveLightState(state: true);
					model2.SetIsRandomActiveLightState(state: true);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
