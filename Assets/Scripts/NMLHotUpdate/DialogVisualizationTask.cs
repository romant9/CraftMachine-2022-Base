using System.Collections.Generic;

public class DialogVisualizationTask : VisualizationTask
{
	private string Tutoria1_6_0 = "Dialog,Tag_1051430701,dialog.mission_S01E01M06TerminusTwo.dialog_SurvivorCallsForHelp.line_01";

	private bool dialogStarted;

	private List<string> Actions { get; set; }

	public override bool IsGlobalBlocker => true;

	public DialogVisualizationTask(List<string> actions)
		: base(null)
	{
		Actions = actions;
	}

	public override void Start()
	{
		base.Start();
		Actions.Add("Dialog,Hide");
		if (TutorialView.Instance.PerformingActions && TutorialView.Instance.CurentPerformingAction == Tutoria1_6_0 && TutorialView.Instance != null && TutorialView.Instance.Running)
		{
			TutorialView.Instance.Stop();
		}
		TutorialView.Instance.StartCutScene(Actions);
	}

	public override bool Update(float deltaTime)
	{
		if (TutorialView.Instance.PerformingActions)
		{
			dialogStarted = true;
			return true;
		}
		if (dialogStarted)
		{
			return TutorialView.Instance.PerformingActions;
		}
		return true;
	}
}
