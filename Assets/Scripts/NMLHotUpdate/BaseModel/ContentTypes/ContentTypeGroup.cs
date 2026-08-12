namespace BaseModel.ContentTypes
{
	public class ContentTypeGroup
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public ContentType[] ContentTypes { get; set; }
	}
}
