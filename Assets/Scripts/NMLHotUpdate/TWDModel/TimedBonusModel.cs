namespace TWDModel
{
	public class TimedBonusModel : TWDModelObject
	{
		public TimedBonusType TimedBonusTypeType { get; set; }

		public long MillisecondsTillCompletion { get; protected set; }

		public bool IsActive => MillisecondsTillCompletion > 0;

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (MillisecondsTillCompletion > 0)
			{
				MillisecondsTillCompletion -= deltaTime;
				DoInUpdate();
				if (MillisecondsTillCompletion <= 0)
				{
					OnBonusEnded();
					base.manager.Player.NotifyChange("currencyChangedEvent");
				}
			}
		}

		protected virtual void DoInUpdate()
		{
		}

		protected virtual void OnBonusEnded()
		{
		}

		public virtual TWDModelResult SetDuration(FixedPoint duration)
		{
			if (MillisecondsTillCompletion < 0)
			{
				MillisecondsTillCompletion = 0L;
			}
			MillisecondsTillCompletion += (long)duration;
			base.manager.Player.NotifyChange("currencyChangedEvent");
			return TWDModelResult.OK;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
