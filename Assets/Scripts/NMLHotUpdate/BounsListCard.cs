using TWDModel;
using UnityEngine;

public class BounsListCard : UIListCard<BounsInfo>
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel lineLabel;

	[SerializeField]
	private UILabel lockLabel;

	[SerializeField]
	private UILabel traitDescription;

	[SerializeField]
	private BounsPortrait ownerPortrait;

	[SerializeField]
	private GameObject partnerParent;

	[SerializeField]
	private UIButtonExtended upgradeButton;

	[SerializeField]
	private UIButtonExtended equipButton;

	[SerializeField]
	private UIButtonExtended removeButton;

	[SerializeField]
	private UISprite backgroundSprite;

	[SerializeField]
	private UITexture icon;

	[SerializeField]
	private GameObject traitObj;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color normalColor;

	private bool IsEquipping => base.Item?.BounsModel?.UsingSurvivor != null;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null)
		{
			ResetUI();
			SetupButton();
			SetPortrait();
		}
	}

	private void ResetUI()
	{
		if (nameLabel != null)
		{
			nameLabel.text = LocalizationManager.GetText(base.Item.BounsInfoDefinition.Name);
		}
		if (lineLabel != null)
		{
			lineLabel.text = LocalizationManager.GetText(base.Item.BounsInfoDefinition.Line);
		}
		if (lockLabel != null)
		{
			lockLabel.text = (base.Item.IsLock ? LocalizationManager.GetText("Achievement.CooperationUnlock.Title") : string.Format("{0} Lv.{1}", LocalizationManager.GetText("Achievement.CooperationSkill.Descriptions"), base.Item.Level));
		}
		if (backgroundSprite != null)
		{
			backgroundSprite.color = (IsEquipping ? selectedColor : normalColor);
		}
		if (icon != null)
		{
			Object obj = UnityUtils.LoadFromAssetBundle(IsEquipping ? (base.Item.BounsInfoDefinition.VisualOverride + "_Highlight") : base.Item.BounsInfoDefinition.VisualOverride, "itemgraphics");
			if (obj != null)
			{
				icon.mainTexture = (Texture)obj;
			}
		}
		if (traitDescription != null)
		{
			string content = base.Item.GetTraitDescription(base.Item.IsLock, isTrait: true) + "\n" + base.Item.GetTraitDescription(base.Item.IsLock, isTrait: false);
			HelpersUI.SetContentToLabel(traitDescription, content);
		}
		Helpers.GameObjectSetActive(equipButton, !base.Item.IsLock && !IsEquipping);
		Helpers.GameObjectSetActive(removeButton, !base.Item.IsLock && IsEquipping);
		Helpers.GameObjectSetActive(upgradeButton, base.Item.GetNextBounsLevelDefinition() != null);
		bool flag = removeButton.gameObject.activeSelf || equipButton.gameObject.activeSelf;
		upgradeButton.transform.localPosition = (flag ? new Vector3(0f, 40f, 0f) : Vector3.zero);
		removeButton.transform.localPosition = (upgradeButton.gameObject.activeSelf ? new Vector3(0f, -40f, 0f) : Vector3.zero);
		equipButton.transform.localPosition = (upgradeButton.gameObject.activeSelf ? new Vector3(0f, -40f, 0f) : Vector3.zero);
	}

	private void SetupButton()
	{
		if (upgradeButton != null)
		{
			upgradeButton.SetClickCallback(OnUpgradeButtonClicked);
		}
		if (equipButton != null)
		{
			equipButton.SetClickCallback(OnEquipButtonClicked);
		}
		if (removeButton != null)
		{
			removeButton.SetClickCallback(OnRemoveButtonClicked);
		}
	}

	private void SetPortrait()
	{
		if (ownerPortrait == null || partnerParent == null)
		{
			return;
		}
		ownerPortrait.Init(base.Item.SurvivorModel.ActorDefinitionID);
		string partner = base.Item.BounsInfoDefinition.Partner;
		if (string.IsNullOrEmpty(partner))
		{
			return;
		}
		string[] array = partner.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			string heroId = array[i];
			GameObject gameObject = Helpers.InstantiateToParent(ownerPortrait.gameObject, partnerParent);
			if (gameObject != null)
			{
				BounsPortrait component = gameObject.GetComponent<BounsPortrait>();
				if (component != null)
				{
					component.Init(heroId);
					component.transform.localPosition = new Vector3((float)i * 60f, 0f, 0f);
				}
			}
		}
	}

	private bool IsHaveLockHero(out string message)
	{
		message = "";
		string partner = base.Item.BounsInfoDefinition.Partner;
		if (string.IsNullOrEmpty(partner))
		{
			return false;
		}
		string[] array = partner.Split(';');
		foreach (string text in array)
		{
			ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(text);
			if (!GameManager.Instance.playerModel.SurvivorContainer.HasHero(text))
			{
				message = LocalizationManager.GetText("Achievement.Actor.InefficiencyDescription", actorDefinition.FullName);
				return true;
			}
		}
		return false;
	}

	private void OnUpgradeButtonClicked(UIButtonExtended button)
	{
		if (IsHaveLockHero(out var message))
		{
			UIEvent.Send("BounsInfo", message);
		}
		else if (base.Item?.GetNextBounsLevelDefinition() != null)
		{
			BounsUpgradePopup bounsUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BounsUpgradePopup) as BounsUpgradePopup;
			if (!(bounsUpgradePopup == null))
			{
				bounsUpgradePopup.Open();
				bounsUpgradePopup.Init(base.Item);
			}
		}
	}

	private void OnEquipButtonClicked(UIButtonExtended button)
	{
		if (IsHaveLockHero(out var message))
		{
			UIEvent.Send("BounsInfo", message);
		}
		else if (base.Item?.SurvivorModel != null && base.Item?.BounsModel != null && Helpers.ExecuteCommand(new EquipBounsCommand(base.Item.SurvivorModel, base.Item.BounsModel)) == TWDModelResult.OK)
		{
			ResetUI();
			UIEvent.Send("BounsEquip");
		}
	}

	private void OnRemoveButtonClicked(UIButtonExtended button)
	{
		if (base.Item?.SurvivorModel != null && Helpers.ExecuteCommand(new UnEquipBounsCommand(base.Item.SurvivorModel)) == TWDModelResult.OK)
		{
			ResetUI();
			UIEvent.Send("BounsEquip");
		}
	}

	public void OnTraitTooltipClicked()
	{
		TooltipManager.OpenTextBoxWithText(traitObj, LocalizationManager.GetText(base.Item.BounsInfoDefinition.Line));
	}
}
