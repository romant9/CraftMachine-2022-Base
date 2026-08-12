using BaseModel;

namespace TWDModel
{
	public class SeasonEpisodeSeenCommand : ModelCommand
	{
		public string MapId { get; private set; }

		public SeasonEpisodeSeenCommand()
		{
		}

		public SeasonEpisodeSeenCommand(string mapId)
		{
			MapId = mapId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager && !string.IsNullOrEmpty(MapId))
			{
				if (tWDModelManager.GameEconomyData.GetMapDefinitionById(MapId) == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				tWDModelManager.Player.Blackboard.SetToggle("Toggle.Episode." + MapId + ".Seen");
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
