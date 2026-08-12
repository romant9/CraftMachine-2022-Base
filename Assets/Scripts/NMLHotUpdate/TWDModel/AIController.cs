using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class AIController
	{
		public static bool VerboseDebug;

		protected List<BehaviorBase> Behaviors;

		public bool IsPvP
		{
			get
			{
				if (Actor != null)
				{
					return Actor.manager.Player.Combat.HasPvPRules;
				}
				return false;
			}
		}

		public bool Enabled { get; set; }

		public AIDataModel AIDataModel
		{
			get
			{
				if (Actor == null)
				{
					return null;
				}
				return Actor.AIDataModel;
			}
		}

		public ActorModel Actor { get; private set; }

		public CombatModel CombatModel => Actor.manager.CombatModel;

		public virtual bool CanPerformOOT
		{
			get
			{
				if (Actor != null)
				{
					if (Actor.Faction == Faction.Walker)
					{
						return AIDataModel.Alertness != AIAlertness.Idle;
					}
					return true;
				}
				return false;
			}
		}

		public bool IsActorIncapacitated
		{
			get
			{
				if (!Actor.IsStunned && !Actor.IsStruggling && !Actor.IsBleedingOut && !Actor.IsEatingLure && !Actor.IsElectricShocked)
				{
					return Actor.IsQuantunCanNotMove;
				}
				return true;
			}
		}

		public virtual bool HasControl => Enabled;

		public AIController(ActorModel actor)
		{
			Actor = actor;
			AIDataModel.ScriptedBehaviorsChanged += OnScriptedBehaviorsChanged;
			AIDataModel.AIAlertnessStateChanged += OnAIAlertnessStateChanged;
			Behaviors = CreateBehaviors();
			Enabled = false;
		}

		private void OnScriptedBehaviorsChanged()
		{
			Behaviors = CreateBehaviors();
		}

		public virtual void AttackTarget(ActorModel actor)
		{
			AIDataModel.SetCurrentTarget(actor);
			AIDataModel.Alertness = AIAlertness.Aggressive;
		}

		public virtual void ReceiveDamage(ActorModel attacker, DamageType damageType)
		{
			if (attacker != null && attacker != Actor)
			{
				if (damageType != DamageType.Suffer)
				{
					ClearHerd();
				}
				AIDataModel.SetEvent(AIDataModel.DamageReceived);
				if (AIDataModel.GetCurrentTarget() == null || AIDataModel.Alertness < AIAlertness.Homing)
				{
					AttackTarget(attacker);
				}
			}
		}

		public virtual void SeeEnemy(ActorModel enemy)
		{
			ActorModel currentTarget = AIDataModel.GetCurrentTarget();
			if (!AIDataModel.HasEvent(AIDataModel.EnemySeen) && (currentTarget == null || currentTarget == enemy) && !Actor.IsStruggling && !Actor.IsStunned && !Actor.IsElectricShocked && !Actor.IsQuantunCanNotMove && !Actor.IsEatingLure && Actor.ExclusiveTimedEffect == null)
			{
				Actor.NotifyChange("actorTurnToTarget", enemy.GridCoordinate);
				AIDataModel.SetEvent(AIDataModel.EnemySeen);
			}
		}

		public virtual void IsStuck(bool enabled)
		{
			if (enabled)
			{
				AIDataModel.SetEvent(AIDataModel.IsStuck);
			}
			else
			{
				AIDataModel.ClearEvent(AIDataModel.IsStuck);
			}
		}

		public virtual bool IsStuck()
		{
			return AIDataModel.HasEvent(AIDataModel.IsStuck);
		}

		public virtual void FollowTarget(ActorModel target)
		{
			AIDataModel.Alertness = AIAlertness.Alerted;
			AIDataModel.SetModelReference(AIDataModel.FollowTarget, target);
		}

		public virtual void ClearFollowTarget()
		{
			AIDataModel.SetModelReference(AIDataModel.FollowTarget, null);
		}

		public virtual void ClearHerd()
		{
			if (Actor.ExclusiveTimedEffect != null && Actor.ExclusiveTimedEffect.Type == TimedEffectType.Herd)
			{
				AIDataModel.SetCurrentTarget(IsPvP ? AIBehaviorHelpers.GetPvPAttackTarget(Actor, CombatModel, AIDataModel.GetCurrentTarget(), closest: true) : AIBehaviorHelpers.GetAttackTarget(Actor, CombatModel, AIDataModel.GetCurrentTarget(), closest: true));
				Actor.FinishTimedEffect(interrupted: true);
			}
		}

		public virtual void HeardNoise(GridCoordinate source)
		{
			AIDataModel.SetEvent(AIDataModel.HeardNoise);
			AIDataModel.SetGridCoordinate(AIDataModel.LastNoiseCoordinate, source);
		}

		public virtual bool IsFighting()
		{
			if (AIDataModel == null || AIDataModel.Alertness < AIAlertness.Homing)
			{
				return IsActorIncapacitated;
			}
			return true;
		}

		private BehaviorBase GetHighestPriorityBehavior()
		{
			BehaviorBase result = null;
			if (Behaviors != null)
			{
				int num = -1;
				foreach (BehaviorBase behavior in Behaviors)
				{
					int priority = behavior.GetPriority();
					if (priority > num)
					{
						num = priority;
						result = behavior;
					}
				}
			}
			return result;
		}

		public virtual void ExecuteTurn()
		{
			if (CombatModel.AILog != null)
			{
				CombatModel.AILog.StartLogEntry(CombatModel.TurnManager.TurnCount, Actor);
			}
			if (Behaviors != null && Behaviors.Count > 0)
			{
				int num = 0;
				while (Actor.TurnState != TurnState.Completed)
				{
					BehaviorBase highestPriorityBehavior = GetHighestPriorityBehavior();
					ActorModel currentTarget = AIDataModel.GetCurrentTarget();
					if (highestPriorityBehavior.GetType() == typeof(WalkerAttackBehavior) || highestPriorityBehavior.GetType() == typeof(RaiderAttackBehavior))
					{
						currentTarget?.manager?.ExecuteAction(new PreAttackAction(currentTarget, Actor));
					}
					if (Actor.IsDead || IsActorIncapacitated)
					{
						Actor.EndAction();
						break;
					}
					OnPreExecuteBehavior();
					if (CombatModel.AILog != null && CombatModel.AILog.CurrentActorTurnLogEntry != null)
					{
						ActorTurnEntry currentActorTurnLogEntry = CombatModel.AILog.CurrentActorTurnLogEntry;
						currentActorTurnLogEntry.AddBehaviorLogEntry(highestPriorityBehavior);
						currentActorTurnLogEntry.SetBeginCoordinate(Actor.GridCoordinate);
						currentActorTurnLogEntry.SetPreExecuteCurrentTarget(AIDataModel.GetCurrentTarget());
						currentActorTurnLogEntry.SetAlertnessState(AIDataModel.Alertness);
						currentActorTurnLogEntry.SetAIMode(AIDataModel.Mode);
					}
					if (VerboseDebug)
					{
						Actor.manager.Debug.Log(">>>>> Selected behavior for '" + Actor?.ToString() + "' is '" + highestPriorityBehavior.GetType().Name + "'");
					}
					highestPriorityBehavior.ExecuteAction();
					if (CombatModel.AILog != null && CombatModel.AILog.CurrentActorTurnLogEntry != null)
					{
						ActorTurnEntry currentActorTurnLogEntry2 = CombatModel.AILog.CurrentActorTurnLogEntry;
						currentActorTurnLogEntry2.SetEndCoordinate(Actor.GridCoordinate);
						currentActorTurnLogEntry2.SetMoveToTarget(AIDataModel.GetGridCoordinate(AIDataModel.MoveToCoordinate));
						currentActorTurnLogEntry2.SetAfterExecuteCurrentTarget(AIDataModel.GetCurrentTarget());
					}
					num++;
					if (num > 9)
					{
						Actor.EndAction();
						Actor.manager.Debug.LogWarning("Potential infinite loop in AIController for Actor '" + Actor?.ToString() + "'. EndAction called to prevent getting stuck.");
						break;
					}
				}
			}
			if (CombatModel.AILog != null)
			{
				CombatModel.AILog.EndLogEntry();
			}
		}

		protected virtual void OnPreExecuteBehavior()
		{
			CombatModel.GetFactionAIController(Actor.Faction)?.UpdateSituation();
		}

		private List<BehaviorBase> CreateBehaviors()
		{
			List<BehaviorBase> list = CreateSystemicBehaviors();
			foreach (string scriptedBehaviorClass in AIDataModel.ScriptedBehaviorClasses)
			{
				Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BehaviorBase), scriptedBehaviorClass);
				if (type != null && Activator.CreateInstance(type, this) is BehaviorBase item)
				{
					list.Add(item);
				}
			}
			return list;
		}

		protected virtual List<BehaviorBase> CreateSystemicBehaviors()
		{
			return new List<BehaviorBase>();
		}

		private void OnAIAlertnessStateChanged(AIAlertness prevState, AIAlertness newState)
		{
			Actor.NotifyChange("actorAIAlertnessStateChanged", newState);
		}
	}
}
