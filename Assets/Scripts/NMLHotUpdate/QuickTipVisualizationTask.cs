using System.Collections.Generic;

public class QuickTipVisualizationTask : VisualizationTask
{
	public string TipID { get; private set; }

	public override bool IsGlobalBlocker => true;

	public QuickTipVisualizationTask(string tipID)
		: base(null)
	{
		TipID = tipID;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		return new List<VisualizationTask> { this };
	}

	public override void Start()
	{
		base.Start();
		PopupQuickTip obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatQuickTipPopup) as PopupQuickTip;
		obj.TipId = TipID;
		obj.Open();
	}

	public override bool Update(float deltaTime)
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatQuickTipPopup).IsOpen)
		{
			return true;
		}
		return false;
	}
}
