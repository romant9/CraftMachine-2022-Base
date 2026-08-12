using BaseModel;

namespace TWDModel
{
	public class MarkGuildSuggestionPopupShownCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel obj = manager.GetPlayer() as PlayerModel;
			obj.GuildSuggestionPopupShownCount++;
			obj.GuildSuggestionPopupLastShownTime = obj.UtcTimeStamp;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
