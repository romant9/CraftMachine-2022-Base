namespace TWDModel
{
	public class DoubleXpTimedBonusModelModel : TimedBonusModel
	{
		public override void Start()
		{
			base.Start();
			base.TimedBonusTypeType = TimedBonusType.DoubleXp;
			base.manager.Player.RefreshSurvivalPointsAddMultiplier();
		}

		public override TWDModelResult SetDuration(FixedPoint duration)
		{
			TWDModelResult result = base.SetDuration(duration);
			base.manager.Player.RefreshSurvivalPointsAddMultiplier();
			return result;
		}

		protected override void OnBonusEnded()
		{
			base.OnBonusEnded();
			base.manager.Player.RefreshSurvivalPointsAddMultiplier();
		}
	}
}
