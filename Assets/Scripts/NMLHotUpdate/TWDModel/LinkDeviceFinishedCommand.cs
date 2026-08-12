using BaseModel;

namespace TWDModel
{
	public class LinkDeviceFinishedCommand : ModelCommand
	{
		public string OldPlayerId;

		public string NewPlayerId;

		public LinkDeviceFinishedCommand()
		{
		}

		public LinkDeviceFinishedCommand(string oldPlayerId, string newPlayerId)
		{
			OldPlayerId = oldPlayerId;
			NewPlayerId = newPlayerId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager)
			{
				manager.Debug.LogWarning($"LinkDeviceFinishedCommand: Linking old player [{OldPlayerId}] to current player [{NewPlayerId}]");
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
