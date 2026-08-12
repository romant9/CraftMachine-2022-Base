using System;

namespace BaseModel.ContentTypes
{
	[Flags]
	public enum ContentViewerFlags
	{
		EarthMap = 1,
		ImageGallery = 2,
		Json = 4,
		JsonDiff = 8,
		PlainText = 0x10,
		VideoGallery = 0x20
	}
}
