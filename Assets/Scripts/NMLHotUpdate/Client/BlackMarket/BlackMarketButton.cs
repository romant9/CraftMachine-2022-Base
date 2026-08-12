using TWDModel;
using UnityEngine;

namespace Client.BlackMarket
{
	[RequireComponent(typeof(UIButton))]
	public class BlackMarketButton : MonoBehaviour
	{
		[SerializeField]
		private GameObject blackMarketSlotUpdatedNotification;

		private float slotUpdateInterval;

		private EventDelegate onClickEventDelegate;

		private UIButton button;

		private void Awake()
		{
			button = GetComponent<UIButton>();
			onClickEventDelegate = new EventDelegate(OnClick);
			UIEvent.OnUIEvent += OnUIEvent;
		}

		private void OnDestroy()
		{
			UIEvent.OnUIEvent -= OnUIEvent;
		}

		private void OnEnable()
		{
			Helpers.GameObjectSetActive(blackMarketSlotUpdatedNotification, GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleBlackMarketSlotUpdated"));
			button.onClick.Add(onClickEventDelegate);
		}

		private void Update()
		{
			if (TutorialView.Instance != null && TutorialView.Instance.Running)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			slotUpdateInterval += Time.deltaTime;
			if (slotUpdateInterval >= 1f)
			{
				TryUpdateBlackMarketSlots();
				slotUpdateInterval = 0f;
			}
		}

		private void OnDisable()
		{
			button.onClick.Remove(onClickEventDelegate);
		}

		private void OnClick()
		{
			if (blackMarketSlotUpdatedNotification.activeInHierarchy && Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("Toggle.ToggleBlackMarketSlotUpdated")) == TWDModelResult.OK)
			{
				Helpers.GameObjectSetActive(blackMarketSlotUpdatedNotification, value: false);
			}
			ShopPopupHelper.OpenWithIndex(4);
		}

		private void OnUIEvent(string type, object parameter)
		{
			switch (type)
			{
			case "OnBuildingMoveCancelled":
			case "OnBuildingMoveEnded":
				if (TutorialView.Instance == null || !TutorialView.Instance.Running)
				{
					base.gameObject.SetActive(value: true);
				}
				break;
			case "OnBuildingConstructionStartPlacing":
			case "OnBuildingMoveStarted":
				base.gameObject.SetActive(value: false);
				break;
			}
		}

		private void TryUpdateBlackMarketSlots()
		{
			if (GameManager.Instance.playerModel.BlackMarket.NeedToUpdate() && Helpers.ExecuteCommand(new UpdateBlackMarketCommand()) == TWDModelResult.OK)
			{
				Helpers.GameObjectSetActive(blackMarketSlotUpdatedNotification, value: true);
			}
		}
	}
}
