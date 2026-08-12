using System;

namespace BaseModel.ContentTypes
{
	public sealed class ContentType
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public CdnContentTypeParse Parse { get; set; }

		public Type ConcreteType { get; set; }

		public CdnContentDataSourceKind DataSourceKind { get; set; }

		public string HttpContentEncoding { get; set; }

		public ContentViewerFlags ContentViewerFlags { get; set; }

		public bool DeduplicateByPath { get; set; }
	}
}
