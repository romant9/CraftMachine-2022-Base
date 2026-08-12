namespace TWDModel
{
	public class DoorModel : TWDModelObjectWithViewId, InteractionReceiver, TriggerReceiver
	{
		public const string ChangeIsOpen = "IsOpen";

		public const string ChangeIsHidden = "IsHidden";

		public bool IsOpen { get; set; }

		public bool IsHidden { get; set; }

		public DoorModel()
		{
		}

		public DoorModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public void SetHidden(ActorModel instigator, bool hidden)
		{
			IsHidden = hidden;
			NotifyChange("IsHidden", instigator);
		}

		public void FlipDoor(ActorModel instigator)
		{
			IsOpen = !IsOpen;
			NotifyChange("IsOpen", instigator);
		}

		public void OnInteractionStep(InteractiveObjectModel instigator, ActorModel interactingActor)
		{
		}

		public void OnInteractionCanceled(InteractiveObjectModel instigator, ActorModel interactingActor)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel instigator, ActorModel interactingActor)
		{
			FlipDoor(interactingActor);
		}

		public void OnAttacked(InteractiveObjectModel instigator, ActorModel attackingActor)
		{
		}

		public void OnDestroyed(InteractiveObjectModel instigator, ActorModel attackingActor)
		{
			if (!IsOpen)
			{
				IsOpen = true;
				NotifyChange("IsOpen", attackingActor);
			}
		}

		public void OnTriggered(ActorModel instigator)
		{
			FlipDoor(instigator);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
