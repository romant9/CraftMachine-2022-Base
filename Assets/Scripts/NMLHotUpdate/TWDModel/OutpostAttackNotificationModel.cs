namespace TWDModel
{
	public class OutpostAttackNotificationModel : TWDModelObject
	{
		public string PlayerName { get; set; }

		public int Level { get; set; }

		public override bool IsValid()
		{
			return true;
		}
	}
}
