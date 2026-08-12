namespace TWDModel
{
	public class SurvivalManualActorStoryNode : TWDModelObject
	{
		public string StoryActorID { get; set; }

		public string LinkActorID { get; set; }

		public StoryUnlockStatus Status { get; set; }

		public int MemoryID { get; set; }

		public override bool IsValid()
		{
			return true;
		}
	}
}
