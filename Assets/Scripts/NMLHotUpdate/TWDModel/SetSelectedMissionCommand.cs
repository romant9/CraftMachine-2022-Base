using BaseModel;

namespace TWDModel
{
	internal class SetSelectedMissionCommand : ModelCommand
	{
		public MapMissionParameters parameters { get; set; }

		public bool ShuffleLoot { get; set; }

		public SetSelectedMissionCommand()
		{
		}

		public SetSelectedMissionCommand(MapMissionParameters parameters)
		{
			this.parameters = parameters;
			ShuffleLoot = false;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			tWDModelManager.Player.SetSelectedMission(parameters);
			if (ShuffleLoot)
			{
				DropEventDefinition.DropEventType eventType = ((tWDModelManager.GameEconomyData.GetMissionData(parameters.MissionId).MissionType == MissionType.Rescue) ? DropEventDefinition.DropEventType.MissionRescue : DropEventDefinition.DropEventType.MissionScavenge);
				DropEventDefinition.DropEventContext context = (parameters.IsDeadly ? DropEventDefinition.DropEventContext.Deadly : DropEventDefinition.DropEventContext.Normal);
				tWDModelManager.Player.LootManager.ShuffleRewards(new LootEntryGenParams
				{
					eventType = eventType,
					targetLevel = parameters.MissionLevel,
					tag = parameters.LootTag,
					context = context
				});
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
