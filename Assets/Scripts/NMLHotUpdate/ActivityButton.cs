using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActivityButton : MonoBehaviour
{
	[SerializeField]
	private GameObject freeRedDot;

	[SerializeField]
	private GameObject newRedDot;

	[SerializeField]
	private GameObject button;

	[SerializeField]
	private GameObject banner;

	private void OnEnable()
	{
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
		UIEvent.OnUIEvent += OnEvent;
		UpdateUi();
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDisable()
	{
		EventManager.OnEvent -= OnEvent;
		UIEvent.OnUIEvent -= OnEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUi();
	}

	private void OnEvent(string type, object parameter)
	{
		if (type == "ActivityIconRefreshEvent")
		{
			UpdateUi();
		}
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		_ = 19;
	}

	private void UpdateUi()
	{
		Helpers.GameObjectSetActive(button, value: false);
		Helpers.GameObjectSetActive(freeRedDot, value: false);
		Helpers.GameObjectSetActive(newRedDot, value: false);
		Helpers.GameObjectSetActive(banner, value: false);
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null || playerModel.CouncilLevel < 3)
		{
			return;
		}
		ActivityIntegrationManager activityIntegrationManager = GameManager.Instance.playerModel?.ActivityIntegrationManager;
		if (activityIntegrationManager == null)
		{
			return;
		}
		List<IActivityManagerIntegrationInterface> integrationActivityList = activityIntegrationManager.GetIntegrationActivityList();
		if (integrationActivityList != null && integrationActivityList.Count > 0)
		{
			Helpers.GameObjectSetActive(button, value: true);
			Helpers.GameObjectSetActive(banner, value: true);
			switch (activityIntegrationManager.GetCampNotifyType())
			{
			case ActivityNotifyType.EventOpen:
				Helpers.GameObjectSetActive(freeRedDot, value: false);
				Helpers.GameObjectSetActive(newRedDot, value: true);
				break;
			case ActivityNotifyType.RewardCanBeClaim:
				Helpers.GameObjectSetActive(freeRedDot, value: true);
				Helpers.GameObjectSetActive(newRedDot, value: false);
				break;
			case ActivityNotifyType.MissionCanBeComplete:
				Helpers.GameObjectSetActive(freeRedDot, value: true);
				Helpers.GameObjectSetActive(newRedDot, value: false);
				break;
			case ActivityNotifyType.None:
				break;
			}
		}
	}

	public void OnButtonClick()
	{
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.ActivityPopup))
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActivityPopup, HUDManager.Instance.UIContainerTopCameras).Open();
		}
	}
}
