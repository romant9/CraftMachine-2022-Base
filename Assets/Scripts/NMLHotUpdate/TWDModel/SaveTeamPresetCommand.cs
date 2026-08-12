using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class SaveTeamPresetCommand : ModelCommand
	{
		public int Index { get; set; }

		public int[] Survivors { get; set; }

		public string[] Supports { get; set; }

		public SaveTeamPresetCommand()
		{
		}

		public SaveTeamPresetCommand(int index, ITeamPresetData presetData)
		{
			Index = index;
			Survivors = presetData.Survivors.Select((SurvivorModel survivor) => survivor.ModelId).ToArray();
			Supports = presetData.Supports;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			TWDModelManager twdModelManager = manager as TWDModelManager;
			if (twdModelManager != null)
			{
				TeamTeamPreset teamTeamPreset = twdModelManager.Player.TeamPresetsManager.Presets[Index];
				teamTeamPreset.Survivors = Survivors.Select((int id) => twdModelManager.GetModel<SurvivorModel>(id)).ToArray();
				teamTeamPreset.Supports = Supports.ToArray();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
