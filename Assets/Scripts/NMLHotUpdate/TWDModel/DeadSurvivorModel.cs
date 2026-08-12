using BaseModel;

namespace TWDModel
{
	public class DeadSurvivorModel : TWDModelObject, IUserViewableObject
	{
		private bool deadSurvivorNotificationViewed;

		public static string DeadSurvivorNotificationViewedEvent = "DeadSurvivorNotificationViewedEvent";

		public string Name { get; private set; }

		public SurvivorModel SurvivorModel { get; set; }

		public bool DeadSurvivorNotificationViewed
		{
			get
			{
				return deadSurvivorNotificationViewed;
			}
			set
			{
				if (deadSurvivorNotificationViewed != value)
				{
					deadSurvivorNotificationViewed = value;
					NotifyChange(DeadSurvivorNotificationViewedEvent);
				}
			}
		}

		[IgnoreModelProperty]
		public SurvivorStatistics Statistics { get; private set; }

		public void OnObjectViewedByUser()
		{
			DeadSurvivorNotificationViewed = true;
		}

		public override bool IsValid()
		{
			return Statistics != null;
		}

		public void SetDeadSurvivor(SurvivorModel survivorModel)
		{
			Name = survivorModel.Name;
			Statistics = survivorModel.Statistics;
			Statistics.SurvivorDied();
			SurvivorModel = survivorModel;
			SurvivorModel.ClearModelObjectReferences();
		}
	}
}
