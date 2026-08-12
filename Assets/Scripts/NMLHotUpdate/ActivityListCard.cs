using TWDModel;
using UnityEngine;

public class ActivityListCard : UIListCard<IActivityManagerIntegrationInterface>
{
	[SerializeField]
	private UISprite sprite;

	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private GameObject freeRedDot;

	[SerializeField]
	private GameObject newRedDot;

	[SerializeField]
	private GameObject timeGo;

	[SerializeField]
	private UILabel timeLabel;

	private long _gameModeTimeLeft;

	private bool _endedEventSent;

	private void Update()
	{
		if (!timeGo.activeSelf)
		{
			return;
		}
		if (_gameModeTimeLeft >= 0)
		{
			_gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (_gameModeTimeLeft <= 0)
			{
				_gameModeTimeLeft = 0L;
				if (!_endedEventSent)
				{
					_endedEventSent = true;
					UIEvent.Send("ActivityListRefreshEvent");
				}
			}
		}
		if ((bool)timeLabel)
		{
			string text = LocalizationManager.GetText("UI_Roulette_Countdown", FormatTimeLeft(_gameModeTimeLeft));
			HelpersUI.SetContentToLabel(timeLabel, text);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item == null || string.IsNullOrEmpty(base.Item.GetIntegrationEventId()))
		{
			return;
		}
		if (!base.Item.CanShowInActivityList())
		{
			if (!_endedEventSent)
			{
				_endedEventSent = true;
				UIEvent.Send("ActivityListRefreshEvent");
			}
			return;
		}
		if (base.Item.GetIntegrationEventId() == "Subscription")
		{
			UpdateSubscription();
		}
		BroadcastDefinition broadcastDefinition = ((!(base.Item is RouletteActivityDataModel rouletteActivityDataModel)) ? ((!(base.Item is RecycleWeaponActivityModel recycleWeaponActivityModel)) ? GameManager.Instance.gameEconomyData?.GetBroadcastDefinitionById(base.Item.GetIntegrationEventId()) : GameManager.Instance.gameEconomyData?.GetBroadcastDefinitionById(recycleWeaponActivityModel.GetIntegrationEventId(), recycleWeaponActivityModel.Identifier)) : GameManager.Instance.gameEconomyData?.GetBroadcastDefinitionById(rouletteActivityDataModel.GetIntegrationEventId(), rouletteActivityDataModel.ConfigId));
		if (broadcastDefinition != null)
		{
			if (base.Item is WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel)
			{
				string survivorClassName = HelpersLocalization.GetSurvivorClassName(weeklyChallengeClassTeamActivityModel.CurrentDefinition.GetClasses()[0]);
				HelpersUI.SetContentToLabel(label, LocalizationManager.GetText(broadcastDefinition.TabTitle, survivorClassName));
			}
			else
			{
				HelpersUI.SetContentToLabel(label, LocalizationManager.GetText(broadcastDefinition.TabTitle));
			}
			if (base.Item.GetIntegrationEventId() == "Subscription")
			{
				UpdateSubscription();
			}
			else
			{
				HelpersUI.SetSprite(sprite, broadcastDefinition.Icon);
			}
			if (broadcastDefinition.EndTimeMilliseconds > 0)
			{
				Helpers.GameObjectSetActive(timeGo, value: true);
				_gameModeTimeLeft = broadcastDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
				_endedEventSent = _gameModeTimeLeft <= 0;
			}
			else
			{
				Helpers.GameObjectSetActive(timeGo, value: false);
			}
		}
		ActivityNotifyType? activityNotifyType = GameManager.Instance.playerModel?.ActivityIntegrationManager?.GetNotifyTypeByActivityManager(base.Item);
		GameObject obj = freeRedDot;
		bool value;
		if (activityNotifyType.HasValue)
		{
			ActivityNotifyType valueOrDefault = activityNotifyType.GetValueOrDefault();
			if ((uint)(valueOrDefault - 2) <= 1u)
			{
				value = true;
				goto IL_0254;
			}
		}
		value = false;
		goto IL_0254;
		IL_0254:
		Helpers.GameObjectSetActive(obj, value);
		Helpers.GameObjectSetActive(newRedDot, activityNotifyType.HasValue && activityNotifyType == ActivityNotifyType.EventOpen);
	}

	public void OnButtonClick()
	{
		if (base.Item != null)
		{
			UIEvent.Send("ActivityClickEvent", base.Item);
		}
	}

	public void SetBgSprite(string id)
	{
		Helpers.GameObjectSetActive(bg, value: false);
		if (base.Item != null)
		{
			bool value = id.Equals(base.Item.GetIntegrationEventId());
			if (base.Item is RouletteActivityDataModel rouletteActivityDataModel)
			{
				value = id.Equals(rouletteActivityDataModel.ConfigId.ToString());
			}
			else if (base.Item is RecycleWeaponActivityModel recycleWeaponActivityModel)
			{
				value = id.Equals(recycleWeaponActivityModel.Identifier.ToString());
			}
			Helpers.GameObjectSetActive(bg, value);
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "ActivitySendFlagCommandEvent":
			UpdateUI();
			break;
		case "OnBundleBought":
			UpdateUI();
			break;
		case "ActivityClickEvent":
			if (parameter is IActivityManagerIntegrationInterface activityManagerIntegrationInterface)
			{
				if (activityManagerIntegrationInterface is RouletteActivityDataModel rouletteActivityDataModel)
				{
					SetBgSprite(rouletteActivityDataModel.ConfigId.ToString());
				}
				else if (activityManagerIntegrationInterface is RecycleWeaponActivityModel recycleWeaponActivityModel)
				{
					SetBgSprite(recycleWeaponActivityModel.Identifier.ToString());
				}
				else
				{
					SetBgSprite(activityManagerIntegrationInterface.GetIntegrationEventId());
				}
			}
			break;
		}
		if (parameter is IAPConfirmPopupNew && type == "OnPopUpOpen")
		{
			UpdateUI();
		}
	}

	private void UpdateSubscription()
	{
		SubscriptionManager subscriptionManager = GameManager.Instance.playerModel?.SubscriptionManager;
		HelpersUI.SetSprite(sprite, "UI_Icon_Subscription_Bronze");
		if (subscriptionManager == null)
		{
			return;
		}
		if (subscriptionManager.IsSubscriptionActive)
		{
			if (subscriptionManager.IsActiveWeeklySubscription)
			{
				HelpersUI.SetSprite(sprite, "UI_Icon_Subscription_Sliver");
			}
			if (subscriptionManager.IsActiveMonthlySubscription)
			{
				HelpersUI.SetSprite(sprite, "UI_Icon_Subscription_Gold");
			}
		}
		else
		{
			HelpersUI.SetSprite(sprite, "UI_Icon_Subscription_Bronze");
		}
	}

	private string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}
}
