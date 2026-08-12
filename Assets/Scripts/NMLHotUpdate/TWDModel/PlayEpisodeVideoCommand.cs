using BaseModel;

namespace TWDModel
{
	public class PlayEpisodeVideoCommand : ModelCommand
	{
		public string EpisodeId { get; set; }

		public PlayEpisodeVideoCommand()
		{
		}

		public PlayEpisodeVideoCommand(PlayerModel player)
			: base(player)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (string.IsNullOrEmpty(EpisodeId))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.Blackboard.SetToggle(BlackboardModel.GetEpisodeVideoWatchedKey(EpisodeId));
			tWDModelManager.Metrics.AddStart().AddSeasonVideo(EpisodeId).Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
