namespace TwdCustomMod
{
	public class PlayersIDData
	{
		public string PlayerName { get; set; }
		public string GameID { get; set; }
		public string EosID { get; set; }
		public int Level { get; set; }

		public PlayersIDData(string name, string gameID, string eosID, int level)
		{
			PlayerName = name;
			GameID = gameID;
			EosID = eosID;
			Level = level;
		}
	}
}
