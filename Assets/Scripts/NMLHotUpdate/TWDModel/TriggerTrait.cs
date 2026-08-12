using System.Collections.Generic;

namespace TWDModel
{
	public class TriggerTrait : ActionModifier
	{
		private int triggerChance = 100;

		public TriggerTrait()
		{
		}

		public TriggerTrait(int chance)
		{
			triggerChance = chance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			FixedPoint value = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
			int chanceExtension = (int)(value * 100.0);
			ActorModel model = base.manager.GetModel<ActorModel>(action.ModelId);
			if (model != actor)
			{
				return ActionListClearFlag.Keep;
			}
			MoveAction moveAction = action as MoveAction;
			PushActorAction pushActorAction = action as PushActorAction;
			if (moveAction != null || pushActorAction != null)
			{
				GridPath gridPath = ((moveAction != null) ? moveAction.Path : pushActorAction.Path);
				foreach (TriggerModel model2 in model.manager.CombatModel.GetModels<TriggerModel>())
				{
					for (int i = 0; i < gridPath.Count; i++)
					{
						GridCoordinate coordinate = gridPath[i];
						if (model2.CanTrigger(model, coordinate) && base.manager.Player.RollDice(RollDiceType.Trigger, triggerChance, chanceExtension) != PlayerRandomChanceResult.Failed && model2.TryReserveActivation(model, coordinate))
						{
							addedActions.Add(new TriggerAction(model, model2));
							if (model2.InterruptActor)
							{
								gridPath.ClipTo(coordinate);
								gridPath.ClearTargetCoordinate();
								model.EndAction();
								return ActionListClearFlag.Clear;
							}
							return ActionListClearFlag.Keep;
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
