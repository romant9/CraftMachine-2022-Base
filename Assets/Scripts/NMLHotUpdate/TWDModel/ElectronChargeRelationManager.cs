using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ElectronChargeRelationManager : FactionToActorManager
	{
		[JsonIgnore]
		public List<ElectronChargeRelation> ExistedElectronChargeRelations => base.manager.CombatModel.Models.OfType<ElectronChargeRelation>().ToList();

		protected override bool IgnoreExpireTurnRemove => false;

		protected override FactionToActorRelationType RelationType => FactionToActorRelationType.ElectronCharge;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (!actor.IsDead)
			{
				return;
			}
			foreach (ElectronChargeRelation item in ExistedElectronChargeRelations.Where((ElectronChargeRelation x) => x.TargetActor == actor).ToList())
			{
				base.manager.CombatModel.RemoveModel(item);
			}
		}

		protected override FactionToActorRelationShouldAddRelationStatus CheckShouldAdd(FactionToActorRelation newRelation)
		{
			if (newRelation.TargetActor.IsElectricShocked)
			{
				return FactionToActorRelationShouldAddRelationStatus.CanNotAdd;
			}
			if (ExistedElectronChargeRelations == null)
			{
				return FactionToActorRelationShouldAddRelationStatus.CanAdd;
			}
			ElectronChargeRelation newElectronChargeRelation = newRelation as ElectronChargeRelation;
			if (newElectronChargeRelation == null)
			{
				return FactionToActorRelationShouldAddRelationStatus.CanNotAdd;
			}
			if (ExistedElectronChargeRelations.Find((ElectronChargeRelation x) => x.TargetActor == newElectronChargeRelation.TargetActor) == null)
			{
				return FactionToActorRelationShouldAddRelationStatus.CanAdd;
			}
			return FactionToActorRelationShouldAddRelationStatus.AlreadyHave;
		}

		protected override void PostCheckShouldAdd(FactionToActorRelationShouldAddRelationStatus shouldAddRelationStatus, FactionToActorRelation newRelation)
		{
			if (shouldAddRelationStatus != FactionToActorRelationShouldAddRelationStatus.AlreadyHave)
			{
				return;
			}
			ElectronChargeRelation newElectronChargeRelation = newRelation as ElectronChargeRelation;
			if (newElectronChargeRelation != null)
			{
				ElectronChargeRelation electronChargeRelation = ExistedElectronChargeRelations.Find((ElectronChargeRelation x) => x.TargetActor == newElectronChargeRelation.TargetActor);
				if (electronChargeRelation != null && electronChargeRelation.CurrentLayer < electronChargeRelation.MaxLayer)
				{
					electronChargeRelation.AddCurrentLayer();
					electronChargeRelation.ExpiryTurn = newRelation.ExpiryTurn;
					electronChargeRelation.LeftTurns = electronChargeRelation.ExpiryTurn - base.manager.CombatModel.TurnManager.TurnCount;
					electronChargeRelation.TargetActor.NotifyChange("ActorElectronChargeUpdateEvent");
				}
			}
		}

		protected override void OnRemoveRelation(FactionToActorRelation relation)
		{
			if (!relation.TargetActor.IsDead)
			{
				relation.TargetActor.NotifyChange("ActorElectronChargeUpdateEvent");
			}
		}

		protected override void OnExpirationFactionTurn(FactionToActorRelation relation)
		{
			if (relation is ElectronChargeRelation electronChargeRelation && !electronChargeRelation.TargetActor.IsDead)
			{
				electronChargeRelation.SubtractLeftTurns();
				electronChargeRelation.TargetActor.NotifyChange("ActorElectronChargeUpdateEvent");
			}
		}

		protected override void NotifyRelationChanged(FactionToActorRelationShouldAddRelationStatus shouldAddRelationStatus, FactionToActorRelation relation)
		{
		}

		protected override void NotifyRelationAdded(FactionToActorRelation relation)
		{
			relation.TargetActor.NotifyChange("ActorElectronChargeUpdateEvent");
		}

		protected override void Tick(IEnumerable<ActorModel> actorModels)
		{
		}
	}
}
