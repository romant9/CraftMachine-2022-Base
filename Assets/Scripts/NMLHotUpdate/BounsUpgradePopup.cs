using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BounsUpgradePopup : HUDElement
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel nowLevelLabel;

	[SerializeField]
	private UILabel nextLevelLabel;

	[SerializeField]
	private BounsTraitsPanel bounsTraitsPanel;

	[SerializeField]
	private BounsTraitsPanel bounsQualityPanel;

	[SerializeField]
	private BounsTraitsPanel nextBounsTraitsPanel;

	[SerializeField]
	private BounsTraitsPanel nextBounsQualityPanel;

	[SerializeField]
	private GameObject costInfoParent;

	[SerializeField]
	private GameObject costItemObj;

	[SerializeField]
	private UIButtonExtended upgradeButton;

	[SerializeField]
	private UITexture icon;

	private BounsInfo bounsInfo;

	[SerializeField]
	[Tooltip("Label show when there is some info")]
	private UILabel infoLabel;

	[SerializeField]
	private UISprite infoSprite;

	[SerializeField]
	[Tooltip("Time to show in seconds")]
	private float timeToShow = 2f;

	[SerializeField]
	private Color errorColor;

	[SerializeField]
	private Color normalColor;

	public void Init(BounsInfo info)
	{
		bounsInfo = info;
		UpdateUpgradePopup();
		UpdateCostInfo();
		InternalHide();
	}

	private void UpdateCostInfo()
	{
		Helpers.GameObjectSetActive(costInfoParent, value: false);
		BounsLevelDefinition nextBounsLevelDefinition = bounsInfo.GetNextBounsLevelDefinition();
		if (nextBounsLevelDefinition == null)
		{
			return;
		}
		Dictionary<CurrencyType, int> costInfo = nextBounsLevelDefinition.GetCostInfo();
		if (costInfo == null || costInfo.Count <= 0)
		{
			return;
		}
		Helpers.GameObjectSetActive(costInfoParent, value: true);
		bool flag = true;
		foreach (KeyValuePair<CurrencyType, int> item in costInfo)
		{
			GameObject gameObject = Helpers.InstantiateToParent(costItemObj, costInfoParent);
			if (!(gameObject != null))
			{
				continue;
			}
			BounsCostItem component = gameObject.GetComponent<BounsCostItem>();
			if (component != null)
			{
				component.Init(item.Key, item.Value);
				Helpers.GameObjectSetActive(gameObject, value: true);
				if (!component.IsEnough(item.Key, item.Value))
				{
					flag = false;
				}
			}
		}
		upgradeButton.IsVisuallyDisabled = !flag;
	}

	private void UpdateUpgradePopup()
	{
		if (nameLabel != null)
		{
			nameLabel.text = LocalizationManager.GetText(bounsInfo.BounsInfoDefinition.Name);
		}
		if (nowLevelLabel != null)
		{
			nowLevelLabel.text = "Lv." + bounsInfo.Level;
		}
		if (nextLevelLabel != null)
		{
			nextLevelLabel.text = "Lv." + (bounsInfo.Level + 1);
		}
		if (upgradeButton != null)
		{
			upgradeButton.SetClickCallback(OnUpgradeClicked);
		}
		if (icon != null)
		{
			Object obj = UnityUtils.LoadFromAssetBundle(bounsInfo.BounsInfoDefinition.VisualOverride, "itemgraphics");
			if (obj != null)
			{
				icon.mainTexture = (Texture)obj;
			}
		}
		Helpers.GameObjectSetActive(bounsTraitsPanel, value: false);
		Helpers.GameObjectSetActive(bounsQualityPanel, value: false);
		Helpers.GameObjectSetActive(nextBounsTraitsPanel, value: false);
		Helpers.GameObjectSetActive(nextBounsQualityPanel, value: false);
		BounsLevelDefinition currentBounsLevelDefinition = bounsInfo.GetCurrentBounsLevelDefinition();
		BounsLevelDefinition nextBounsLevelDefinition = bounsInfo.GetNextBounsLevelDefinition();
		if (currentBounsLevelDefinition != null && bounsTraitsPanel != null && bounsQualityPanel != null)
		{
			Helpers.GameObjectSetActive(bounsTraitsPanel, value: true);
			Helpers.GameObjectSetActive(bounsQualityPanel, value: true);
			bounsTraitsPanel.Init(bounsInfo.GetTraitLevel(currentBounsLevelDefinition.TraitsLevel), bounsInfo.GetTraitDescription(isNext: false, isTrait: true), currentBounsLevelDefinition.TraitsLevel);
			bounsQualityPanel.Init(bounsInfo.GetTraitLevel(currentBounsLevelDefinition.QualityLevel), bounsInfo.GetTraitDescription(isNext: false, isTrait: false), currentBounsLevelDefinition.QualityLevel);
		}
		if (nextBounsLevelDefinition != null && nextBounsTraitsPanel != null && nextBounsQualityPanel != null)
		{
			Helpers.GameObjectSetActive(nextBounsTraitsPanel, value: true);
			Helpers.GameObjectSetActive(nextBounsQualityPanel, value: true);
			nextBounsTraitsPanel.Init(bounsInfo.GetTraitLevel(nextBounsLevelDefinition.TraitsLevel), bounsInfo.GetTraitDescription(isNext: true, isTrait: true), nextBounsLevelDefinition.TraitsLevel);
			nextBounsQualityPanel.Init(bounsInfo.GetTraitLevel(nextBounsLevelDefinition.QualityLevel), bounsInfo.GetTraitDescription(isNext: true, isTrait: false), nextBounsLevelDefinition.QualityLevel);
		}
	}

	private void OnUpgradeClicked(UIButtonExtended button)
	{
		switch (Helpers.ExecuteCommand(new UpgradeBounsLevelCommand(bounsInfo.BounsInfoDefinition.ItemID)))
		{
		case TWDModelResult.OK:
			UIEvent.Send("BounsUpgrade", LocalizationManager.GetText("Achievement.CooperationLvup.Title"));
			Close();
			break;
		case TWDModelResult.NotEnoughCurrency:
			ShowInfo(LocalizationManager.GetText("Achievement.Item.InefficiencyDescription"), isError: true);
			break;
		}
	}

	private void ShowInfo(string text, bool isError = false)
	{
		InternalHide();
		SetInfoText(infoLabel, text);
		infoSprite.color = (isError ? errorColor : normalColor);
	}

	private void SetInfoText(UILabel label, string text)
	{
		if (label != null && label.gameObject != null)
		{
			label.gameObject.SetActive(value: true);
			label.text = text;
			CancelInvoke("InternalHide");
			Invoke("InternalHide", timeToShow);
		}
		else
		{
			Debug.LogError("HUDNotification: Could not show notification because label is NULL!");
		}
	}

	private void InternalHide()
	{
		if (infoLabel != null && infoLabel.gameObject != null)
		{
			infoLabel.gameObject.SetActive(value: false);
		}
	}
}
