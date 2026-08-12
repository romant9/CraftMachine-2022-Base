using UnityEngine;

public class UIGuildBattleLockdownTimer : UIGuildBattleTimerFixed
{
	[SerializeField]
	private bool accurate;

	protected override long GetTimer()
	{
		if (!accurate)
		{
			return GuildWarHelper.GetTimeToBattleLockdownClient(battleTimeSlot);
		}
		return GuildWarHelper.GetTimeToBattleLockdown(battleTimeSlot);
	}

	protected override bool ShouldBeActive()
	{
		if (deactivateWhenTimerReachesZero)
		{
			return GetTimer() > 0;
		}
		return true;
	}
}
