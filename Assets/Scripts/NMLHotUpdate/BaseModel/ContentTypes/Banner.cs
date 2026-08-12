namespace BaseModel.ContentTypes
{
	public sealed class Banner : ContentTypeBase
	{
		public string ImageUrl { get; set; }

		public string NavigationLink { get; set; }

		public int ShowTimes { get; set; }

		public long SizeInBytes { get; set; }
	}
}
