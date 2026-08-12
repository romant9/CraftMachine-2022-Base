namespace TWDModel
{
	public class TWDGuildMigration
	{
		public string Version { get; set; }

		public GameVersion GameVersion => new GameVersion(Version);

		public virtual bool Migrate(GuildModel guild, TWDModelManager manager)
		{
			guild.Version = Version;
			return true;
		}

		protected bool InitializeProperty(object instance, string propertyName, object defaultValue)
		{
			object propertyValue = GetPropertyValue(instance, propertyName);
			if (propertyValue == null || propertyValue.Equals(0))
			{
				SetPropertyValue(instance, propertyName, defaultValue);
				return true;
			}
			return false;
		}

		protected object GetPropertyValue(object instance, string propertyName)
		{
			return instance.GetType().GetProperty(propertyName).GetValue(instance, null);
		}

		protected void SetPropertyValue(object instance, string propertyName, object propertyValue)
		{
			instance.GetType().GetProperty(propertyName).SetValue(instance, propertyValue, null);
		}
	}
}
