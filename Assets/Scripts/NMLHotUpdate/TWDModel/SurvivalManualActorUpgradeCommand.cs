using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class SurvivalManualActorUpgradeCommand : ConsumeCurrencyCommand
	{
		public enum UpgradeType
		{
			ActorUpgrade = 0,
			OneClickUpgrade = 1
		}

		public List<string> StoryActorIDs { get; set; }

		public new int ModelId { get; set; }

		public UpgradeType Upgrade { get; set; }

		public SurvivalManualActorUpgradeCommand()
		{
		}

		public SurvivalManualActorUpgradeCommand(int id, List<string> storyActorIDs, UpgradeType upgrade)
		{
			ModelId = id;
			StoryActorIDs = storyActorIDs;
			Upgrade = upgrade;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivalManualModel model = manager.GetModel<SurvivalManualModel>(ModelId);
			if (Upgrade == UpgradeType.ActorUpgrade)
			{
				TWDModelResult result = model.UpgradeActor(StoryActorIDs);
				return new NGModelCommandRespond(this, result);
			}
			TWDModelResult result2 = model.OneClickUpgradeActors(StoryActorIDs);
			return new NGModelCommandRespond(this, result2);
		}
	}
}
