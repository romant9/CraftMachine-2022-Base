using System;

public class CombatTextPopupVisualizationTask : VisualizationTask
{
	private bool globallyBlocking { get; set; }

	public override bool IsGlobalBlocker => globallyBlocking;

	private Action DelayedNotification { get; set; }

	public CombatTextPopupVisualizationTask(Action delayedNotification)
		: base(null)
	{
		globallyBlocking = true;
		DelayedNotification = delayedNotification;
	}

	public override bool Update(float deltaTime)
	{
		if (TutorialView.Instance.PerformingActions)
		{
			return true;
		}
		DelayedNotification();
		return false;
	}
}
