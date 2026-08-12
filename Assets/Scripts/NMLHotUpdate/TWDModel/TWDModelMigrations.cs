using System.Collections.Generic;

namespace TWDModel
{
	public class TWDModelMigrations
	{
		protected TWDModelManager Manager { get; set; }

		protected List<TWDModelMigration> Migrations { get; set; }

		public TWDModelMigrations(TWDModelManager manager)
		{
			Manager = manager;
			Migrations = new List<TWDModelMigration>();
			Migrations.Add(new Migration080());
			Migrations.Add(new Migration082());
			Migrations.Add(new Migration090());
			Migrations.Add(new Migration100());
			Migrations.Add(new Migration110());
			Migrations.Add(new Migration120());
			Migrations.Add(new Migration130());
			Migrations.Add(new Migration140());
			Migrations.Add(new Migration150());
			Migrations.Add(new Migration160());
			Migrations.Add(new Migration162());
			Migrations.Add(new Migration170());
			Migrations.Add(new Migration171());
			Migrations.Add(new Migration180());
			Migrations.Add(new Migration182());
			Migrations.Add(new Migration190());
			Migrations.Add(new Migration1100());
			Migrations.Add(new Migration200());
			Migrations.Add(new Migration210());
			Migrations.Add(new Migration220());
			Migrations.Add(new Migration230());
			Migrations.Add(new Migration240());
			Migrations.Add(new Migration250());
			Migrations.Add(new Migration260());
			Migrations.Add(new Migration270());
			Migrations.Add(new Migration280());
			Migrations.Add(new Migration290());
			Migrations.Add(new Migration2100());
			Migrations.Add(new Migration2110());
			Migrations.Add(new Migration2120());
			Migrations.Add(new Migration3000());
			Migrations.Add(new Migration3100());
			Migrations.Add(new Migration3200());
			Migrations.Add(new Migration3300());
			Migrations.Add(new Migration3400());
			Migrations.Add(new Migration3500());
			Migrations.Add(new Migration3600());
			Migrations.Add(new Migration3700());
			Migrations.Add(new Migration3800());
			Migrations.Add(new Migration3900());
			Migrations.Add(new Migration31000());
			Migrations.Add(new Migration31100());
			Migrations.Add(new Migration31200());
			Migrations.Add(new Migration31300());
			Migrations.Add(new Migration31400());
			Migrations.Add(new Migration31500());
			Migrations.Add(new Migration31600());
			Migrations.Add(new Migration31700());
			Migrations.Add(new Migration31800());
			Migrations.Add(new Migration400());
			Migrations.Add(new Migration410());
			Migrations.Add(new Migration420());
			Migrations.Add(new Migration430());
			Migrations.Add(new Migration440());
			Migrations.Add(new Migration450());
			Migrations.Add(new Migration460());
			Migrations.Add(new Migration470());
			Migrations.Add(new Migration480());
			Migrations.Add(new Migration490());
			Migrations.Add(new Migration500());
			Migrations.Add(new Migration510());
			Migrations.Add(new Migration520());
			Migrations.Add(new Migration530());
			Migrations.Add(new Migration540());
			Migrations.Add(new Migration550());
			Migrations.Add(new Migration560());
			Migrations.Add(new Migration570());
			Migrations.Add(new Migration580());
			Migrations.Add(new Migration590());
			Migrations.Add(new Migration600());
			Migrations.Add(new Migration610());
			Migrations.Add(new Migration611());
			Migrations.Add(new Migration620());
			Migrations.Add(new Migration630());
			Migrations.Add(new Migration640());
			Migrations.Add(new Migration650());
			Migrations.Add(new Migration660());
			Migrations.Add(new Migration670());
			Migrations.Add(new Migration680());
			Migrations.Add(new Migration690());
			Migrations.Add(new Migration6100());
			Migrations.Add(new Migration6110());
			Migrations.Add(new Migration6120());
			Migrations.Add(new Migration6130());
			Migrations.Add(new Migration6140());
			Migrations.Add(new Migration6150());
			Migrations.Add(new Migration6160());
			Migrations.Add(new Migration6170());
			Migrations.Add(new Migration6180());
			Migrations.Add(new Migration6190());
			Migrations.Add(new Migration6200());
			Migrations.Add(new Migration6210());
			Migrations.Add(new Migration6220());
			Migrations.Add(new Migration6230());
			Migrations.Add(new Migration6240());
			Migrations.Add(new Migration6250());
			Migrations.Add(new Migration6260());
			Migrations.Add(new Migration6270());
			Migrations.Add(new Migration6280());
			Migrations.Add(new Migration6290());
			Migrations.Add(new Migration6300());
			Migrations.Add(new Migration6310());
			Migrations.Add(new Migration6320());
			Migrations.Add(new Migration700());
			Migrations.Add(new Migration710());
			Migrations.Add(new Migration720());
			Migrations.Add(new Migration730());
			Migrations.Add(new Migration740());
			Migrations.Add(new Migration750());
			Migrations.Add(new Migration760());
			Migrations.Add(new Migration770());
			Migrations.Add(new Migration780());
			Migrations.Add(new Migration790());
			Migrations.Add(new Migration7100());
			Migrations.Add(new Migration7110());
			Migrations.Add(new Migration7120());
			Migrations.Add(new Migration7130());
			Migrations.Add(new Migration7140());
			Migrations.Add(new Migration7150());
			Migrations.Add(new Migration7160());
			Migrations.Add(new Migration7170());
			Migrations.Add(new Migration7180());
			Migrations.Add(new Migration7190());
			Migrations.Add(new Migration7200());
		}

		public bool Migrate(PlayerModel player)
		{
			GameVersion gameVersion = new GameVersion(player.Version);
			player.SetManager(Manager);
			for (int i = 0; i < Migrations.Count; i++)
			{
				TWDModelMigration tWDModelMigration = Migrations[i];
				if (gameVersion.CompareTo(tWDModelMigration.GameVersion) < 0)
				{
					bool flag = tWDModelMigration.Migrate(player, Manager);
					Manager.Debug.Log("Migration to:" + tWDModelMigration.GameVersion?.ToString() + ":" + flag);
					if (!flag)
					{
						Manager.Debug.LogError("Failed to apply migration " + tWDModelMigration.Version);
					}
					gameVersion = new GameVersion(player.Version);
				}
			}
			GameVersion other = new GameVersion(Manager.GetVersion());
			if (gameVersion.CompareTo(other) < 0)
			{
				player.Version = Manager.GetVersion();
			}
			OutpostVersionMigration.Migrate(player, Manager);
			return true;
		}

		public bool MigrateDev(PlayerModel player)
		{
			GameVersion gameVersion = new GameVersion(player.Version);
			player.SetManager(Manager);
			for (int i = 0; i < Migrations.Count; i++)
			{
				TWDModelMigration tWDModelMigration = Migrations[i];
				if (gameVersion.CompareTo(tWDModelMigration.GameVersion) == 0)
				{
					bool flag = tWDModelMigration.Migrate(player, Manager);
					Manager.Debug.Log("Migration to:" + tWDModelMigration.GameVersion?.ToString() + ":" + flag);
					if (!flag)
					{
						Manager.Debug.LogError("Failed to apply migration " + tWDModelMigration.Version);
					}
					gameVersion = new GameVersion(player.Version);
				}
			}
			GameVersion other = new GameVersion(Manager.GetVersion());
			if (gameVersion.CompareTo(other) < 0)
			{
				player.Version = Manager.GetVersion();
			}
			OutpostVersionMigration.Migrate(player, Manager);
			return true;
		}
	}
}
