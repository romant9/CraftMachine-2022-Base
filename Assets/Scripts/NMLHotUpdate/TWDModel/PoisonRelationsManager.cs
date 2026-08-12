using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class PoisonRelationsManager : ActorToActorRelationsManager
	{
		[JsonIgnore]
		public List<PoisonRelation> ExistedPoisonRelations => base.manager.CombatModel.Models.OfType<PoisonRelation>().ToList();

		protected override RelationType RelationType => RelationType.Poison;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		protected override void Tick(IEnumerable<ActorModel> actorModels)
		{
			if (actorModels == null || ExistedPoisonRelations == null || ExistedPoisonRelations.Count == 0)
			{
				return;
			}
			foreach (ActorModel item in new List<ActorModel>(actorModels))
			{
				if (item.IsDead)
				{
					continue;
				}
				foreach (PoisonRelation existedPoisonRelation in ExistedPoisonRelations)
				{
					if (existedPoisonRelation.FoundingFaction != item.Faction && existedPoisonRelation.TargetActor == item)
					{
						ApplyDmg(existedPoisonRelation.SourceActor, existedPoisonRelation.TargetActor, existedPoisonRelation.AttackerDamagePercentage, existedPoisonRelation.CurrentLayerCount);
					}
				}
			}
		}

		protected void ApplyDmg(ActorModel source, ActorModel target, FixedPoint attackerDamagePercentage, int layerCount)
		{
			if (source is SurvivorModel survivorModel)
			{
				FixedPoint fixedPoint = survivorModel.GetDamageForPreferredWeapon() * attackerDamagePercentage * layerCount;
				CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, target, (int)fixedPoint, 0, DamageType.Poison, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
			}
		}

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (!actor.IsDead)
			{
				return;
			}
			foreach (PoisonRelation item in ExistedPoisonRelations.Where((PoisonRelation x) => x.TargetActor == actor).ToList())
			{
				base.manager.CombatModel.RemoveModel(item);
			}
		}

		protected override ShouldAddRelationStatus CheckShouldAdd(ActorToActorRelation newRelation)
		{
			if (ExistedPoisonRelations == null || ExistedPoisonRelations.Count == 0)
			{
				newRelation.TargetActor.NotifyChange("ActorBePoisoned");
				return ShouldAddRelationStatus.CanAdd;
			}
			PoisonRelation poisonRelation = ExistedPoisonRelations.Find((PoisonRelation x) => x.SourceActor == newRelation.SourceActor && x.TargetActor == newRelation.TargetActor);
			if (poisonRelation == null)
			{
				newRelation.TargetActor.NotifyChange("ActorBePoisoned");
				return ShouldAddRelationStatus.CanAdd;
			}
			if (poisonRelation.CurrentLayerCount >= poisonRelation.MaxLayerCount)
			{
				return ShouldAddRelationStatus.CanNotAdd;
			}
			return ShouldAddRelationStatus.AlreadyHave;
		}

		protected override void PostCheckShouldAdd(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation newRelation)
		{
			if (shouldAddRelationStatus == ShouldAddRelationStatus.CanNotAdd || shouldAddRelationStatus == ShouldAddRelationStatus.AlreadyHave)
			{
				PoisonRelation poisonRelation = ExistedPoisonRelations.Find((PoisonRelation x) => x.SourceActor == newRelation.SourceActor && x.TargetActor == newRelation.TargetActor);
				if (poisonRelation != null)
				{
					poisonRelation.ExpiryTurn = newRelation.ExpiryTurn;
					poisonRelation.LeftTurns = poisonRelation.ExpiryTurn - base.manager.CombatModel.TurnManager.TurnCount;
					poisonRelation.AddLayerCount();
				}
			}
		}

		protected override void NotifyRelationChanged(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation relation)
		{
			relation.TargetActor.NotifyChange("ActorPoisonUpdate");
		}

		protected override void OnRemoveRelation(ActorToActorRelation relation)
		{
			if (!relation.TargetActor.IsDead)
			{
				relation.TargetActor.NotifyChange("ActorPoisonUpdate");
			}
		}

		protected override void OnExpirationFactionTurn(ActorToActorRelation relation)
		{
			if (relation is PoisonRelation poisonRelation && !poisonRelation.TargetActor.IsDead)
			{
				poisonRelation.SubtractLeftTurns();
				poisonRelation.TargetActor.NotifyChange("ActorPoisonUpdate");
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
