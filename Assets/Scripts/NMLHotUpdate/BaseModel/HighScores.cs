using System.Collections.Generic;

namespace BaseModel
{
	public class HighScores
	{
		public string NextPartitionKey { get; set; }

		public string NextRowKey { get; set; }

		public string NextTableName { get; set; }

		public int ScoresShown { get; set; }

		public List<ScoreEntry> Scores { get; set; }

		public HighScores()
		{
			Scores = new List<ScoreEntry>();
		}
	}
}
