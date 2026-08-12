using Postgrest.Attributes;
using System;

namespace Supabase.TWD
{
	[Table("cm_users")]
	public class CMUserSerialized : Postgrest.Models.BaseModel
	{
		[PrimaryKey("uid", false), Column("uid")]
		public string uid { get; set; }

		[Column("mail")]
		public string mail { get; set; }

		[Column("user_name")]
		public string user_name { get; set; }

		[Column("current_hash_id")]
		public string current_hash_id { get; set; }

		[Column("pin_hash_id")]
		public string pin_hash_id { get; set; }

		[Column("epic_id")]
		public string epic_id { get; set; }

		[Column("first_run")]
		public DateTime first_run { get; set; }

		[Column("last_run")]
		public DateTime? last_run { get; set; }

		[Column("times_run")]
		public int times_run { get; set; }

		[Column("times_connect")]
		public int times_connect { get; set; }

		[Column("regged")]
		public bool regged { get; set; }

		[Column("blocked")]
		public bool blocked { get; set; }

		[Column("pro_guild")]
		public bool pro_guild { get; set; }

		[Column("pro_link")]
		public bool pro_link { get; set; }

		[Column("device_info")]
		public string device_info { get; set; }

		[Column("country")]
		public string country { get; set; }

		[Column("wishes")]
		public string wishes { get; set; }

		[Column("feedback")]
		public string feedback { get; set; }

		[Column("client_version")]
		public string client_version { get; set; }

		[Column("mod_version")]
		public string mod_version { get; set; }

		[Column("reg_code")]
		public long reg_code { get; set; }

		[Column("description")]
		public string description { get; set; }

		[Column("content")]
		public string content { get; set; }

		[Column("trial_count")]
		public int trial_count { get; set; }

		[Column("session_duration")]
		public long? session_duration { get; set; }

		public CMUserSerialized() { }

		public CMUserSerialized(CMUser cmuser)
		{
			uid = cmuser.UID;
			mail = cmuser.Email;
			user_name = cmuser.UserName;
			current_hash_id = cmuser.HashID;
			pin_hash_id = cmuser.PinHashID;
			epic_id = cmuser.EpicID;
			first_run = cmuser.FirstRun;
			last_run = cmuser.LastRun;
			times_run = cmuser.TimesRun;
			times_connect = cmuser.TimesConnect;
			regged = cmuser.Regged;
			blocked = cmuser.Blocked;
			pro_guild = cmuser.ProGuild;
			pro_link = cmuser.ProLink;
			device_info = cmuser.DeviceInfo;
			country = cmuser.Country;
			wishes = cmuser.Wishes;
			feedback = cmuser.Feedback;
			client_version = cmuser.ClientVersion;
			mod_version = cmuser.ModVersion;
			reg_code = cmuser.RegCode;
			description = cmuser.Description;
			content = cmuser.Content;
			trial_count = cmuser.TrialCount;
			session_duration = cmuser.SessionDuration;
		}
	}
}
