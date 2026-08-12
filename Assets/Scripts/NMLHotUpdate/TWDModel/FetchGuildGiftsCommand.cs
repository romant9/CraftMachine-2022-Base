using BaseModel;

namespace TWDModel
{
	public class FetchGuildGiftsCommand : ModelCommand
	{
		public string GuildId { get; set; }

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			if (tWDModelManager.Player != null && tWDModelManager.GetGroupModel(GuildId) != null)
			{
				GuildModel guild = tWDModelManager.GetGroupModel(GuildId) as GuildModel;
				TWDModelResult result = tWDModelManager.Player.FetchGuildGifts(guild);
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player == null)
			{
				tWDModelManager.Debug.LogError("TWDModel Player is null");
			}
			if (tWDModelManager.GetGroupModel(GuildId) == null)
			{
				tWDModelManager.Debug.LogError("TWDModel could not find target guild " + GuildId.ToString());
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
