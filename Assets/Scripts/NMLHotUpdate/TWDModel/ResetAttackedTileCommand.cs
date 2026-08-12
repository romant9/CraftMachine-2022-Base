using BaseModel;

namespace TWDModel
{
	public class ResetAttackedTileCommand : ModelCommand
	{
		public ResetAttackedTileCommand()
		{
		}

		public ResetAttackedTileCommand(MapContainerModel map)
			: base(map)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			bool num = manager.GetModel<MapContainerModel>(base.ModelId).MissionCompleted();
			TWDModelResult result = TWDModelResult.OK;
			if (!num)
			{
				manager.Debug.LogWarning("Ignored duplicate ResetAttackedTileCommand for the map.");
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
