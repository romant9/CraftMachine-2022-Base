using BaseModel;

namespace TWDModel
{
	public class RenameCharacterCommand : ModelCommand
	{
		public const int MinLength = 1;

		public const int MaxLength = 12;

		public string NewName { get; set; }

		public RenameCharacterCommand()
		{
		}

		public RenameCharacterCommand(ActorModel actor, string newName)
			: base(actor)
		{
			NewName = newName;
		}

		public static bool IsNameValid(PlayerModel playerModel, string name)
		{
			if (name == null)
			{
				return false;
			}
			if (name.Length < 1 || name.Length > 12)
			{
				return false;
			}
			if (name.Contains(" "))
			{
				return false;
			}
			if (!playerModel.IsValidNameCharacters(name))
			{
				return false;
			}
			return true;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.GameEconomyData.ConfigData.CanRenameSurvivors && manager.GetModel<ActorModel>(base.ModelId) is SurvivorModel survivorModel && IsNameValid(tWDModelManager.Player, NewName))
			{
				survivorModel.SurvivorName = NewName;
				survivorModel.NotifyChange("SurvivorName");
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
