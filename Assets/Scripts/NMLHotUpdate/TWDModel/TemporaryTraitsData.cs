namespace TWDModel
{
	public class TemporaryTraitsData
	{
		public string Identifier { get; set; }

		public TraitExpirationType Type { get; set; }

		public FixedPoint ConstructionMultiplier { get; set; }

		public TemporaryTraitsData()
		{
		}

		public TemporaryTraitsData(string id, TraitExpirationType type, FixedPoint multiplier)
		{
			Identifier = id;
			Type = type;
			ConstructionMultiplier = multiplier;
		}
	}
}
