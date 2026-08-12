using BaseModel;

namespace TWDModel
{
	public class SpawnGrindMissionCommand : ModelCommand
	{
		public DropEventDefinition.DropEventTag LootTag;

		public int MissionLevel;

		public int GrindButtonDefinitionId;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager as TWDModelManager).Player.MapContainerModel.SpawnGrindMission(GrindButtonDefinitionId))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
