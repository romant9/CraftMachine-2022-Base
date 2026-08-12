namespace TWDModel
{
	public class SurvivorController : AIController
	{
		public override bool CanPerformOOT => true;

		public SurvivorController(ActorModel actor)
			: base(actor)
		{
			base.AIDataModel.Alertness = AIAlertness.Aggressive;
		}
	}
}
