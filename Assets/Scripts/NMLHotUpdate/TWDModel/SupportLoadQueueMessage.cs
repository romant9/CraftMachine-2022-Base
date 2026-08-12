namespace TWDModel
{
	public abstract class SupportLoadQueueMessage : LoadQueueMessage
	{
		public long SupportGivenTimestamp { get; set; }

		public string SupportEntityGUID { get; set; }

		public SupportLoadQueueMessage()
		{
		}
	}
}
