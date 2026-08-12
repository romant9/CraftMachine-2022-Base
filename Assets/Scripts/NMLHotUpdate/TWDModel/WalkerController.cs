using System.Collections.Generic;

namespace TWDModel
{
	public class WalkerController : AIController
	{
		public WalkerModel Walker => base.Actor as WalkerModel;

		public WalkerController(ActorModel actor)
			: base(actor)
		{
			base.Enabled = true;
		}

		public override void HeardNoise(GridCoordinate source)
		{
			base.HeardNoise(source);
			if (base.AIDataModel.Alertness == AIAlertness.Idle)
			{
				base.AIDataModel.Alertness = AIAlertness.Wandering;
				base.AIDataModel.SetGridCoordinate(AIDataModel.MoveToCoordinate, source);
				ClearFollowTarget();
				AlertNearbyWalkers();
			}
		}

		public override void AttackTarget(ActorModel actor)
		{
			if (actor.IsCamouflaged)
			{
				return;
			}
			if (actor.IsDisoriented)
			{
				base.AIDataModel.SetCurrentTarget(actor);
				base.AIDataModel.Alertness = AIAlertness.Homing;
			}
			else if (actor.IsTaunted)
			{
				base.AIDataModel.SetCurrentTarget(actor);
				base.AIDataModel.Alertness = AIAlertness.Homing;
			}
			else if (base.IsPvP)
			{
				if (actor.Faction != Faction.Raider)
				{
					base.AIDataModel.SetCurrentTarget(actor);
					base.AIDataModel.Alertness = AIAlertness.Homing;
				}
			}
			else
			{
				base.AIDataModel.SetCurrentTarget(actor);
				base.AIDataModel.Alertness = AIAlertness.Homing;
			}
		}

		public override void ReceiveDamage(ActorModel attacker, DamageType damageType)
		{
			base.ReceiveDamage(attacker, damageType);
			ClearFollowTarget();
			AlertNearbyWalkers();
		}

		public override void SeeEnemy(ActorModel enemy)
		{
			if (!enemy.IsInvisible && !enemy.IsCamouflaged)
			{
				base.SeeEnemy(enemy);
				if (base.AIDataModel.GetCurrentTarget() == null || base.AIDataModel.Alertness < AIAlertness.Homing)
				{
					AttackTarget(enemy);
				}
				ClearFollowTarget();
				AlertNearbyWalkers();
			}
		}

		protected override void OnPreExecuteBehavior()
		{
			base.OnPreExecuteBehavior();
			ActorModel actorModel = base.AIDataModel.GetCurrentTarget();
			if (actorModel != null && (actorModel.IsDead || actorModel.IsStruggling || actorModel.IsInvisible || actorModel.IsCamouflaged))
			{
				actorModel = null;
				base.AIDataModel.SetCurrentTarget(actorModel);
			}
			bool flag = base.AIDataModel.HasEvent(AIDataModel.ForceCivilianTargets);
			ActorModel actorModel2 = null;
			if (base.Actor.IsHerded)
			{
				if (!base.Actor.ExclusiveTimedEffect.Instigator.IsDead && !base.Actor.ExclusiveTimedEffect.Instigator.IsStruggling)
				{
					actorModel2 = base.Actor.ExclusiveTimedEffect.Instigator;
				}
				else
				{
					ClearHerd();
				}
			}
			if (flag || actorModel2 == null)
			{
				actorModel2 = (base.IsPvP ? AIBehaviorHelpers.GetPvPAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true, flag ? Faction.Civilian : Faction.Any) : AIBehaviorHelpers.GetAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true, flag ? Faction.Civilian : Faction.Any));
				if (flag && actorModel2 == null)
				{
					actorModel2 = (base.IsPvP ? AIBehaviorHelpers.GetPvPAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true) : AIBehaviorHelpers.GetAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true));
				}
			}
			if (base.Actor.MoveCompleted && !base.Actor.IsStunned && !base.Actor.IsStruggling && !base.Actor.IsEatingLure && !base.Actor.IsElectricShocked && !base.Actor.IsQuantunCanNotMove && actorModel != null && !base.CombatModel.CanTraverse(null, base.Actor.GridCoordinate, actorModel.GridCoordinate))
			{
				ActorModel actorModel3 = (base.IsPvP ? AIBehaviorHelpers.GetPvPAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true) : AIBehaviorHelpers.GetAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true));
				if (actorModel3 != null && base.CombatModel.CanTraverse(null, base.Actor.GridCoordinate, actorModel3.GridCoordinate))
				{
					actorModel = actorModel3;
				}
			}
			if (actorModel2 != actorModel)
			{
				actorModel = actorModel2;
				base.AIDataModel.SetCurrentTarget(actorModel);
			}
			if (actorModel != null && AIBehaviorHelpers.CanSeeTarget(base.Actor, base.CombatModel, actorModel) && base.AIDataModel.Alertness < AIAlertness.Homing)
			{
				AttackTarget(actorModel);
			}
		}

		protected override List<BehaviorBase> CreateSystemicBehaviors()
		{
			return new List<BehaviorBase>
			{
				new ActorEndTurnBehavior(this),
				new WalkerIdleBehavior(this),
				new WalkerMoveBehavior(this),
				new WalkerAttackBehavior(this)
			};
		}

		public override void ExecuteTurn()
		{
			if (base.AIDataModel.Alertness == AIAlertness.Alerted)
			{
				base.AIDataModel.Alertness = AIAlertness.Wandering;
			}
			else if (base.AIDataModel.Alertness == AIAlertness.Aggressive)
			{
				base.AIDataModel.Alertness = AIAlertness.Homing;
			}
			base.ExecuteTurn();
		}

		private void AlertNearbyWalkers()
		{
			foreach (ActorModel factionActor in base.CombatModel.GetFactionActors(Faction.Walker))
			{
				if (factionActor != base.Actor && base.AIDataModel.GetModelReference<ActorModel>(AIDataModel.FollowTarget) == null && factionActor.AIController.AIDataModel.Alertness == AIAlertness.Idle && base.Actor.GridCoordinate.DistanceTo(factionActor.GridCoordinate) <= 1.5)
				{
					factionActor.AIDataModel.Alertness = AIAlertness.Wandering;
				}
			}
		}
	}
}
