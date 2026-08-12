namespace TWDModel
{
	public class LevelLoopingSoundModel : TWDModelObjectWithViewId, InteractionReceiver
	{
		public LoopingSoundPlayState LoopingSoundPlayState { get; private set; }

		public void SetLoopingSoundPlayState(LoopingSoundPlayState state)
		{
			LoopingSoundPlayState = state;
			NotifyChange("StateChanged");
		}

		public LevelLoopingSoundModel()
		{
		}

		public LevelLoopingSoundModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public void OnInteractionCompleted(InteractiveObjectModel model, ActorModel interactingActor)
		{
			SetLoopingSoundPlayState(LoopingSoundPlayState.Stopped);
		}

		public void OnInteractionCanceled(InteractiveObjectModel instigator, ActorModel interactingActor)
		{
		}

		public void OnInteractionStep(InteractiveObjectModel model, ActorModel interactingActor)
		{
		}

		public void OnAttacked(InteractiveObjectModel instigator, ActorModel attackingActor)
		{
		}

		public void OnDestroyed(InteractiveObjectModel instigator, ActorModel attackingActor)
		{
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
