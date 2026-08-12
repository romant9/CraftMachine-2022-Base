namespace TWDModel
{
	public class OutpostObjectiveModel : TWDModelObjectWithViewId, InteractionReceiver
	{
		public OutpostObjectiveType OutpostObjectiveType { get; set; }

		public OutpostObjectiveModel()
		{
		}

		public OutpostObjectiveModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnDestroyed(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
			if (OutpostObjectiveType == OutpostObjectiveType.ResourceContainer)
			{
				base.manager.Player.Combat.SetPvPLootCollected();
			}
			else if (OutpostObjectiveType == OutpostObjectiveType.Flag)
			{
				base.manager.Player.Combat.SetPvPFlagCollected();
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
