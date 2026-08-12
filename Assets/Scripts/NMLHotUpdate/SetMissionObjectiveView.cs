using BaseModel;
using TWDModel;

public class SetMissionObjectiveView : ModelView<SetMissionObjectiveModel>
{
	public string ObjectiveText;

	public string CustomText1;

	public string CustomText2;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChange;
		if (base.Model.IsTriggered)
		{
			UpdateMissionObjective();
		}
	}

	public void OnModelChange(ModelObject model, string changed, object args)
	{
		if (changed == "triggerStateChanged")
		{
			UpdateMissionObjective();
		}
	}

	private void UpdateMissionObjective()
	{
		if (CombatView.Instance != null)
		{
			SetMissionObjectiveModel model = base.Model;
			CombatView.Instance.Model.CurrentMissionObjective.SetDescription(model.ObjectiveText, model.CustomText1, model.CustomText2, showObjectivesPopup: false);
		}
		else
		{
			Debug.LogWarning("CombatView not set - cannot update objective");
		}
	}
}
