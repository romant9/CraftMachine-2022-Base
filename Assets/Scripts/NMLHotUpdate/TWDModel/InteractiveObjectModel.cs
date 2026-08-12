using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class InteractiveObjectModel : TWDSpatialModelObject
	{
		public const string InteractionDisabledEvent = "InteractionDisabledEvent";

		public List<InteractionReceiver> receivers;

		[JsonIgnore]
		protected List<InteractionReceiver> NonModelReceivers;

		public int lastTurnAttacked = -1;

		public Direction InteractionDirection { get; protected set; }

		public Placement Placement { get; protected set; }

		public int TurnsToComplete { get; set; }

		public int NPCAttacksToDestroy { get; set; }

		public int NPCAttackCount { get; set; }

		public bool VisibleInFog { get; protected set; }

		public int UsedTurns { get; set; }

		public bool OneTimeOnly { get; protected set; }

		public InteractBy InteractBy { get; protected set; }

		[IgnoreModelProperty]
		public ActorModel Interactor { get; set; }

		public bool InteractionDisabled { get; private set; }

		[JsonIgnore]
		public bool CanBeInteracted
		{
			get
			{
				if (!Completed && !Disabled && Interactor == null)
				{
					return !InteractionDisabled;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool Completed => UsedTurns >= TurnsToComplete;

		[JsonIgnore]
		public bool Disabled { get; set; }

		[JsonIgnore]
		public bool IsVisibleToSurvivors { get; set; }

		public bool HasBeenActivated { get; set; }

		public bool HasInteractionStarted => UsedTurns > 0;

		public void SetInteractionDisabled(bool disabled)
		{
			InteractionDisabled = disabled;
			NotifyChange("InteractionDisabledEvent");
		}

		public InteractiveObjectModel()
		{
			IsVisibleToSurvivors = false;
			receivers = new List<InteractionReceiver>();
			NonModelReceivers = new List<InteractionReceiver>();
		}

		public InteractiveObjectModel(List<GridCoordinate> gridCoordinates, string viewId, int turnsToComplete, bool oneTimeOnly, bool visibleInFog, int attacksToDestroy, Direction interactionDirection, InteractBy interactBy)
		{
			TurnsToComplete = turnsToComplete;
			OneTimeOnly = oneTimeOnly;
			VisibleInFog = visibleInFog;
			IsVisibleToSurvivors = VisibleInFog;
			NPCAttacksToDestroy = attacksToDestroy;
			Placement = Placement.Cell;
			base.ViewId = viewId;
			InteractionDirection = interactionDirection;
			base.Location = new TWDObjectLocation(gridCoordinates, null);
			InteractBy = interactBy;
		}

		public InteractiveObjectModel(List<int> edgeIds, string viewId, int turnsToComplete, bool oneTimeOnly, bool visibleInFog, int attacksToDestroy, Direction interactionDirection, InteractBy interactBy)
		{
			TurnsToComplete = turnsToComplete;
			OneTimeOnly = oneTimeOnly;
			VisibleInFog = visibleInFog;
			IsVisibleToSurvivors = VisibleInFog;
			NPCAttacksToDestroy = attacksToDestroy;
			Placement = Placement.Edge;
			base.ViewId = viewId;
			InteractionDirection = interactionDirection;
			base.Location = new TWDObjectLocation(null, edgeIds);
			InteractBy = interactBy;
		}

		public void AddNonModelReceiver(InteractionReceiver receiver)
		{
			if (receiver is TWDModelObject)
			{
				base.manager.Debug.LogError("InteractiveObjectModel::AddNonModelReceiver() -> Trying to add TWDModelObject as non-model receiver!");
			}
			else
			{
				NonModelReceivers.Add(receiver);
			}
		}

		public void RemoveNonModelReceiver(InteractionReceiver receiver)
		{
			NonModelReceivers.Remove(receiver);
		}

		public override void Initialize()
		{
			base.Initialize();
			HasBeenActivated = false;
			InteractionDisabled = false;
		}

		protected void NotifyStep(ActorModel interactingActor)
		{
			if (receivers != null)
			{
				for (int i = 0; i < receivers.Count; i++)
				{
					receivers[i].OnInteractionStep(this, interactingActor);
				}
			}
			if (NonModelReceivers != null)
			{
				for (int j = 0; j < NonModelReceivers.Count; j++)
				{
					NonModelReceivers[j].OnInteractionStep(this, interactingActor);
				}
			}
		}

		protected void NotifyCompleted(ActorModel interactingActor)
		{
			if (receivers != null)
			{
				for (int i = 0; i < receivers.Count; i++)
				{
					receivers[i].OnInteractionCompleted(this, interactingActor);
				}
			}
			if (NonModelReceivers != null)
			{
				for (int j = 0; j < NonModelReceivers.Count; j++)
				{
					NonModelReceivers[j].OnInteractionCompleted(this, interactingActor);
				}
			}
		}

		protected void NotifyCanceled(ActorModel interactingActor)
		{
			if (receivers != null)
			{
				for (int i = 0; i < receivers.Count; i++)
				{
					receivers[i].OnInteractionCanceled(this, interactingActor);
				}
			}
			if (NonModelReceivers != null)
			{
				for (int j = 0; j < NonModelReceivers.Count; j++)
				{
					NonModelReceivers[j].OnInteractionCanceled(this, interactingActor);
				}
			}
		}

		protected void NotifyAttacked(ActorModel attackingActor)
		{
			if (receivers != null)
			{
				for (int i = 0; i < receivers.Count; i++)
				{
					receivers[i].OnAttacked(this, attackingActor);
				}
			}
			if (NonModelReceivers != null)
			{
				for (int j = 0; j < NonModelReceivers.Count; j++)
				{
					NonModelReceivers[j].OnAttacked(this, attackingActor);
				}
			}
		}

		protected void NotifyDestroyed(ActorModel attackingActor)
		{
			if (receivers != null)
			{
				for (int i = 0; i < receivers.Count; i++)
				{
					receivers[i].OnDestroyed(this, attackingActor);
				}
			}
			if (NonModelReceivers != null)
			{
				for (int j = 0; j < NonModelReceivers.Count; j++)
				{
					NonModelReceivers[j].OnDestroyed(this, attackingActor);
				}
			}
		}

		public void OnAttacked(ActorModel attackingActor)
		{
			int turnCount = base.manager.CombatModel.TurnManager.TurnCount;
			if (turnCount != lastTurnAttacked)
			{
				lastTurnAttacked = turnCount;
				NPCAttackCount++;
				if (NPCAttackCount == NPCAttacksToDestroy)
				{
					Disabled = true;
					NotifyDestroyed(attackingActor);
				}
				else
				{
					NotifyAttacked(attackingActor);
				}
			}
		}

		public void InteractStep(ActorModel interactingActor, int currentStep, int totalSteps)
		{
			if (!Disabled && !Completed)
			{
				UsedTurns = currentStep;
				NotifyStep(interactingActor);
			}
		}

		public bool CompleteInteraction(ActorModel interactingActor)
		{
			if (Disabled)
			{
				return true;
			}
			UsedTurns = TurnsToComplete;
			if (Completed)
			{
				if (!OneTimeOnly)
				{
					UsedTurns = 0;
				}
				if (!HasBeenActivated)
				{
					HasBeenActivated = true;
				}
				Interactor = null;
				NotifyCompleted(interactingActor);
				return true;
			}
			return false;
		}

		public void CancelInteraction(ActorModel interactingActor)
		{
			Interactor = null;
			TurnsToComplete -= UsedTurns;
			UsedTurns = 0;
			NotifyCanceled(interactingActor);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
