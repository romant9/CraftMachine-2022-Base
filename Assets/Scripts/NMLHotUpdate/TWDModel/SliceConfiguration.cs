namespace TWDModel
{
	public class SliceConfiguration
	{
		public SlicePosition Position;

		public string ViewId;

		public SliceConfiguration()
		{
		}

		public SliceConfiguration(SlicePosition position, string viewId)
		{
			Position = position;
			ViewId = viewId;
		}
	}
}
