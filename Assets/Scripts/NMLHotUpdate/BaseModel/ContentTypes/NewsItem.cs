namespace BaseModel.ContentTypes
{
	public class NewsItem : ContentTypeBase
	{
		public string ParentEntryId { get; set; }

		public string Title { get; set; }

		public string Abstract { get; set; }

		public string Category { get; set; }

		public string LanguageCode { get; set; }

		public string Content { get; set; }

		public string ImageUrl { get; set; }

		public long ImageSizeInBytes { get; set; }

		public string ThumbnailUrl { get; set; }

		public long ThumbnailSizeInBytes { get; set; }

		public string NavigationLink { get; set; }

		public string PromoAttributes { get; set; }

		public bool ShowCounter { get; set; }
	}
}
