using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class TriggerModel : TWDSpatialModelObject
	{
		public List<TriggerReceiver> receivers;

		[JsonIgnore]
		protected List<TriggerReceiver> NonModelReceivers;

		public int CurrentActivationCount;

		public bool InterruptActor;

		public Faction Faction { get; set; }

		public int ActivationCount { get; set; }

		public bool HasBeenActivated => CurrentActivationCount > 0;

		protected bool CanActivate
		{
			get
			{
				if (CurrentActivationCount >= ActivationCount)
				{
					return ActivationCount < 0;
				}
				return true;
			}
		}

		public override void Initialize()
		{
			CurrentActivationCount = 0;
		}

		public TriggerModel()
		{
			receivers = new List<TriggerReceiver>();
			NonModelReceivers = new List<TriggerReceiver>();
		}

		public TriggerModel(string viewId, List<GridCoordinate> gridCoordinates, Faction faction, int activationCount, bool interruptActor)
			: this()
		{
			base.ViewId = viewId;
			Faction = faction;
			ActivationCount = activationCount;
			base.Location = new TWDObjectLocation(gridCoordinates, null);
			InterruptActor = interruptActor;
		}

		protected void NotifyTriggered(ActorModel interactingActor)
		{
			if (receivers != null)
			{
				for (int i = 0; i < receivers.Count; i++)
				{
					receivers[i].OnTriggered(interactingActor);
				}
			}
			if (NonModelReceivers != null)
			{
				for (int j = 0; j < NonModelReceivers.Count; j++)
				{
					NonModelReceivers[j].OnTriggered(interactingActor);
				}
			}
		}

		public void AddNonModelReceiver(TriggerReceiver receiver)
		{
			if (receiver is TWDModelObject)
			{
				base.manager.Debug.LogError("TriggerModel::AddNonModelReceiver() -> Trying to add TWDModelObject as non-model receiver!");
			}
			else
			{
				NonModelReceivers.Add(receiver);
			}
		}

		public void RemoveNonModelReceiver(TriggerReceiver receiver)
		{
			NonModelReceivers.Remove(receiver);
		}

		protected bool FactionMatches(ActorModel instigator)
		{
			if (instigator != null && Faction != instigator.Faction)
			{
				return Faction == Faction.Any;
			}
			return true;
		}

		public bool Trigger(ActorModel instigator)
		{
			if (CanActivate && FactionMatches(instigator))
			{
				CurrentActivationCount++;
				NotifyTriggered(instigator);
				return true;
			}
			return false;
		}

		public bool TryReserveActivation(ActorModel instigator, GridCoordinate coordinate)
		{
			if (!CanTrigger(instigator, coordinate))
			{
				return false;
			}
			CurrentActivationCount++;
			return true;
		}

		internal bool TriggerReserved(ActorModel instigator)
		{
			if (!FactionMatches(instigator))
			{
				return false;
			}
			NotifyTriggered(instigator);
			return true;
		}

		public virtual bool CanTrigger(ActorModel instigator, GridCoordinate coordinate)
		{
			bool flag = false;
			foreach (GridCoordinate coordinate2 in base.Location.Coordinates)
			{
				if (coordinate.IsValid && coordinate2 == coordinate)
				{
					flag = true;
					break;
				}
			}
			if (flag && CanActivate)
			{
				return FactionMatches(instigator);
			}
			return false;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
