using Postgrest.Attributes;
using System;

namespace Supabase.TWD
{
	[Table("cm_users")]
	public class CMUser : Postgrest.Models.BaseModel
	{
		[PrimaryKey("uid", false), Column("uid")] // Primary key column
		public string UID { get; set; }

		[Column("mail")] // Standard column
		public string Email { get; set; }

		[Column("user_name")] // Standard column
		public string UserName { get; set; }

		[Column("current_hash_id")] // Standard column
		public string HashID { get; set; }

		[Column("pin_hash_id")] // Standard column
		public string PinHashID { get; set; }

		[Column("epic_id")] // Standard column
		public string EpicID { get; set; }

		[Column("first_run")] // Standard column
		public DateTime FirstRun { get; set; }

		[Column("last_run")] // Standard column
		public DateTime? LastRun { get; set; }

		[Column("times_run")] // Standard column
		public int TimesRun { get; set; }

		[Column("times_connect")] // Standard column
		public int TimesConnect { get; set; }

		[Column("regged")] // Standard column
		public bool Regged { get; set; }

		[Column("blocked")] // Standard column
		public bool Blocked { get; set; }

		[Column("pro_guild")] // Standard column
		public bool ProGuild { get; set; }

		[Column("pro_link")] // Standard column
		public bool ProLink { get; set; }

		[Column("device_info")] // Standard column
		public string DeviceInfo { get; set; }

		[Column("country")] // Standard column
		public string Country { get; set; }

		[Column("wishes")] // Standard column
		public string Wishes { get; set; }

		[Column("feedback")] // Standard column
		public string Feedback { get; set; }

		[Column("client_version")] // Standard column
		public string ClientVersion { get; set; }

		[Column("mod_version")] // Standard column
		public string ModVersion { get; set; }

		[Column("reg_code")] // Standard column
		public long RegCode { get; set; }

		[Column("description")] // Standard column
		public string Description { get; set; }

		[Column("content")] // Standard column
		public string Content { get; set; }

		[Column("trial_count")] // Standard column
		public int TrialCount { get; set; }

		[Column("session_duration")] // Standard column
		public long? SessionDuration { get; set; }

		public CMUser() { }

		public CMUser(CMUserSerialized cmuser)
		{
			UID = cmuser.uid;
			Email = cmuser.mail;
			UserName = cmuser.user_name;
			HashID = cmuser.current_hash_id;
			PinHashID = cmuser.pin_hash_id;
			EpicID = cmuser.epic_id;
			FirstRun = cmuser.first_run;
			LastRun = cmuser.last_run;
			TimesRun = cmuser.times_run;
			TimesConnect = cmuser.times_connect;
			Regged = cmuser.regged;
			Blocked = cmuser.blocked;
			ProGuild = cmuser.pro_guild;
			ProLink = cmuser.pro_link;
			DeviceInfo = cmuser.device_info;
			Country = cmuser.country;
			Wishes = cmuser.wishes;
			Feedback = cmuser.feedback;
			ClientVersion = cmuser.client_version;
			ModVersion = cmuser.mod_version;
			RegCode = cmuser.reg_code;
			Description = cmuser.description;
			Content = cmuser.content;
			TrialCount = cmuser.trial_count;
			SessionDuration = cmuser.session_duration;
		}
	}
}
