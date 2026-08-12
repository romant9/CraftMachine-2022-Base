using BaseModel;

namespace TWDModel
{
	public class SelectApocalypseBuffCommand : ModelCommand
	{
		public int selectIndex { get; set; }

		public SelectApocalypseBuffCommand()
		{
		}

		public SelectApocalypseBuffCommand(int index)
		{
			selectIndex = index;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.WeeklyChallenge != null)
			{
				TWDModelResult result = tWDModelManager.Player.ApocalypseWeeklyChallenge.SelectApocalypse(selectIndex);
				return new NGModelCommandRespond(this, result);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
