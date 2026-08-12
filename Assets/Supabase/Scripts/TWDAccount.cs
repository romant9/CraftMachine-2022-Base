using Postgrest.Attributes;
using System;

namespace Supabase.TWD
{
	[Table("twd_accounts")] // Name of the table in Postgres
	public class TWDAccount : Postgrest.Models.BaseModel
	{
		[PrimaryKey("hash_id", false)] // Primary key column
		public string HashID { get; set; }

		[Column("player_name")] // Standard column
		public string PlayerName { get; set; }

		[Column("player_level")] // Standard column
		public int PlayerLevel { get; set; }

		[Column("google_id")] // Standard column
		public string GoogleID { get; set; }

		[Column("guild_id")] // Standard column
		public string GuildID { get; set; }

		[Column("guild_name")] // Standard column
		public string GuildName { get; set; }

		[Column("last_used")] // Standard column
		public DateTime LastUsed { get; set; }

		[Column("times_used")] // Standard column
		public int TimesUsed { get; set; }

		[Column("uid_current")] // Standard column
		public string UID_Linked { get; set; }

		public TWDAccount() { }
	}
}
