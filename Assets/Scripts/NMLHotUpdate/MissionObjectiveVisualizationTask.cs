using System.Collections.Generic;
using TWDModel;

public class MissionObjectiveVisualizationTask : VisualizationTask
{
	private MissionObjective MissionObjective { get; set; }

	private MissionObjectiveView MissionObjectiveView { get; set; }

	private List<CombatExitView> CombatExitViews { get; set; }

	private bool ViewsUpdated { get; set; }

	private bool ActionCameraStarted { get; set; }

	private bool ShowObjectivesPopup { get; set; }

	public MissionObjectiveVisualizationTask(MissionObjective missionObjective, List<TWDModelObject> combatExitModels, bool showObjectivesPopup)
		: base(null)
	{
		ShowObjectivesPopup = showObjectivesPopup;
		MissionObjective = missionObjective;
		MissionObjectiveView = GameManager.Instance.GetViewForModel(MissionObjective) as MissionObjectiveView;
		CombatExitViews = new List<CombatExitView>();
		for (int i = 0; i < combatExitModels.Count; i++)
		{
			CombatExitView combatExitView = GameManager.Instance.GetViewForModel(combatExitModels[i] as CombatExitModel) as CombatExitView;
			if (combatExitView != null)
			{
				CombatExitViews.Add(combatExitView);
			}
		}
		AddDependencyToAllActors();
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		return new List<VisualizationTask> { this };
	}

	public override void Start()
	{
		base.Start();
		ViewsUpdated = false;
		ActionCameraStarted = false;
	}

	private void UpdateViews()
	{
		MissionObjectiveView.UpdateViewFromTask();
		for (int i = 0; i < CombatExitViews.Count; i++)
		{
			CombatExitViews[i].UpdateViewFromTask();
		}
		if (ShowObjectivesPopup)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMissionObjectivesPopUp).Open();
		}
		ViewsUpdated = true;
	}

	public override bool Update(float deltaTime)
	{
		if (!ViewsUpdated)
		{
			UpdateViews();
		}
		if (SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMissionObjectivesPopUp).IsOpen && !GameManager.Instance.playerModel.Combat.MissionCompleted)
		{
			return true;
		}
		return false;
	}
}
