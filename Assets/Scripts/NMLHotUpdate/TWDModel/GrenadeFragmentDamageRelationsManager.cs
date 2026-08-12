using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GrenadeFragmentDamageRelationsManager : ActorToActorRelationsManager
	{
		[JsonIgnore]
		public List<GrenadeFragmentDamageRelation> ExistedGrenadeFragmentDamageRelationRelations => base.manager.CombatModel.Models.OfType<GrenadeFragmentDamageRelation>().ToList();

		protected override RelationType RelationType => RelationType.GrenadeFragmentDamage;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (!actor.IsDead)
			{
				return;
			}
			foreach (GrenadeFragmentDamageRelation item in ExistedGrenadeFragmentDamageRelationRelations.Where((GrenadeFragmentDamageRelation x) => x.TargetActor == actor).ToList())
			{
				base.manager.CombatModel.RemoveModel(item);
			}
		}

		protected override ShouldAddRelationStatus CheckShouldAdd(ActorToActorRelation newRelation)
		{
			if (ExistedGrenadeFragmentDamageRelationRelations == null || ExistedGrenadeFragmentDamageRelationRelations.Count == 0)
			{
				newRelation.TargetActor.NotifyChange("ActorBeGrenadeFragmentDamaged");
				return ShouldAddRelationStatus.CanAdd;
			}
			if (ExistedGrenadeFragmentDamageRelationRelations.Find((GrenadeFragmentDamageRelation x) => x.TargetActor == newRelation.TargetActor && x.FoundingFaction == newRelation.FoundingFaction) == null)
			{
				return ShouldAddRelationStatus.CanAdd;
			}
			return ShouldAddRelationStatus.AlreadyHave;
		}

		protected override void PostCheckShouldAdd(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation newRelation)
		{
			if (shouldAddRelationStatus == ShouldAddRelationStatus.CanNotAdd || shouldAddRelationStatus == ShouldAddRelationStatus.AlreadyHave)
			{
				GrenadeFragmentDamageRelation grenadeFragmentDamageRelation = ExistedGrenadeFragmentDamageRelationRelations.Find((GrenadeFragmentDamageRelation x) => x.TargetActor == newRelation.TargetActor && x.FoundingFaction == newRelation.FoundingFaction);
				if (grenadeFragmentDamageRelation != null && newRelation is GrenadeFragmentDamageRelation grenadeFragmentDamageRelation2)
				{
					grenadeFragmentDamageRelation.UpdateRelation(grenadeFragmentDamageRelation2.SourceActor, grenadeFragmentDamageRelation2.FoundingFaction, grenadeFragmentDamageRelation2.AdditionDamagePercentage, grenadeFragmentDamageRelation2.AdditionAddDamagePercentage);
				}
			}
		}

		public void RemoveRelation(CombatModel combatModel, ActorToActorRelation relation)
		{
			OnRemoveRelation(relation);
			combatModel.RemoveModel(relation);
			RefreshActorAreaStates();
		}

		protected override void NotifyRelationChanged(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation relation)
		{
			if (!relation.TargetActor.IsDead)
			{
				relation.TargetActor.NotifyChange("ActorGrenadeFragmentDamageUpdate");
			}
		}

		protected override void OnRemoveRelation(ActorToActorRelation relation)
		{
			if (!relation.TargetActor.IsDead)
			{
				relation.TargetActor.NotifyChange("ActorGrenadeFragmentDamageUpdate");
			}
		}

		protected override void OnExpirationFactionTurn(ActorToActorRelation relation)
		{
			relation.TargetActor.NotifyChange("ActorGrenadeFragmentDamageUpdate");
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
