using System.Collections.Generic;

namespace TWDModel
{
	public class TWDGuildMigrations
	{
		protected TWDModelManager Manager { get; set; }

		protected List<TWDGuildMigration> Migrations { get; set; }

		public TWDGuildMigrations(TWDModelManager manager)
		{
			Manager = manager;
			Migrations = new List<TWDGuildMigration>();
			Migrations.Add(new GuildMigration170());
			Migrations.Add(new GuildMigration200());
			Migrations.Add(new GuildMigration250());
			Migrations.Add(new GuildMigration3100());
			Migrations.Add(new GuildMigration4700());
			Migrations.Add(new GuildMigration6000());
			Migrations.Add(new GuildMigration6200());
		}

		public bool Migrate(GuildModel guild)
		{
			GameVersion gameVersion = new GameVersion(guild.Version);
			for (int i = 0; i < Migrations.Count; i++)
			{
				TWDGuildMigration tWDGuildMigration = Migrations[i];
				if (gameVersion.CompareTo(tWDGuildMigration.GameVersion) < 0)
				{
					if (!tWDGuildMigration.Migrate(guild, Manager))
					{
						return false;
					}
					gameVersion = new GameVersion(guild.Version);
				}
			}
			GameVersion other = new GameVersion(Manager.GetVersion());
			if (gameVersion.CompareTo(other) < 0)
			{
				guild.Version = Manager.GetVersion();
			}
			return true;
		}
	}
}
