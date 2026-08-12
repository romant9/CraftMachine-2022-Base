namespace TwdCustomMod
{
    public class User
    {
        public string HashID { get; set; }
        public string PlayerName { get; set; }
        public string FirstRun { get; set; }
        public string LastRun { get; set; }
        public int TimesRun { get; set; }
        public int TimesConnect { get; set; }
        public bool Regged { get; set; }
        public bool Blocked { get; set; }
        public bool ProGuild { get; set; }
        public bool ProLink { get; set; }
        public string GoogleId { get; set; }
        public string EpicId { get; set; }
        public string DeviceInfo { get; set; }
        public string GuilName { get; set; }
        public string GuildId { get; set; }
        public string Country { get; set; }
        public string Whishes { get; set; }
        public string Feedback { get; set; }
        public string ClientVersion { get; set; }
        public string ModVersion { get; set; }
        public long RegCode { get; set; }

        public string Linked_HashID { get; set; }

        public User() { }
    }

}

