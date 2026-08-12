using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class SendPlayerInputMetricsCommand : ModelCommand
	{
		public PlayerInputMetricsData data;

		public string LevelName;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.Analytics != null)
			{
				Dictionary<string, string> dictionary = data.ToDictionary();
				dictionary.Add("Level", LevelName);
				tWDModelManager.Analytics.CreateEvent("PlayerInputMetrics", dictionary);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
