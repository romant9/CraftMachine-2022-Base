using System.Collections.Generic;

namespace TWDModel
{
	public class PestilenceTrait : ActionModifier
	{
		private FixedPoint SpreadRadius;

		private int SpreadEnemyCount;

		public PestilenceTrait(FixedPoint spreadRadius, int spreadEnemyCount)
		{
			SpreadRadius = spreadRadius;
			SpreadEnemyCount = spreadEnemyCount;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PestilenceAction pestilenceAction && pestilenceAction.Source == actor && pestilenceAction.IsMainTarget && pestilenceAction.Target != null && !pestilenceAction.Target.IsDead)
			{
				CombatModel combatModel = base.manager.CombatModel;
				List<ActorModel> radiusEnemies = GetRadiusEnemies(combatModel, pestilenceAction.Source, pestilenceAction.Target.GridCoordinate);
				if (radiusEnemies.Count == 0)
				{
					return ActionListClearFlag.Keep;
				}
				CreatePestilence(pestilenceAction.Source, pestilenceAction.Target, radiusEnemies, pestilenceAction.ResetTurns);
			}
			return ActionListClearFlag.Keep;
		}

		private List<ActorModel> GetRadiusEnemies(CombatModel combatModel, ActorModel source, GridCoordinate coordinate)
		{
			List<ActorModel> list = new List<ActorModel>();
			foreach (GridCoordinate coordinate2 in combatModel.Grid.Coordinates)
			{
				if (coordinate2.CheckGridInWidthAndHeightRange(coordinate, (int)SpreadRadius))
				{
					ActorModel occupier = combatModel.GetOccupier(coordinate2);
					if (!(coordinate2 == coordinate) && occupier != null && occupier.Faction != source.Faction)
					{
						list.Add(occupier);
					}
				}
			}
			if (list.Count <= SpreadEnemyCount)
			{
				return list;
			}
			List<ActorModel> list2 = new List<ActorModel>();
			for (int i = 0; i < SpreadEnemyCount; i++)
			{
				ActorModel randomElement = combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: true);
				list2.Add(randomElement);
			}
			return list2;
		}

		private void CreatePestilence(ActorModel source, ActorModel pestilenceSource, List<ActorModel> pestilenceTargets, int resetTurns)
		{
			CombatModel combatModel = source.manager.CombatModel;
			PoisonRelationsManager poisonRelationsManager = combatModel.GetModel<PoisonRelationsManager>();
			if (poisonRelationsManager == null)
			{
				poisonRelationsManager = new PoisonRelationsManager();
				poisonRelationsManager.SetManager(source.manager);
				combatModel.AddModel(poisonRelationsManager);
			}
			PoisonRelation poisonRelation = poisonRelationsManager.ExistedPoisonRelations.Find((PoisonRelation x) => x.SourceActor == source && x.TargetActor == pestilenceSource);
			if (poisonRelation == null)
			{
				return;
			}
			foreach (ActorModel pestilenceTarget in pestilenceTargets)
			{
				PoisonRelation poisonRelation2 = poisonRelationsManager.ExistedPoisonRelations.Find((PoisonRelation x) => x.SourceActor == source && x.TargetActor == pestilenceTarget);
				if (poisonRelation2 == null)
				{
					PoisonRelation newRelation = new PoisonRelation(source, pestilenceTarget, source.Faction, combatModel.TurnManager.TurnCount + resetTurns, poisonRelation.AttackerDamagePercentage, poisonRelation.MaxLayerCount, poisonRelation.CurrentLayerCount, resetTurns);
					poisonRelationsManager.AddRelation(newRelation);
					continue;
				}
				if (poisonRelation.CurrentLayerCount > poisonRelation2.CurrentLayerCount)
				{
					poisonRelation2.SetCurrentLayerCount(poisonRelation.CurrentLayerCount);
				}
				poisonRelation2.ExpiryTurn = combatModel.TurnManager.TurnCount + resetTurns;
				poisonRelation2.LeftTurns = resetTurns;
				poisonRelation2.TargetActor.NotifyChange("ActorPoisonUpdate");
			}
		}
	}
}
