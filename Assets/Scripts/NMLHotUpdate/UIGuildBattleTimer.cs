using BaseModel;
using TWDModel;
using UnityEngine;

public class UIGuildBattleTimer : MonoBehaviour
{
	public enum BattleEnum
	{
		Ongoing = 0,
		Next = 1
	}

	[Header("If used in combat")]
	[SerializeField]
	private bool usedInCombat;

	[Header("When not ongoing show 00:00 or hide")]
	[SerializeField]
	private bool showZeroWhenNotActive;

	[Header("Optional. If the timer's label might have multiple contents")]
	[SerializeField]
	protected UILabel battleContentLabel;

	[Header("Optional. Auto set at Awake()")]
	[SerializeField]
	private GameObject battleTimerContainer;

	[SerializeField]
	private UILabel battleTimerLabel;

	private float refreshTimeLeft;

	[Header("Battle Setting")]
	public BattleEnum BattleSetting;

	private void Awake()
	{
		bool value = true;
		if (GameManager.Instance != null && GameManager.Instance.playerModel.Combat != null && usedInCombat)
		{
			value = GameManager.Instance.playerModel.Combat.IsGuildBattleMission;
		}
		Helpers.GameObjectSetActive(base.gameObject, value);
		if (battleTimerLabel == null)
		{
			battleTimerLabel = GetComponent<UILabel>();
		}
		if (battleTimerContainer == null)
		{
			battleTimerContainer = base.gameObject;
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed += OnGuildBattlePlayerChange;
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null && guildWarModel.CurrentBattle != null)
		{
			guildWarModel.CurrentBattle.Changed += OnGuildBattleModelChange;
		}
		UpdateUI();
	}

	private void OnDisable()
	{
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null && guildWarModel.CurrentBattle != null)
		{
			guildWarModel.CurrentBattle.Changed -= OnGuildBattleModelChange;
		}
	}

	public void Update()
	{
		refreshTimeLeft -= Time.deltaTime;
		if (!(refreshTimeLeft < 0f))
		{
			return;
		}
		if (battleTimerContainer.activeSelf)
		{
			if (BattleSetting == BattleEnum.Ongoing)
			{
				if (IsOnGoing())
				{
					HelpersUI.SetContentToLabel(battleTimerLabel, GetFormatedTime());
					if (GuildWarHelper.IsLastMinuteForBattleEnd())
					{
						HelpersUI.SetColor(battleTimerLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.LastMinuteWarningLabelColor);
					}
					else
					{
						HelpersUI.SetColor(battleTimerLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NormalTimerColor);
					}
				}
				else
				{
					HelpersUI.SetContentToLabel(battleTimerLabel, Helpers.FormatTimeWithDoubleDigits(0L));
					HelpersUI.SetColor(battleTimerLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.LastMinuteWarningLabelColor);
				}
			}
			else if (BattleSetting == BattleEnum.Next && !IsOnGoing())
			{
				HelpersUI.SetContentToLabel(battleTimerLabel, GetFormatedTime());
				HelpersUI.SetColor(battleTimerLabel, SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NormalTimerColor);
			}
		}
		refreshTimeLeft = 0.5f;
	}

	protected virtual bool IsOnGoing()
	{
		return GuildWarHelper.IsBattleOnGoing();
	}

	protected virtual string GetFormatedTime()
	{
		string result = null;
		if (BattleSetting == BattleEnum.Ongoing)
		{
			result = GuildWarHelper.GetFormatedTimeLeftToCurrentBattleEnd(roundLastMinute: true, lastMinuteWarning: false);
		}
		else if (BattleSetting == BattleEnum.Next)
		{
			result = GuildWarHelper.GetFormatedTimeLeftToNextAvailableBattleStart(roundLastMinute: true, lastMinuteWarning: false);
		}
		return result;
	}

	protected virtual void SetContentInLabel()
	{
		if (battleContentLabel != null)
		{
			HelpersUI.SetContentToLabel(battleContentLabel, "");
		}
	}

	protected virtual void UpdateUI()
	{
		bool value = showZeroWhenNotActive || (IsOnGoing() && BattleSetting == BattleEnum.Ongoing) || (!IsOnGoing() && BattleSetting == BattleEnum.Next);
		Helpers.GameObjectSetActive(battleTimerContainer, value);
		SetContentInLabel();
	}

	private void OnGuildBattlePlayerChange(ModelObject m, string changed, object arg)
	{
		if (changed == "GuildBattleStarted")
		{
			UpdateUI();
		}
	}

	private void OnGuildBattleModelChange(TWDGroupModelChild modelObject, string changed, object args)
	{
		UpdateUI();
	}
}
