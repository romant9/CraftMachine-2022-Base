using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class MoveTutorialNode : NodeBase
	{
		[GraphItVariable("End turn for actor after user performs the move.")]
		public bool EndTurn = true;

		[GraphItVariable("Is the actor uninterruptable while performing the move.")]
		public bool Uninterruptable = true;

		public GridCoordinate TargetCoordinate;

		public bool Active;

		[JsonIgnore]
		private ActorModel TargetActor;

		[JsonIgnore]
		[GraphItImportData("Target Actors", "")]
		public List<ActorModel> TargetActors => Import("Target Actors") as List<ActorModel>;

		public MoveTutorialNode()
		{
		}

		public MoveTutorialNode(MoveTutorialNode node)
			: base(node)
		{
			EndTurn = node.EndTurn;
			Uninterruptable = node.Uninterruptable;
			TargetCoordinate = node.TargetCoordinate;
			Active = node.Active;
			TargetActor = node.TargetActor;
		}

		public override NodeBase RecordValue()
		{
			return new MoveTutorialNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			Active = false;
			TargetActor = null;
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
				TargetActor = TargetActors[0];
				base.manager.CombatModel.TurnManager.CanSwitchActiveActor = true;
				base.manager.CombatModel.TurnManager.ActiveActor = TargetActor;
				base.manager.CombatModel.TurnManager.CanSwitchActiveActor = false;
				base.manager.CombatModel.SetSuggestedInteractionTarget(base.manager.CombatModel.TurnManager.ActiveActor, TargetCoordinate);
				TargetActor.Changed += OnActorModelChanged;
				if (Uninterruptable)
				{
					TargetActor.AddTrait("TutorialUninterruptable");
				}
				base.manager.CombatModel.TurnManager.FactionPostChanged += OnFactionPostChanged;
				Active = true;
			}
			else
			{
				base.manager.Debug.LogError("Trying to start MoveTutorialNode with null actor list or zero (0) actors in the list.");
			}
		}

		private void EndTutorial()
		{
			if (Active)
			{
				Active = false;
				base.manager.CombatModel.TurnManager.CanSwitchActiveActor = true;
				base.manager.CombatModel.ClearSuggestedInteractionTarget();
				TargetActor.Changed -= OnActorModelChanged;
				if (Uninterruptable)
				{
					TargetActor.RemoveTrait("TutorialUninterruptable");
				}
				base.manager.CombatModel.TurnManager.FactionPostChanged -= OnFactionPostChanged;
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
			switch (changed)
			{
			case "actorMoveCompleted":
			case "actorSecondMoveCompleted":
			case "actorAbilityCompleted":
				EndTutorial();
				if (EndTurn)
				{
					TargetActor.EndAction();
				}
				Completed();
				break;
			}
		}

		[GraphItOutput("Completed", "")]
		public void Completed()
		{
			Fire("Completed");
		}
	}
}
