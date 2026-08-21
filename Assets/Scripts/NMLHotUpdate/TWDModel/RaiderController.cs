using System.Collections.Generic;

namespace TWDModel
{
	public class RaiderController : AIController
	{
		public RaiderController(ActorModel actor)
			: base(actor)
		{
			if (base.IsPvP)
			{
				base.Enabled = false;
			}
			else
			{
				base.Enabled = true;
			}
		}

		public override void ExecuteTurn()
		{
			TryAutoUseFortificationsForDefender();
			if (base.Actor is TankActorModel)
			{
				OnPreExecuteBehavior();
			}
			base.ExecuteTurn();
		}

		public override void AttackTarget(ActorModel actor)
		{
			if (base.IsPvP)
			{
				if (!actor.IsWalker)
				{
					base.AttackTarget(actor);
				}
			}
			else
			{
				base.AttackTarget(actor);
			}
		}

		public override void SeeEnemy(ActorModel enemy)
		{
			base.SeeEnemy(enemy);
			if (base.AIDataModel.GetCurrentTarget() == null || base.AIDataModel.Alertness < AIAlertness.Homing)
			{
				AttackTarget(enemy);
			}
			ClearFollowTarget();
		}

		protected override void OnPreExecuteBehavior()
		{
			base.OnPreExecuteBehavior();
			ActorModel actorModel = base.AIDataModel.GetCurrentTarget();
			if (actorModel != null && (actorModel.IsDead || actorModel.IsStruggling || actorModel.IsBleedingOut))
			{
				actorModel = null;
				base.AIDataModel.SetCurrentTarget(actorModel);
			}
			ActorModel actorModel2 = (base.IsPvP ? AIBehaviorHelpers.GetPvPAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true) : AIBehaviorHelpers.GetAttackTarget(base.Actor, base.CombatModel, actorModel, closest: true));
			if (base.Actor.MoveCompleted && !base.IsActorIncapacitated && actorModel != null)
			{
				EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
				if (weaponEquipment != null && weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(base.CombatModel, base.Actor, base.Actor.GridCoordinate, actorModel.GridCoordinate) != AbilityResult.Success && actorModel2 != null && weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(base.CombatModel, base.Actor, base.Actor.GridCoordinate, actorModel2.GridCoordinate) != AbilityResult.Success)
				{
					actorModel2 = null;
				}
			}
			if (actorModel2 != null && actorModel2 != actorModel)
			{
				actorModel = actorModel2;
				base.AIDataModel.SetCurrentTarget(actorModel);
			}
			if (actorModel != null && AIBehaviorHelpers.CanSeeTarget(base.Actor, base.CombatModel, actorModel) && base.AIDataModel.Alertness < AIAlertness.Homing)
			{
				AttackTarget(actorModel);
			}
			ActorModel buddyAidTarget = AIBehaviorHelpers.GetBuddyAidTarget(base.Actor, base.CombatModel);
			base.AIDataModel.SetBuddyAidTarget(buddyAidTarget);
			TryAutoUseFortificationsForDefender();
		}

		private bool TryAutoUseFortificationsForDefender()
		{
			if (base.Actor == null || base.Actor.Faction != Faction.Raider || base.Actor.IsDead || base.IsActorIncapacitated || base.AIDataModel == null || (base.AIDataModel.Alertness != AIAlertness.Alerted && base.AIDataModel.Alertness != AIAlertness.Aggressive) || base.Actor.IsInFortifications)
			{
				return false;
			}
			return (base.Actor.CommandSkillModelManager?.GetCommandSkill<FortificationsSkill>(CommandSkillType.CommandSkillFortifications))?.ReleaseSkillToTargetCell(GridCoordinate.Invalid) ?? false;
		}

		protected override List<BehaviorBase> CreateSystemicBehaviors()
		{
			if (base.IsPvP)
			{
				List<BehaviorBase> list = null;
				if (base.AIDataModel.Mode == AIMode.Stationary)
				{
					return new List<BehaviorBase>
					{
						new ActorEndTurnBehavior(this),
						new RaiderIdleBehavior(this),
						new RaiderAttackBehavior(this)
					};
				}
				return new List<BehaviorBase>
				{
					new ActorEndTurnBehavior(this),
					new RaiderIdleBehavior(this),
					new RaiderMoveBehavior(this),
					new RaiderAttackBehavior(this)
				};
			}
			if (base.AIDataModel.Mode == AIMode.Stationary)
			{
				return new List<BehaviorBase>
				{
					new ActorEndTurnBehavior(this),
					new RaiderIdleBehavior(this),
					new RaiderAttackBehavior(this)
				};
			}
			return new List<BehaviorBase>
			{
				new ActorEndTurnBehavior(this),
				new RaiderIdleBehavior(this),
				new RaiderMoveBehavior(this),
				new RaiderAttackBehavior(this)
			};
		}
	}
}
