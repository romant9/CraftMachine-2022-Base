using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActivityPopup : HUDElement
{
	[SerializeField]
	private ActivityListPanel activityListPanel;

	[SerializeField]
	private UIScrollView scrollView;

	[SerializeField]
	private ThreeDayPopup threeDayPopup;

	[SerializeField]
	private SubscriptionPopup subscriptionPopup;

	[SerializeField]
	private ActiveFoundationPopup activeFoundationPopup;

	[SerializeField]
	private RouletteLotteryPopup rouletteLotteryPopup;

	[SerializeField]
	private CampaignPopup campaignPopup;

	[SerializeField]
	private LoginSevenDayPopup sevenDayPopup;

	[SerializeField]
	private WeeklyChallengeActivityPopup weeklyChallengeActivityPopup;

	[SerializeField]
	private RecycleWeaponPopup recycleWeaponPopup;

	private IActivityManagerIntegrationInterface _currentActivityData;

	private float _verticalPadding = 100f;

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
		if (type == "ActivityClickEvent")
		{
			if (parameter is IActivityManagerIntegrationInterface activityData)
			{
				OnSelect(activityData);
			}
		}
		else if (type == "ActivityListRefreshEvent")
		{
			RefreshList();
		}
	}

	private void RefreshList()
	{
		List<IActivityManagerIntegrationInterface> list = GameManager.Instance.playerModel?.ActivityIntegrationManager?.GetIntegrationActivityList();
		if (list == null || list.Count == 0)
		{
			Close();
			return;
		}
		if (activityListPanel == null)
		{
			return;
		}
		activityListPanel.Init(list);
		int num;
		IActivityManagerIntegrationInterface activityManagerIntegrationInterface;
		if (_currentActivityData != null)
		{
			num = (list.Contains(_currentActivityData) ? 1 : 0);
			if (num != 0)
			{
				activityManagerIntegrationInterface = _currentActivityData;
				goto IL_0079;
			}
		}
		else
		{
			num = 0;
		}
		activityManagerIntegrationInterface = list[0];
		goto IL_0079;
		IL_0079:
		IActivityManagerIntegrationInterface activityManagerIntegrationInterface2 = activityManagerIntegrationInterface;
		ActivityListCard activityListCard = activityListPanel.GetCard(activityManagerIntegrationInterface2) as ActivityListCard;
		if (activityListCard != null)
		{
			if (activityManagerIntegrationInterface2 is RouletteActivityDataModel rouletteActivityDataModel)
			{
				activityListCard.SetBgSprite(rouletteActivityDataModel.ConfigId.ToString());
			}
			else if (activityManagerIntegrationInterface2 is RecycleWeaponActivityModel recycleWeaponActivityModel)
			{
				activityListCard.SetBgSprite(recycleWeaponActivityModel.Identifier.ToString());
			}
			else
			{
				activityListCard.SetBgSprite(activityManagerIntegrationInterface2.GetIntegrationEventId());
			}
		}
		if (num == 0)
		{
			_currentActivityData = null;
			OnSelect(activityManagerIntegrationInterface2);
		}
	}

	public override void Open()
	{
		base.Open();
		List<IActivityManagerIntegrationInterface> list = GameManager.Instance.playerModel?.ActivityIntegrationManager?.GetIntegrationActivityList();
		if (activityListPanel != null && list != null && list.Count > 0)
		{
			activityListPanel.Init(list);
			ActivityListCard activityListCard = activityListPanel.GetCard(list[0]) as ActivityListCard;
			if (activityListCard != null)
			{
				if (list[0] is RouletteActivityDataModel rouletteActivityDataModel)
				{
					activityListCard.SetBgSprite(rouletteActivityDataModel.ConfigId.ToString());
				}
				else if (list[0] is RecycleWeaponActivityModel recycleWeaponActivityModel)
				{
					activityListCard.SetBgSprite(recycleWeaponActivityModel.Identifier.ToString());
				}
				else
				{
					activityListCard.SetBgSprite(list[0].GetIntegrationEventId());
				}
			}
			OnSelect(list[0]);
		}
		if (CampView.Instance != null && CampView.Instance.IsShown)
		{
			CampView.Instance.Hud.ShowcampHudContainer(show: false);
			CampView.Instance.Hud.ShowcampUiContainer(show: false);
			CampView.Instance.Hud.UpdateGenericElementsAfterChange();
		}
		if (GameManager.Instance.playerModel != null)
		{
			GameManager.Instance.playerModel.RouletteManager?.GetActiveConfigs();
		}
	}

	public override void Close()
	{
		base.Close();
		_currentActivityData = null;
		if (CampView.Instance != null && CampView.Instance.IsShown && CampView.Instance.Hud != null)
		{
			CampView.Instance.Hud.ShowcampHudContainer(show: true);
			CampView.Instance.Hud.ShowcampUiContainer(show: true);
			CampView.Instance.Hud.UpdateGenericElementsAfterChange();
		}
	}

	public void OnSelect(IActivityManagerIntegrationInterface activityData)
	{
		if (activityData != _currentActivityData)
		{
			_currentActivityData = activityData;
			threeDayPopup.Close();
			subscriptionPopup.Close();
			activeFoundationPopup.Close();
			rouletteLotteryPopup.Close();
			campaignPopup.Close();
			sevenDayPopup.Close();
			weeklyChallengeActivityPopup.Close();
			recycleWeaponPopup.Close();
			if (activityData is SubscriptionManager)
			{
				subscriptionPopup.Open();
			}
			else if (activityData is ActiveFoundationManager)
			{
				activeFoundationPopup.Open();
			}
			else if (activityData is ThreeDayModel)
			{
				threeDayPopup.Open();
			}
			else if (activityData is RouletteActivityDataModel data)
			{
				rouletteLotteryPopup.Open(data);
			}
			else if (activityData is CampaignModel)
			{
				campaignPopup.Open();
			}
			else if (activityData is SevenDayLoginPeriodModel)
			{
				sevenDayPopup.Open();
			}
			else if (activityData is WeeklyChallengeClassTeamActivityModel)
			{
				weeklyChallengeActivityPopup.Open();
			}
			else if (activityData is RecycleWeaponActivityModel info)
			{
				recycleWeaponPopup.SetInfo(info);
				recycleWeaponPopup.Open();
			}
			if (((!(activityData is RouletteActivityDataModel rouletteActivityDataModel)) ? ((!(activityData is RecycleWeaponActivityModel recycleWeaponActivityModel)) ? Helpers.ExecuteCommand(new ActivityIntegrationCloseCanPopOpenStatusCommand(_currentActivityData.GetIntegrationEventId())) : Helpers.ExecuteCommand(new ActivityIntegrationCloseCanPopOpenStatusCommand(_currentActivityData.GetIntegrationEventId(), recycleWeaponActivityModel.Identifier))) : Helpers.ExecuteCommand(new ActivityIntegrationCloseCanPopOpenStatusCommand(_currentActivityData.GetIntegrationEventId(), rouletteActivityDataModel.ConfigId))) == TWDModelResult.OK)
			{
				UIEvent.Send("ActivitySendFlagCommandEvent");
			}
		}
	}

	public void ScrollToIndex(int index)
	{
		GameManager.Instance.TimingManager.Timer(TimeSpan.FromSeconds(0.30000001192092896), delegate
		{
			ScrollTo(index);
		});
	}

	private void ScrollTo(int index)
	{
		scrollView.ResetPosition();
		float y = CalculateTierScroll(index);
		scrollView.MoveRelative(new Vector3(0f, y));
		scrollView.RestrictWithinBounds(instant: true);
	}

	private float CalculateTierScroll(int index)
	{
		return (float)index * _verticalPadding;
	}
}
