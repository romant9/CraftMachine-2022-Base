using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class LoadNewAccountCommand : ModelCommand
	{
		public string Type { get; set; }

		public string UserId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager obj = manager as TWDModelManager;
			int level = obj.Player.Camp.GetBuilding("Council").Level;
			int level2 = obj.Player.Level;
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"council_level",
					level.ToString()
				},
				{
					"player_level",
					level2.ToString()
				},
				{ "phase", "load_old_town" },
				{ "type", Type },
				{ "restored_player_id", UserId }
			};
			TutorialModel tutorial = obj.Player.Tutorial;
			if (tutorial != null)
			{
				dictionary.Add("tutorialCompleted", tutorial.StaticTutorialComplete.ToString());
				if (tutorial.StaticTutorialComplete)
				{
					dictionary.Add("tutorialPartId", "None");
					dictionary.Add("tutorialStep", "0");
				}
				else
				{
					dictionary.Add("tutorialPartId", tutorial.CurrentPartId);
					dictionary.Add("tutorialStep", tutorial.CurrentStep.ToString());
				}
			}
			obj.SendMetricsEvent("progress_report", dictionary);
			obj.TdMetrics.SetEventType("progress_report").AddProperty("type", Type).AddProperty("restored_player_id", UserId)
				.Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
