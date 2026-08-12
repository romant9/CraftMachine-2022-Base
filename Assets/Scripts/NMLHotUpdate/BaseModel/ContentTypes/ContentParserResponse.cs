using System.Collections.Generic;

namespace BaseModel.ContentTypes
{
	public sealed class ContentParserResponse
	{
		public byte[] Content;

		public string Filename;

		public string HttpContentType;

		public string Path;

		public List<string> Errors;

		public List<string> Warnings;
	}
}
