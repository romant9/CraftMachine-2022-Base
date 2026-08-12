using TWDModel;
using UnityEngine;

public class OutpostStateBase : MonoBehaviour
{
	public string StateName;

	private OutpostLevelModel outpostLevelModel;

	public virtual bool ShowHeader => true;

	public virtual GameObject GetTutorialPanel => null;

	public string StateTitleLocalizationKey => "Outpost." + StateName + ".Title";

	public RunLocationModel OutpostTemplateModel { get; set; }

	public OutpostLevelModel OutpostLevelModel
	{
		get
		{
			return outpostLevelModel;
		}
		set
		{
			if (outpostLevelModel != value)
			{
				OutpostLevelModel oldModel = outpostLevelModel;
				outpostLevelModel = value;
				OnOutpostModelChanged(oldModel, outpostLevelModel);
			}
		}
	}

	public event RequestStateChangeHandler OnRequestStateChange;

	protected void RequestStateChange(StateChangeDirection direction)
	{
		this.OnRequestStateChange?.Invoke(direction);
	}

	protected virtual void OnOutpostModelChanged(OutpostLevelModel oldModel, OutpostLevelModel newModel)
	{
	}

	public string GetTitle()
	{
		if (SingularityMonoBehaviour<LocalizationManager>.Instance == null)
		{
			return "";
		}
		return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(StateTitleLocalizationKey);
	}
}
