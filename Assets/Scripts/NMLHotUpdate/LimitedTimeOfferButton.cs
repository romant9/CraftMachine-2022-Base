using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class LimitedTimeOfferButton : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UISprite timerLabelBG;

	[SerializeField]
	private UISprite timerLabelShadow;

	[SerializeField]
	private UISprite buttonSprite;

	[SerializeField]
	private Metrics.BundleSource bundleSource;

	private LimitedBundleData bundleOffered;

	private void OnEnable()
	{
		NoOffer();
		UIEvent.OnUIEvent += OnEvent;
		SetOffer(GetFirstLimitedBundle());
		GameManager instance = GameManager.Instance;
		if (instance != null)
		{
			PlayerModel playerModel = instance.playerModel;
			if (playerModel.BundleManager != null)
			{
				playerModel.BundleManager.Changed -= OnBundleManagerChanged;
				playerModel.BundleManager.Changed += OnBundleManagerChanged;
			}
		}
		UIEvent.Send("CampBottomLeftFreshEvent");
	}

	private void OnBundleManagerChanged(ModelObject m, string changed, object args)
	{
		if (changed == "LimitedBundleAvailableEvent")
		{
			NoOffer();
			SetOffer(GetFirstLimitedBundle());
			UIEvent.Send("CampBottomLeftFreshEvent");
		}
	}

	protected virtual LimitedBundleData GetFirstLimitedBundle()
	{
		BundleManagerModel bundleManager = GameManager.Instance.playerModel.BundleManager;
		LimitedBundleData limitedBundleData = null;
		if (bundleManager != null)
		{
			List<LimitedBundleData> initiatedLimitedBundles = bundleManager.InitiatedLimitedBundles;
			for (int i = 0; i < initiatedLimitedBundles.Count; i++)
			{
				LimitedBundleData limitedBundleData2 = initiatedLimitedBundles[i];
				if (limitedBundleData2 != null && limitedBundleData2.IsAvailable && (limitedBundleData == null || limitedBundleData.AvailabilityTime > limitedBundleData2.AvailabilityTime))
				{
					limitedBundleData = limitedBundleData2;
				}
			}
		}
		return limitedBundleData;
	}

	private void OnDisable()
	{
		NoOffer();
		UIEvent.OnUIEvent -= OnEvent;
		GameManager instance = GameManager.Instance;
		if (instance != null)
		{
			PlayerModel playerModel = instance.playerModel;
			if (playerModel.BundleManager != null)
			{
				playerModel.BundleManager.Changed -= OnBundleManagerChanged;
			}
		}
	}

	private void OnEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnBuildingMoveCancelled":
		case "OnBuildingMoveEnded":
			NoOffer();
			SetOffer(GetFirstLimitedBundle());
			UIEvent.Send("CampBottomLeftFreshEvent");
			break;
		case "OnBuildingConstructionStartPlacing":
		case "OnBuildingMoveStarted":
			button.SetActive(value: false);
			UIEvent.Send("CampBottomLeftFreshEvent");
			break;
		}
	}

	protected virtual void NoOffer()
	{
		bundleOffered = null;
		if (button != null)
		{
			button.SetActive(value: false);
		}
	}

	protected virtual void SetOffer(LimitedBundleData bundle)
	{
		bundleOffered = bundle;
		button.SetActive(bundle != null);
		if (bundleOffered != null)
		{
			BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(bundleOffered.BundleID);
			if (bundleStoreDefinition != null && !string.IsNullOrEmpty(bundleStoreDefinition.OfferButtonSpriteName))
			{
				DebugTWD.Log($"Set Offer bundle: {bundle.BundleID}, {bundle.EndTimestamp}");
				HelpersUI.SetSprite(buttonSprite, bundleStoreDefinition.OfferButtonSpriteName);
			}
			else
			{
				HelpersUI.SetSprite(buttonSprite, "Ui_Shop_Bundle_Special");
			}
		}
	}

	private void Update()
	{
		bool activeSelf = button.activeSelf;
		if (bundleOffered != null)
		{
			timerLabel.text = Helpers.FormatTimeNoZero(bundleOffered.Timer);
			if (bundleOffered.Timer <= 0)
			{
				NoOffer();
			}
			LimitedBundleData firstLimitedBundle = GetFirstLimitedBundle();
			if (bundleOffered != firstLimitedBundle || !button.activeSelf)
			{
				SetOffer(firstLimitedBundle);
			}
			if (bundleOffered != null && GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(bundleOffered.BundleID).NoPopUpOfferTimer)
			{
				Helpers.GameObjectSetActive(timerLabel, value: false);
				Helpers.GameObjectSetActive(timerLabelBG, value: false);
				Helpers.GameObjectSetActive(timerLabelShadow, value: false);
			}
		}
		if (TutorialView.Instance.RunningButNotSuggesting)
		{
			button.SetActive(value: false);
		}
		RefreshBottomLeftTable(activeSelf);
	}

	public void OnClick()
	{
		if (bundleOffered != null && bundleOffered.BundleID != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			GameManager.Instance.BundleSource = bundleSource;
			BundleCardPopup.OpenBundle(bundleOffered.BundleID);
		}
	}

	private void RefreshBottomLeftTable(bool isActive)
	{
		if (isActive != button.activeSelf)
		{
			UIEvent.Send("CampBottomLeftFreshEvent");
		}
	}
}
