using System;

namespace BaseModel
{
	[Serializable]
	public sealed class LoadContentRequest
	{
		public string TransactionId;

		public string ContentPath;

		public string Checksum;

		public LoadMode Mode;

		public int ResultThreshold;
	}
}
