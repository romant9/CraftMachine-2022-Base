using UnityEngine;

public class UIGuildWarTimerWithLabel : UIGuildBattleTimerFixed
{
	[SerializeField]
	private string localizationString;

	protected override void OnEnable()
	{
		base.OnEnable();
		timerText = localizationString;
	}

	protected override long GetTimer()
	{
		return GuildWarHelper.GetTimeLeftToCurrentWarEnd();
	}

	protected override bool ShouldBeActive()
	{
		bool num = GuildWarHelper.IsLockedByCouncilLevelOrTutorial();
		bool flag = !GuildWarHelper.IsGuildMember();
		bool flag2 = !GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled;
		if (!(num || flag || flag2))
		{
			if (deactivateWhenTimerReachesZero)
			{
				return GetTimer() > 0;
			}
			return true;
		}
		return false;
	}
}
