using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class AstheniaRelationsManager : ActorToActorRelationsManager
	{
		[JsonIgnore]
		public List<AstheniaRelation> ExistedAstheniaRelations => base.manager.CombatModel.Models.OfType<AstheniaRelation>().ToList();

		protected override RelationType RelationType => RelationType.Asthenia;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (!actor.IsDead)
			{
				return;
			}
			foreach (AstheniaRelation item in ExistedAstheniaRelations.Where((AstheniaRelation x) => x.TargetActor == actor).ToList())
			{
				base.manager.CombatModel.RemoveModel(item);
			}
		}

		protected override ShouldAddRelationStatus CheckShouldAdd(ActorToActorRelation newRelation)
		{
			if (ExistedAstheniaRelations == null || ExistedAstheniaRelations.Count == 0)
			{
				newRelation.TargetActor.NotifyChange("ActorBeAsthenia");
				return ShouldAddRelationStatus.CanAdd;
			}
			if (ExistedAstheniaRelations.Find((AstheniaRelation x) => x.TargetActor == newRelation.TargetActor) == null)
			{
				newRelation.TargetActor.NotifyChange("ActorBeAsthenia");
				return ShouldAddRelationStatus.CanAdd;
			}
			return ShouldAddRelationStatus.AlreadyHave;
		}

		protected override void PostCheckShouldAdd(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation newRelation)
		{
			if (shouldAddRelationStatus == ShouldAddRelationStatus.CanNotAdd || shouldAddRelationStatus == ShouldAddRelationStatus.AlreadyHave)
			{
				AstheniaRelation astheniaRelation = ExistedAstheniaRelations.Find((AstheniaRelation x) => x.TargetActor == newRelation.TargetActor);
				if (astheniaRelation != null && newRelation is AstheniaRelation astheniaRelation2)
				{
					astheniaRelation.UpdateRelation(astheniaRelation2.SourceActor, astheniaRelation2.FoundingFaction, astheniaRelation2.ExpiryTurn, astheniaRelation2.MakeEnemyDecreaseAttackPercentage, astheniaRelation2.MakeEnemyDecreaseDecreaseDamagePercentage);
				}
			}
		}

		protected override void NotifyRelationChanged(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation relation)
		{
			relation.TargetActor.NotifyChange("ActorAstheniaUpdate");
		}

		protected override void OnRemoveRelation(ActorToActorRelation relation)
		{
			if (!relation.TargetActor.IsDead)
			{
				relation.TargetActor.NotifyChange("ActorAstheniaUpdate");
			}
		}

		protected override void OnExpirationFactionTurn(ActorToActorRelation relation)
		{
			if (relation is AstheniaRelation astheniaRelation && !astheniaRelation.TargetActor.IsDead)
			{
				astheniaRelation.SubtractLeftTurns();
				astheniaRelation.TargetActor.NotifyChange("ActorAstheniaUpdate");
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
