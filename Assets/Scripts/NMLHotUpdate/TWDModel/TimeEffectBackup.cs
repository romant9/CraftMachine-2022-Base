using BaseModel;

namespace TWDModel
{
	public class TimeEffectBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public TimedEffect Model { get; set; }

		public int Duration { get; set; }

		public int Counter { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(TimedEffect model)
		{
			Model = model;
			Duration = model.Duration;
			Counter = model.Counter;
		}

		public virtual void BackUp()
		{
			Model.Duration = Duration;
			Model.Counter = Counter;
		}
	}
}
