namespace TWDModel
{
	public class FixInfluenceScoreLoadQueueMessage : SupportLoadQueueMessage
	{
		public int InfluenceScore { get; set; }

		public FixInfluenceScoreLoadQueueMessage()
		{
		}

		public FixInfluenceScoreLoadQueueMessage(int score)
		{
			InfluenceScore = score;
		}

		public override bool Execute(TWDModelManager manager)
		{
			manager.Player.RankingScore = InfluenceScore;
			manager.Metrics.AddInflueceFixed(InfluenceScore).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			return true;
		}
	}
}
