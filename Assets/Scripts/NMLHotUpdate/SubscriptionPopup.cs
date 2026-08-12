using System;
using TWDModel;
using UnityEngine;

public class SubscriptionPopup : MonoBehaviour
{
	[SerializeField]
	private UILabel weeklyPriceLabel;

	[SerializeField]
	private UILabel monthlyPriceLabel;

	[SerializeField]
	private UILabel weeklyTimeLabel;

	[SerializeField]
	private UILabel monthlyTimeLabel;

	[SerializeField]
	private GameObject monthlyObj;

	[SerializeField]
	private GameObject weeklyObj;

	[SerializeField]
	private GameObject weeklyActiveObj;

	[SerializeField]
	private GameObject monthlyActiveObj;

	[SerializeField]
	private GameObject pcObj;

	[SerializeField]
	private UILabel activeLabel1;

	[SerializeField]
	private UILabel activeLabel2;

	[SerializeField]
	private UILabel activeLabel3;

	[SerializeField]
	private GameObject activeFx1;

	[SerializeField]
	private GameObject activeFx2;

	[SerializeField]
	private GameObject activeFx3;

	[SerializeField]
	private GameObject phoneContainer;

	[SerializeField]
	private GameObject pcContainer;

	private string WeeklyBundleID => GameManager.Instance.gameEconomyData?.SubscriptionConfig?.WeeklySubscriptionPrice;

