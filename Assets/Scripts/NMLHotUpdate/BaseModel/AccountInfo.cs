using System.Collections.Generic;

namespace BaseModel
{
	public sealed class AccountInfo
	{
		public string AccountId { get; set; }

		public AccountType Type { get; set; }

		public Dictionary<string, string> Data { get; set; }
	}
}
