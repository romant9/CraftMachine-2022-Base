namespace TWDModel
{
	public struct DamageNotificationData
	{
		public string TraitIdentifier;

		public bool DueLuck;

		public DamageNotificationData(string traitIdentifier, bool dueLuck)
		{
			TraitIdentifier = traitIdentifier;
			DueLuck = dueLuck;
		}
	}
}
