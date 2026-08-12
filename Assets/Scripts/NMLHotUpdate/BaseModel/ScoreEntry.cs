using System;
using System.Collections.Generic;

namespace BaseModel
{
	public class ScoreEntry
	{
		public int Position { get; set; }

		public string Nickname { get; set; }

		public long Score { get; set; }

		public int Level { get; set; }

		public string HashedId { get; set; }

		public List<string> Socials { get; set; }

		public string Country { get; set; }

		public string GroupId { get; set; }

		public string GroupName { get; set; }

		public DateTime LastActivityTime { get; set; }
	}
}
