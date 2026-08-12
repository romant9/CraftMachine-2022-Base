using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class TeamPresetsManager : TWDModelObject
	{
		public List<TeamTeamPreset> Presets { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public TeamPresetsManager()
		{
			Presets = new List<TeamTeamPreset>();
		}

		public override void SetManager(ModelManager mgr)
		{
			base.SetManager(mgr);
			while (Presets.Count < 6)
			{
				Presets.Add(new TeamTeamPreset());
			}
		}
	}
}
