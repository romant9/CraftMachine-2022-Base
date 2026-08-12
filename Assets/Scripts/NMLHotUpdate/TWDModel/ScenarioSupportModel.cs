namespace TWDModel
{
	public class ScenarioSupportModel : TWDModelObject
	{
		public string SupportId { get; set; }

		public int Level { get; set; }

		public int EquippedIndex { get; set; }

		public ScenarioSupportModel()
		{
		}

		public ScenarioSupportModel(string supportId, int level, int equippedIndex)
		{
			SupportId = supportId;
			Level = level;
			EquippedIndex = equippedIndex;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
