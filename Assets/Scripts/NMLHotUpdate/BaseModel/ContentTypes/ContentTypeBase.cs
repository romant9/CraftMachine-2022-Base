namespace BaseModel.ContentTypes
{
	public abstract class ContentTypeBase
	{
		public string EntryId { get; set; }

		public string Path { get; set; }

		public long StartUnixTime { get; set; }

		public long EndUnixTime { get; set; }

		public int OrderNumber { get; set; }
	}
}
