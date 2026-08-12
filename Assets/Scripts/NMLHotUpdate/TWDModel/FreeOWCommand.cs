using BaseModel;

namespace TWDModel
{
	public class FreeOWCommand : ModelCommand
	{
		public int TargetActorId { get; private set; }

		public FreeOWCommand()
		{
		}

		public FreeOWCommand(ActorModel actor, int targetActorId)
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
			if (model.HasTraitsThatContains("Equipment_Passive_FreeOW"))
			{
				model2.ResetFreeOW();
				if (model2.Faction == Faction.Raider)
				{
					combatModel.AbilityManager.VisitParameter("Equipment_Passive_FreeOWChanceNotToRaider", ref value, model);
				}
				fixedPoint = 0.0;
				if (value != 0.0 && combatModel.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, value, fixedPoint) != PlayerRandomChanceResult.Failed)
				{
					model2.SetFreeOWState(state: true);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
