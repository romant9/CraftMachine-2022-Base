namespace TWDModel
{
	public class AnimatedPropModel : TWDModelObjectWithViewId, TriggerReceiver
	{
		public string AnimationId { get; protected set; }

		public float AnimationSpeed { get; protected set; }

		public bool IsOpen { get; private set; }

		public AnimatedPropModel()
		{
		}

		public AnimatedPropModel(string viewId)
		{
			base.ViewId = viewId;
			AnimationSpeed = 1f;
		}

		public void Animate(ActorModel instigator, string animationId, float animationSpeed)
		{
			AnimationId = animationId;
			AnimationSpeed = animationSpeed;
			NotifyChange("Animate");
		}

		public void OnTriggered(ActorModel instigator)
		{
			Animate(instigator, AnimationId, AnimationSpeed);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
