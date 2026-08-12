using BaseModel.ContentTypes;

namespace TWDModel.ContentTypes
{
	public sealed class EpisodeVideo : ContentTypeBase
	{
		public string AssetId { get; set; }

		public string SourceFilename { get; set; }

		public string EpisodeNumber { get; set; }

		public double LengthMilliseconds { get; set; }

		public string ManifestUri { get; set; }

		public string ManifestSmoothUri { get; set; }

		public string ManifestMpegDashUri { get; set; }

		public string ThumbnailUri { get; set; }
	}
}
