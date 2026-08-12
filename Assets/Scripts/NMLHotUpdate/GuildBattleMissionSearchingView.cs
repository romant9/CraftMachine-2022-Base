using System;
using System.Collections.Generic;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class GuildBattleMissionSearchingView : GuildBattleMissionButton
{
	[Header("PvpView")]
	[SerializeField]
	private GameObject searchingContainer;

	[SerializeField]
	private GameObject clearedContainer;

	[SerializeField]
	private UILabel playerNameLabel;

	[NonSerialized]
	public new TweenTimeline ButtonTimeline = new TweenTimeline();

	[Header("Participants tooltip")]
	[SerializeField]
	private GameObject tooltipTarget;

	[SerializeField]
	private GameObject tooltipIcon;

	[SerializeField]
	private UIButton tooltipButton;

	private string participantsTooltipText;

	public void UpdateUI(GuildBattleMissionQueueData queueData)
	{
		if (queueData != null)
		{
			List<string> list = ((queueData.EnemyMission == null) ? null : queueData.EnemyMission.PvpParticipants);
			string memberId = ((queueData.EnemyMission == null) ? "" : queueData.EnemyMission.PvpPlayerHashedId);
			GuildMemberInfo guildMemberInfo = ((GameManager.Instance.guildModel == null) ? null : GameManager.Instance.guildModel.GetMemberInfo(memberId));
			string text = ((guildMemberInfo == null) ? "" : guildMemberInfo.Name);
			bool flag = list != null && list.Count > 0;
			Helpers.GameObjectSetActive(tooltipIcon, flag);
			tooltipButton.isEnabled = flag;
			if (flag)
			{
				participantsTooltipText = HelpersLocalization.GetParticipantsTooltipText(queueData.EnemyMission);
			}
			GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
			if (((guildBattleMapPopup != null) ? guildBattleMapPopup.GetInitState() : InitState.None) == InitState.ReturnFromCombat)
			{
				CheckIfPlayerJustCompletedMission(queueData.EnemyMission);
			}
			HelpersUI.SetContentToLabel(playerNameLabel, LocalizationManager.GetText("GvG.PvpDefeatedBy{playerName}", GameManager.Instance.GetFilteredText(text)), !string.IsNullOrEmpty(text));
			bool last = queueData.Last;
			Helpers.GameObjectSetActive(searchingContainer, !last);
			Helpers.GameObjectSetActive(clearedContainer, last);
		}
	}

	public new void ClearTimeline()
	{
		ButtonTimeline.Clear();
	}

	public new void QueueOpenTween()
	{
		if (initState != InitState.IsOpen)
		{
			ButtonTimeline.Queue(TweenObjects.Group(base.transform, TweenGroupOpen));
		}
	}

	public void QueueCloseTween(GuildBattleMissionQueueData queueData)
	{
		if (queueData.IsComplete && !queueData.Last)
		{
			ButtonTimeline.Queue(TweenObjects.Group(base.transform, TweenGroupClose));
		}
	}

	public void ShowParticipantsTooltip()
	{
		if (!string.IsNullOrEmpty(participantsTooltipText))
		{
			TooltipManager.OpenTextBoxWithText(tooltipTarget, participantsTooltipText);
		}
	}
}
