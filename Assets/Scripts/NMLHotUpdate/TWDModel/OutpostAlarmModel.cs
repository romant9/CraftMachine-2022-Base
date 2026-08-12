namespace TWDModel
{
	public class OutpostAlarmModel : TWDModelObjectWithViewId, InteractionReceiver
	{
		public OutpostAlarmModel()
		{
		}

		public OutpostAlarmModel(string viewId)
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
			base.manager.Player.Combat.ActivateMaxTurnTimer();
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
