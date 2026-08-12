namespace TWDModel
{
	public class MissionObjective : TWDModelObject
	{
		public string Description { get; protected set; }

		public string CustomText1 { get; protected set; }

		public string CustomText2 { get; protected set; }

		public override bool IsValid()
		{
			return true;
		}

		public void SetDescription(string description, string customText1, string customText2, bool showObjectivesPopup)
		{
			Description = description;
			CustomText1 = customText1;
			CustomText2 = customText2;
			NotifyChange("Description", showObjectivesPopup);
		}

		public MissionObjective()
		{
		}

		public MissionObjective(MissionObjective objective)
		{
			Description = objective.Description;
			CustomText1 = objective.CustomText1;
			CustomText2 = objective.CustomText2;
		}

		public void Backup(MissionObjective objective)
		{
			Description = objective.Description;
			CustomText1 = objective.CustomText1;
			CustomText2 = objective.CustomText2;
		}
	}
}
