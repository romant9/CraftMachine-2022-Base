namespace TWDModel
{
	public class NoiseObjectModel : TWDSpatialModelObject, InteractionReceiver, TriggerReceiver
	{
		public int NoiseRange { get; set; }

		public int ThreatValue { get; set; }

		public NoiseObjectModel()
		{
		}

		public NoiseObjectModel(string viewId, GridCoordinate coordinate, int noiseRange, int threatValue)
		{
			base.ViewId = viewId;
			base.Location = new TWDObjectLocation(coordinate);
			NoiseRange = noiseRange;
			ThreatValue = threatValue;
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			CreateNoise(instigator);
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnDestroyed(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnTriggered(ActorModel instigator)
		{
			CreateNoise(instigator);
		}

		public override bool IsValid()
		{
			return true;
		}

		private void CreateNoise(ActorModel actor)
		{
			base.manager.ExecuteAction(new NoiseAction(actor, base.Location.Coordinate, NoiseRange));
			if (ThreatValue != 0)
			{
				base.manager.ExecuteAction(new ThreatAction(actor, ThreatValue));
			}
		}
	}
}