	private string MonthlyBundleID => GameManager.Instance.gameEconomyData?.SubscriptionConfig?.MonthlySubscriptionPrice;

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
		if (type == "OnBundleBought")
		{
			UpdateUI();
		}
	}

	public void Update()
	{
		SubscriptionManager subscriptionManager = GameManager.Instance.playerModel?.SubscriptionManager;
		if (subscriptionManager != null)
		{
			CheckPriceLabel(subscriptionManager);
		}
	}

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		UpdateUI();
	}

	public void Close()
	{
		UIEvent.Send("SubscriptionEndEvent");
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void UpdateUI()
	{
		SubscriptionManager subscriptionManager = GameManager.Instance.playerModel.SubscriptionManager;
		if (subscriptionManager == null)
		{
			Debug.LogError("SubscriptionManager is NULL");
			return;
		}
		Helpers.GameObjectSetActive(phoneContainer, value: false);
		Helpers.GameObjectSetActive(pcContainer, value: false);
		if (Helpers.IsPCPlatform())
		{
			Helpers.GameObjectSetActive(pcContainer, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(phoneContainer, value: true);
		}
		CheckPriceLabel(subscriptionManager);
		Helpers.GameObjectSetActive(weeklyObj, !subscriptionManager.IsActiveWeeklySubscription);
		Helpers.GameObjectSetActive(monthlyObj, !subscriptionManager.IsActiveMonthlySubscription);
		Helpers.GameObjectSetActive(weeklyActiveObj, subscriptionManager.IsActiveWeeklySubscription);
		Helpers.GameObjectSetActive(monthlyActiveObj, subscriptionManager.IsActiveMonthlySubscription);
		DateTime dateTime = UtilsDateTime.MillisecondsToDateTime(subscriptionManager.WeeklySubscriptionExpiryMillis);
		DateTime dateTime2 = UtilsDateTime.MillisecondsToDateTime(subscriptionManager.MonthlySubscriptionExpiryMillis);
		HelpersUI.SetContentToLabel(weeklyTimeLabel, string.Format("{0}{1}/{2}/{3}", LocalizationManager.GetText("Popup.Subscription.Auto.Renew.Timer"), dateTime.Year, dateTime.Month, dateTime.Day));
		HelpersUI.SetContentToLabel(monthlyTimeLabel, string.Format("{0}{1}/{2}/{3}", LocalizationManager.GetText("Popup.Subscription.Auto.Renew.Timer"), dateTime2.Year, dateTime2.Month, dateTime2.Day));
		if (subscriptionManager.IsActiveMonthlySubscription)
		{
			Helpers.GameObjectSetActive(weeklyObj, value: false);
			Helpers.GameObjectSetActive(monthlyObj, value: false);
			Helpers.GameObjectSetActive(weeklyActiveObj, value: false);
			Helpers.GameObjectSetActive(monthlyActiveObj, value: true);
		}
		if (pcObj == null)
		{
			if (!OfflineManager.IsLoadDataManager) pcObj = base.gameObject.transform.Find("Container/bottom/label_pc").gameObject;
		}
		Helpers.GameObjectSetActive(pcObj, value: false);
		if (!subscriptionManager.IsActiveWeeklySubscription && !subscriptionManager.IsActiveMonthlySubscription)
		{
			Helpers.GameObjectSetActive(weeklyObj, value: false);
			Helpers.GameObjectSetActive(monthlyObj, value: false);
			Helpers.GameObjectSetActive(weeklyActiveObj, value: false);
			Helpers.GameObjectSetActive(monthlyActiveObj, value: false);
			Helpers.GameObjectSetActive(pcObj, value: true);
		}
		Helpers.GameObjectSetActive(weeklyObj, value: false);
		Helpers.GameObjectSetActive(monthlyObj, value: false);
		HelpersUI.SetContentToLabel(activeLabel1, subscriptionManager.IsSubscriptionActive ? LocalizationManager.GetText("Popup.Subscription.Activated") : LocalizationManager.GetText("Popup.Subscription.InActive"));
		HelpersUI.SetContentToLabel(activeLabel2, subscriptionManager.IsSubscriptionActive ? LocalizationManager.GetText("Popup.Subscription.Activated") : LocalizationManager.GetText("Popup.Subscription.InActive"));
		HelpersUI.SetContentToLabel(activeLabel3, subscriptionManager.IsSubscriptionActive ? LocalizationManager.GetText("Popup.Subscription.Activated") : LocalizationManager.GetText("Popup.Subscription.InActive"));
		Helpers.GameObjectSetActive(activeFx1, subscriptionManager.IsSubscriptionActive);
		Helpers.GameObjectSetActive(activeFx2, subscriptionManager.IsSubscriptionActive);
		Helpers.GameObjectSetActive(activeFx3, subscriptionManager.IsSubscriptionActive);
	}

	private void CheckPriceLabel(SubscriptionManager subModel)
	{
		if (!string.IsNullOrEmpty(WeeklyBundleID))
		{
			SetPrice(WeeklyBundleID, weeklyPriceLabel, subModel.WeeklySyncStatus == SubscriptionSyncStatus.WaitSync);
		}
		if (!string.IsNullOrEmpty(MonthlyBundleID))
		{
			SetPrice(MonthlyBundleID, monthlyPriceLabel, subModel.MonthlySyncStatus == SubscriptionSyncStatus.WaitSync);
		}
	}

	public void OnClickWeek()
	{
		string weeklyBundleID = WeeklyBundleID;
		if (!string.IsNullOrEmpty(weeklyBundleID))
		{
			BuySubscription(weeklyBundleID);
		}
	}

	public void OnClickMonth()
	{
		string monthlyBundleID = MonthlyBundleID;
		if (!string.IsNullOrEmpty(monthlyBundleID))
		{
			BuySubscription(monthlyBundleID);
		}
	}

	public void OnClickInfo()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SubscriptionInfoPopup)?.Open();
	}

	private void SetPrice(string id, UILabel label, bool isChecking)
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		string iAPProduct = gameEconomyData.GetBundleContentDefinition(id).IAPProduct;
		string content = (gameEconomyData.GetInAppPurchaseProduct(iAPProduct).PriceUSD > 0f && GameManager.Instance.IAPManager) ? GameManager.Instance.IAPManager.GetFormattedPrice(iAPProduct) : LocalizationManager.GetText("Generic.Free");
		HelpersUI.SetContentToLabel(label, content);
		if (isChecking)
		{
			HelpersUI.SetContentToLabel(label, LocalizationManager.GetText("Popup.Subscription.Subscription.Checking"));
		}
	}

	private void BuySubscription(string id)
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		BundleStoreDefinition bundleStoreDefinition = gameEconomyData.GetBundleStoreDefinition(id);
		BundleContentDefinition bundleContentDefinition = gameEconomyData.GetBundleContentDefinition(id);
		GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
	}
}
