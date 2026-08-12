using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class AttackTutorialNode : NodeBase
	{
		[GraphItVariable("How many percent of target health will the damage be. 100 means the attack will deal the same amount of damage that the target has, 150 means it would deal 1.5x the damage.")]
		public int DamagePercentage = -1;

		[GraphItVariable("Is the actor uninterruptable while performing the move.")]
		public bool Uninterruptable = true;

		public GridCoordinate TargetCoordinate;

		public bool Active;

		[JsonIgnore]
		private ActorModel AttackingActor;

		[JsonIgnore]
		private ActorModel VictimActor;

		[JsonIgnore]
		[GraphItImportData("Target Actors", "")]
		public List<ActorModel> TargetActors => Import("Target Actors") as List<ActorModel>;

		public AttackTutorialNode()
		{
		}

		public AttackTutorialNode(AttackTutorialNode node)
			: base(node)
		{
			DamagePercentage = node.DamagePercentage;
			Uninterruptable = node.Uninterruptable;
			TargetCoordinate = node.TargetCoordinate;
			Active = node.Active;
			AttackingActor = node.AttackingActor;
			VictimActor = node.VictimActor;
		}

		public override NodeBase RecordValue()
		{
			return new AttackTutorialNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			Active = false;
			AttackingActor = null;
			VictimActor = null;
		}

		public override void Start()
		{
			base.Start();
			base.manager.OnModelStarted += OnModelStarted;
		}

		private void OnModelStarted()
		{
			if (Active)
			{
				StartTutorial();
			}
			base.manager.OnModelStarted -= OnModelStarted;
		}

		[GraphItInput("Activate", "")]
		public void Activate()
		{
			StartTutorial();
		}

		private void StartTutorial()
		{
			if (TargetActors != null && TargetActors.Count == 1)
			{
				AttackingActor = TargetActors[0];
				VictimActor = base.manager.CombatModel.GetOccupier(TargetCoordinate);
				if (VictimActor != null)
				{
					base.manager.CombatModel.TurnManager.CanSwitchActiveActor = true;
					base.manager.CombatModel.TurnManager.ActiveActor = AttackingActor;
					base.manager.CombatModel.TurnManager.CanSwitchActiveActor = false;
					base.manager.CombatModel.SetSuggestedInteractionTarget(base.manager.CombatModel.TurnManager.ActiveActor, TargetCoordinate);
					AttackingActor.Changed += OnActorModelChanged;
					if (Uninterruptable)
					{
						AttackingActor.AddTrait("TutorialUninterruptable");
					}
					if (DamagePercentage >= 0)
					{
						AttackingActor.AddTrait("TutorialSetDamage", DamagePercentage * VictimActor.Hitpoints / 100 + 1);
					}
					base.manager.CombatModel.TurnManager.FactionPostChanged += OnFactionPostChanged;
					Active = true;
				}
				else
				{
					base.manager.Debug.LogError("AttackTutorialNode target coordinate does not contain actor!");
				}
			}
			else
			{
				base.manager.Debug.LogError("Trying to start AttackTutorialNode with null actor list or zero (0) actors in the list.");
			}
		}

		private void EndTutorial()
		{
			if (Active)
			{
				Active = false;
				base.manager.CombatModel.TurnManager.CanSwitchActiveActor = true;
				base.manager.CombatModel.ClearSuggestedInteractionTarget();
				AttackingActor.Changed -= OnActorModelChanged;
				if (Uninterruptable)
				{
					AttackingActor.RemoveTrait("TutorialUninterruptable");
				}
				if (DamagePercentage >= 0)
				{
					AttackingActor.RemoveTrait("TutorialSetDamage");
				}
				base.manager.CombatModel.TurnManager.FactionPostChanged -= OnFactionPostChanged;
				VictimActor = null;
				AttackingActor = null;
			}
		}

		private void OnFactionPostChanged(Faction oldFaction, Faction newFaction)
		{
			if (newFaction != Faction.Survivor)
			{
				EndTutorial();
			}
		}

		private void OnActorModelChanged(ModelObject m, string changed, object args)
		{
			if (changed == "actorAbilityCompleted")
			{
				EndTutorial();
				Completed();
			}
		}

		[GraphItOutput("Completed", "")]
		public void Completed()
		{
			Fire("Completed");
		}
	}
}
