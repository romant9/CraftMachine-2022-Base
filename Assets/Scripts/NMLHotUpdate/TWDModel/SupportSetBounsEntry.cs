namespace TWDModel
{
	public class SupportSetBounsEntry
	{
		public int ItemId { get; set; }

		public int Level { get; set; }

		public SupportSetBounsEntry(int itemId, int level)
		{
			ItemId = itemId;
			Level = level;
		}
	}
}
