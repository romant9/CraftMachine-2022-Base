namespace TWDModel
{
	public class CombatDialogPlayerModel : TWDModelObjectWithViewId, InteractionReceiver, TriggerReceiver
	{
		public const string DialogPlayerTriggered = "DialogPlayerTriggered";

		public CombatDialogPlayerModel()
		{
		}

		public CombatDialogPlayerModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void StartDialog(ActorModel instigator)
		{
			NotifyChange("DialogPlayerTriggered", instigator);
		}

		public void OnTriggered(ActorModel instigator)
		{
			StartDialog(instigator);
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			StartDialog(instigator);
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnDestroyed(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}
	}
}
