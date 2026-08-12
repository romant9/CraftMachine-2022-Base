using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActorStatusInfoContainer : HUDElement
{
	[Tooltip("Actor Status Label")]
	public UILabel StatusLabel;

	[Tooltip("Actor Status Background Sprite")]
	public UISprite StatusBackground;

	public GameObject StatusIndicatorContainer;

	public UILabel StatusTurnCountLabel;

	public GameObject StruggleStatusIconContainer;

	public GameObject StunnedStatusIconContainer;

	public GameObject ReloadingStatusIconContainer;

	public GameObject InvisibleStatusIconContainer;

	public GameObject BleedingStatusIconContainer;

	public GameObject BurningStatusIconContainer;

	public GameObject RootedStatusIconContainer;

	public GameObject StaggeredStatusIconContainer;

	public GameObject HealButtonContainer;

	public float statusTextChangeTimeInSeconds = 2f;

	private float dt;

	private List<ActorStatusInfo> statusInfos;

	private int statusIndex;

	private bool timerEnabled;

	public ActorModel Actor;

	public ActorStatusInfoContainer()
	{
		statusInfos = new List<ActorStatusInfo>();
		dt = 0f;
		statusIndex = 0;
		timerEnabled = true;
	}

	public void SetStatusInfo(List<ActorStatusInfo> infos)
	{
		statusInfos = infos;
		dt = 0f;
		if (statusInfos.Count > 0)
		{
			TweenManager.PlayTweenGroup(base.gameObject, 10, forward: true, OnStatusFadeOutPlayed);
			return;
		}
		StatusIndicatorContainer.SetActive(value: false);
		HealButtonContainer.SetActive(value: false);
		StatusLabel.gameObject.SetActive(value: false);
		StatusBackground.gameObject.SetActive(value: false);
	}

	public override void Update()
	{
		if (statusInfos.Count > 1 && timerEnabled)
		{
			dt += Time.deltaTime;
			if (dt >= statusTextChangeTimeInSeconds)
			{
				timerEnabled = false;
				TweenManager.PlayTweenGroup(base.gameObject, 10, forward: true, OnStatusFadeOutPlayed);
				dt = 0f;
			}
		}
	}

	public void ShowHealTooltip()
	{
		if (HasHealableStatus() && !Actor.AbilityCompleted)
		{
			TooltipManager.OpenTextBoxWithText(HealButtonContainer, LocalizationManager.GetText("Tooltip.HealableActorStatus"));
		}
	}

	private bool HasHealableStatus()
	{
		for (int i = 0; i < statusInfos.Count; i++)
		{
			if (statusInfos[i].StatusType == ActorStatusType.Bleeding || statusInfos[i].StatusType == ActorStatusType.Burning)
			{
				return true;
			}
		}
		return false;
	}

	private void OnStatusFadeOutPlayed()
	{
		if (statusInfos.Count > 0)
		{
			if (statusIndex < statusInfos.Count - 1)
			{
				statusIndex++;
			}
			else
			{
				statusIndex = 0;
			}
			ActorStatusInfo actorStatusInfo = statusInfos[statusIndex];
			StatusIndicatorContainer.SetActive(value: false);
			StruggleStatusIconContainer.SetActive(value: false);
			StunnedStatusIconContainer.SetActive(value: false);
			ReloadingStatusIconContainer.SetActive(value: false);
			InvisibleStatusIconContainer.SetActive(value: false);
			BleedingStatusIconContainer.SetActive(value: false);
			BurningStatusIconContainer.SetActive(value: false);
			RootedStatusIconContainer.SetActive(value: false);
			StaggeredStatusIconContainer.SetActive(value: false);
			StatusTurnCountLabel.text = ((actorStatusInfo.TurnCount == -1) ? string.Empty : actorStatusInfo.TurnCount.ToString());
			bool flag = false;
			string textId = "";
			if (actorStatusInfo.StatusType == ActorStatusType.Bleeding)
			{
				StatusIndicatorContainer.SetActive(value: true);
				BleedingStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Bleeding";
				flag = true;
			}
			else if (actorStatusInfo.StatusType == ActorStatusType.Burning)
			{
				StatusIndicatorContainer.SetActive(value: true);
				BurningStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Burning";
				flag = true;
			}
			else if (actorStatusInfo.StatusType == ActorStatusType.Struggling)
			{
				StatusIndicatorContainer.SetActive(value: true);
				StruggleStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Struggling";
			}
			else if (actorStatusInfo.StatusType == ActorStatusType.Stunned)
			{
				StatusIndicatorContainer.SetActive(value: true);
				StunnedStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Stunned";
			}
			else if (actorStatusInfo.StatusType == ActorStatusType.Reloading)
			{
				StatusIndicatorContainer.SetActive(value: true);
				ReloadingStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Reloading";
			}
			else if (actorStatusInfo.StatusType == ActorStatusType.Rooted)
			{
				StatusIndicatorContainer.SetActive(value: true);
				RootedStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Rooted";
			}
			if (actorStatusInfo.StatusType == ActorStatusType.IsInvisible)
			{
				StatusIndicatorContainer.SetActive(value: true);
				InvisibleStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Invisible";
			}
			if (actorStatusInfo.StatusType == ActorStatusType.StaggerActive)
			{
				StatusIndicatorContainer.SetActive(value: true);
				StaggeredStatusIconContainer.SetActive(value: true);
				textId = "ActorStatusInfo.Staggered";
			}
			HealButtonContainer.SetActive(flag && !Actor.AbilityCompleted && !Actor.SecondMoveCompleted);
			StatusLabel.gameObject.SetActive(value: true);
			StatusBackground.gameObject.SetActive(value: true);
			StatusLabel.text = LocalizationManager.GetText(textId);
			TweenManager.PlayTweenGroup(base.gameObject, 11, forward: true, OnStatusFadeInPlayed);
		}
		else
		{
			timerEnabled = true;
			StatusIndicatorContainer.SetActive(value: false);
			HealButtonContainer.SetActive(value: false);
			StatusLabel.gameObject.SetActive(value: false);
			StatusBackground.gameObject.SetActive(value: false);
		}
	}

	private void OnStatusFadeInPlayed()
	{
		timerEnabled = true;
	}
}
