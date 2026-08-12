using UnityEngine;

public class UIGuildBattleTimerFixed : MonoBehaviour
{
	[SerializeField]
	protected UILabel battleTimerLabel;

	protected float refreshTimeLeft;

	protected float refreshTimeRate = 1f;

	protected long battleTimeSlot;

	protected string timerText;

	protected bool deactivateWhenTimerReachesZero;

	protected virtual void OnEnable()
	{
		UpdateUI();
	}

	public void SetBattleTimeSlotForTimer(long timeSlot = 0L, string timerText = "", bool deactivateWhenTimerAtZero = false)
	{
		battleTimeSlot = timeSlot;
		this.timerText = timerText;
		deactivateWhenTimerReachesZero = deactivateWhenTimerAtZero;
		UpdateUI();
	}

	private void Update()
	{
		refreshTimeLeft -= Time.deltaTime;
		if (refreshTimeLeft < 0f)
		{
			UpdateUI();
			refreshTimeLeft = refreshTimeRate;
		}
	}

	protected void UpdateUI()
	{
		bool flag = ShouldBeActive();
		base.gameObject.SetActive(flag);
		if (flag)
		{
			HelpersUI.SetContentToLabel(battleTimerLabel, string.IsNullOrEmpty(timerText) ? GuildWarHelper.SetFormatedTime(GetTimer()) : SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(timerText, GuildWarHelper.SetFormatedTime(GetTimer())));
		}
	}

	protected virtual long GetTimer()
	{
		return GuildWarHelper.GetTimeLeftToBattle(battleTimeSlot);
	}

	protected virtual bool ShouldBeActive()
	{
		if (deactivateWhenTimerReachesZero)
		{
			if (GetTimer() > 0)
			{
				if (GuildWarHelper.IsLockDownTimeForTimeSlotClientSide(battleTimeSlot))
				{
					return GuildWarHelper.GetGuildWarModel().HasEnoughRegisteredPlayersToStartBattleForTimeSlot(battleTimeSlot);
				}
				return true;
			}
			return false;
		}
		return true;
	}
}
