namespace Pocketbase.TWD
{
    [System.Serializable]
    public class CMUser
    {
        // Системные поля PocketBase
        public string id;        // Уникальный ID самой записи в cm_users
        public string created;   // Дата создания (заменяет FirstRun)
        public string updated;   // Дата последнего обновления (заменяет LastRun)

        // Связь с аккаунтом
        public string userID;       // Ссылка на collections.users.id

        // Ваши пользовательские поля
        public string Email;
        public string UserName;
        public string HashID;
        public string PinHashID;
        public string EpicID;

        public int TimesRun;
        public int TimesConnect;
        public bool Regged;
        public bool Blocked;
        public bool ProGuild;
        public bool ProLink;
        public string DeviceInfo;
        public string Country;
        public string Wishes;
        public string Feedback;
        public string ClientVersion;
        public string ModVersion;
        public long RegCode;
        public string Description;
        public string Content;
        public int TrialCount;
        public long SessionDuration;

        public CMUser() { }
    }
}
