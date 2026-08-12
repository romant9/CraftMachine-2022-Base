using BaseModel;

namespace TWDModel
{
	public class StopEditingOutpostCommand : ModelCommand
	{
		public enum ActionType
		{
			Save = 0,
			Discard = 1
		}

		public ActionType Action { get; set; }

		public StopEditingOutpostCommand()
		{
		}

		public StopEditingOutpostCommand(ActionType action)
		{
			Action = action;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: var player } && player.OutpostModel != null)
			{
				if (Action == ActionType.Save)
				{
					player.OutpostModel.SaveEditModel();
				}
				else if (Action == ActionType.Discard)
				{
					player.OutpostModel.DiscardEditModel();
				}
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
