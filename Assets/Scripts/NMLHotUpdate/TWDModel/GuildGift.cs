using System.Collections.Generic;

namespace TWDModel
{
	public class GuildGift : TWDModelObject
	{
		public string Id { get; set; }

		public DropType Type { get; set; }

		public long Creationtime { get; set; }

		public long ExpireTime { get; set; }

		public string GuildId { get; set; }

		public string SenderName { get; set; }

		public string SenderId { get; set; }

		public string SenderMessage { get; set; }

		public List<string> Recipients { get; set; }

		public bool Claimed { get; set; }

		public override bool IsValid()
		{
			return true;
		}
	}
}
