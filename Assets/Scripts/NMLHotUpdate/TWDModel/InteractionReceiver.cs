namespace TWDModel
{
	public interface InteractionReceiver
	{
		void OnInteractionStep(InteractiveObjectModel interactiveObject, ActorModel instigator);

		void OnInteractionCompleted(InteractiveObjectModel interactiveObject, ActorModel instigator);

		void OnInteractionCanceled(InteractiveObjectModel interactiveObject, ActorModel instigator);

		void OnAttacked(InteractiveObjectModel interactiveObject, ActorModel instigator);

		void OnDestroyed(InteractiveObjectModel interactiveObject, ActorModel instigator);
	}
}
