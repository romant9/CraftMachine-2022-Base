using System;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleMissionConfig
	{
		public string ConfigName;

		public string Objectives;

		public string Enemies;

		public static string GetGroupKey(string columnName, string configName)
		{
			return configName + "_" + columnName;
		}
	}
}
