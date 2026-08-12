using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillBagRight : MonoBehaviour
{
	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UILabel traitDesc;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private UILabel btnText;

	[SerializeField]
	private GameObject btnRedGo;

	[SerializeField]
	private GameObject btnGreenGo;

	[SerializeField]
	private GameObject btnGrayGo;

	[SerializeField]
	private GameObject usedGo;

	[SerializeField]
	private GameObject lockGo;

	[SerializeField]
	private GameObject usedWeaponIconContainer;

	[SerializeField]
	private UITexture weaponIcon;

	[SerializeField]
	private UITexture armorIcon;

	[SerializeField]
	private GameObject traitIconUpgradeFxPrefab;

	[SerializeField]
	private UIScrollView ScrollViewDescription;

	private SPTraitsRemoldDefinitions modSkillDefinition;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private SurvivorClass currentFilterClass = SurvivorClass.None;

	private float lastUpgradeClickTime = -999f;

	private const float UpgradeClickCooldownSeconds = 0.2f;

	private ModSkillMode modSkillMode
	{
		get
		{
			if (modSkillDefinition == null)
			{
				return null;
			}
			return playerModel.ModSkillManager.GetModSkillMode(modSkillDefinition.ID);
		}
	}

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void Awake()
	{
		Helpers.GameObjectSetActive(EntryPrefab, value: false);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SPRemoldBagItemClick":
			if (parameter != null && parameter is string)
			{
				string modSkillId2 = (string)parameter;
				Setup(modSkillId2);
			}
			break;
		case "SPRemoldChangeSurvivorClassFilter":
			if (parameter != null && parameter is SurvivorClass)
			{
				currentFilterClass = (SurvivorClass)parameter;
				UpdateUISurvivorClass();
			}
			break;
		case "SPRemoldUpgradeModSkillSuccess":
			if (parameter != null && parameter is string modSkillId)
			{
				Setup(modSkillId);
			}
			else
			{
				UpdateUI();
			}
			break;
		}
	}

	public void Setup(string modSkillId)
	{
		modSkillDefinition = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillId);
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (modSkillDefinition == null)
		{
			return;
		}
		traitName.text = LocalizationManager.GetText(modSkillDefinition.SPTraitsName);
		UILabel uILabel = traitDesc;
		string sPTraitsDesc = modSkillDefinition.SPTraitsDesc;
		object[] arguments = modSkillDefinition.SPTraitsLcValue.ToArray();
		uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
		level.text = LocalizationManager.GetText("System.EquipInfo.Remold.LevelX") + modSkillDefinition.Level;
		HelpersUI.SetTraitsIconOnSprite(traitIcon, modSkillDefinition.SPTraitsIcon, modSkillDefinition.SPTraitsIconOnCloud);
		if (ScrollViewDescription) ScrollViewDescription.ResetPosition();
		starList.Setup(modSkillDefinition.Star);
		Helpers.GameObjectSetActive(lockGo, value: false);
		Helpers.GameObjectSetActive(usedGo, value: false);
		Helpers.GameObjectSetActive(btnRedGo, value: false);
		Helpers.GameObjectSetActive(btnGrayGo, value: false);
		Helpers.GameObjectSetActive(btnGreenGo, value: false);
		if (modSkillMode != null)
		{
			Helpers.GameObjectSetActive(usedGo, modSkillMode.ModSkillState == ModSkillState.Equipped);
			if (modSkillDefinition.MaxLevel <= modSkillDefinition.Level)
			{
				btnText.text = LocalizationManager.GetText("System.Button.Remold.LvMax");
				Helpers.GameObjectSetActive(btnGrayGo, value: true);
			}
			else
			{
				btnText.text = LocalizationManager.GetText("System.Button.Remold.Upgrade");
				Helpers.GameObjectSetActive(btnRedGo, value: true);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(lockGo, value: true);
			Helpers.GameObjectSetActive(btnGreenGo, value: true);
			btnText.text = LocalizationManager.GetText("System.Button.Remold.GetIt");
		}
		Helpers.GameObjectSetActive(weaponIcon, value: false);
		Helpers.GameObjectSetActive(armorIcon, value: false);
		Helpers.GameObjectSetActive(usedWeaponIconContainer, value: false);
		if (modSkillMode != null && modSkillMode.EquipmentItemModel != null)
		{
			Helpers.GameObjectSetActive(usedWeaponIconContainer, value: true);
			EquipmentDefinition definition = modSkillMode.EquipmentItemModel.Definition;
			if (definition.Category == EquipmentCategory.Armor)
			{
				Helpers.GameObjectSetActive(armorIcon, value: true);
				armorIcon.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(definition.ID);
				if (definition.UseSpecialMaterial)
				{
					Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(definition).specialMaterial;
					armorIcon.material = specialMaterial ?? armorIcon.material;
				}
			}
			else
			{
				Helpers.GameObjectSetActive(weaponIcon, value: true);
				weaponIcon.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(definition.ID);
				if (definition.UseSpecialMaterial)
				{
					Material specialMaterial2 = HelpersGfx.GetEquipmentResourceEntry(definition).specialMaterial;
					weaponIcon.material = specialMaterial2 ?? weaponIcon.material;
				}
			}
		}
		FreshListData();
	}

	private void FreshListData()
	{
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		Dictionary<CurrencyType, int> dictionary = playerModel.ModSkillManager.GetMakingCost(modSkillDefinition.ID);
		if (modSkillMode != null)
		{
			dictionary = playerModel.ModSkillManager.GetUpgradeModSkillCost(modSkillDefinition.ID);
		}
		if (dictionary != null && dictionary.Count > 0)
		{
			foreach (KeyValuePair<CurrencyType, int> item in dictionary)
			{
				GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
				if (gameObject.TryGetComponent<SPRemoldSkillBagRightItem>(out var component2))
				{
					component2.Setup(item.Key, item.Value);
				}
				Entries.Add(gameObject);
			}
		}
		component.Reposition();
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public void UpdateUISurvivorClass()
	{
		classIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(currentFilterClass);
	}

	public void OnclickUnlock()
	{
		if (!OfflineManager.IsFreeAll && !playerModel.ModSkillManager.CanUpgradeModSkillForUnlock(modSkillDefinition.ID))
		{
			SPRemoldSkillNotEnoughPopup sPRemoldSkillNotEnoughPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldSkillNotEnoughPopup) as SPRemoldSkillNotEnoughPopup;
			if (sPRemoldSkillNotEnoughPopup != null)
			{
				sPRemoldSkillNotEnoughPopup.Open();
			}
		}
		else
		{
			if (OfflineManager.IsFakeExecuteCommands)
			{
				if (MakeModSkillCommandResult(modSkillDefinition.ID, modSkillDefinition.Type, out _) == TWDModelResult.OK)
				{
					SPRemoldTraitsSkillMergedPopup sPRemoldTraitsSkillMergedPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillMergedPopup) as SPRemoldTraitsSkillMergedPopup;
					if (sPRemoldTraitsSkillMergedPopup != null)
					{
						sPRemoldTraitsSkillMergedPopup.Setup(modSkillDefinition.ID);
						sPRemoldTraitsSkillMergedPopup.Open();
					}
					UIEvent.Send("SPRemoldMakeModSkillSuccess", modSkillDefinition.ID);
				}
			}
			else
			{
				if (Helpers.ExecuteCommand(new MakeModSkillCommand(modSkillDefinition.ID, modSkillDefinition.Type)
				{
					SurvivorClass = currentFilterClass
				}) == TWDModelResult.OK)

				{
					SPRemoldTraitsSkillMergedPopup sPRemoldTraitsSkillMergedPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillMergedPopup) as SPRemoldTraitsSkillMergedPopup;
					if (sPRemoldTraitsSkillMergedPopup != null)
					{
						sPRemoldTraitsSkillMergedPopup.Setup(modSkillDefinition.ID);
						sPRemoldTraitsSkillMergedPopup.Open();
					}
					UIEvent.Send("SPRemoldMakeModSkillSuccess", modSkillDefinition.ID);
				}
			}
		}
	}

	public void OnclickUpgrade()
	{
		if (Time.time - lastUpgradeClickTime < 0.2f)
		{
			return;
		}
		lastUpgradeClickTime = Time.time;
		if (!OfflineManager.IsFreeAll && !playerModel.ModSkillManager.CanUpgradeModSkill(modSkillDefinition.ID))
		{
			SPRemoldSkillNotEnoughPopup sPRemoldSkillNotEnoughPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldSkillNotEnoughPopup) as SPRemoldSkillNotEnoughPopup;
			if (sPRemoldSkillNotEnoughPopup != null)
			{
				sPRemoldSkillNotEnoughPopup.Open();
			}
			return;
		}
		ModSkillMode modSkillMode = playerModel.ModSkillManager.GetModSkillMode(modSkillDefinition.ID, modSkillDefinition.Type);
		if (OfflineManager.IsFakeExecuteCommands)
		{
			if (UpgradeModSkillCommandResult(modSkillDefinition.ID, modSkillDefinition.Type) == TWDModelResult.OK)
			{
				HUDNotification.Info(LocalizationManager.GetText("System.EquipRemold.FuncInfo7"));
				PlayTraitIconUpgradeFx();
				string parameter = ((modSkillMode != null) ? modSkillMode.ID : modSkillDefinition.ID);
				UIEvent.Send("SPRemoldUpgradeModSkillSuccess", parameter);
				UIEvent.Send("SPRemoldBagItemClick", parameter);
			}
		}
		else
		{
			if (Helpers.ExecuteCommand(new UpgradeModSkillCommand(modSkillDefinition.ID, modSkillDefinition.Type)) == TWDModelResult.OK)
			{
				HUDNotification.Info(LocalizationManager.GetText("System.EquipRemold.FuncInfo7"));
				PlayTraitIconUpgradeFx();
				string parameter = ((modSkillMode != null) ? modSkillMode.ID : modSkillDefinition.ID);
				UIEvent.Send("SPRemoldUpgradeModSkillSuccess", parameter);
				UIEvent.Send("SPRemoldBagItemClick", parameter);
			}
		}
	}

	private void PlayTraitIconUpgradeFx()
	{
		if (traitIcon == null)
		{
			return;
		}
		GameObject gameObject;
		try
		{
			gameObject = traitIconUpgradeFxPrefab;
		}
		catch (MissingReferenceException)
		{
			return;
		}
		if (!(gameObject == null))
		{
			GameObject gameObject2;
			try
			{
				gameObject2 = Helpers.InstantiateToParentAndLayer(gameObject, traitIcon.gameObject);
			}
			catch (MissingReferenceException)
			{
				return;
			}
			if (!(gameObject2 == null))
			{
				float num = Mathf.Max(traitIcon.width, traitIcon.height);
				float num2 = ((num > 0f) ? Mathf.Clamp(num / 200f, 0.1f, 0.45f) : 0.2f);
				gameObject2.transform.localScale = new Vector3(num2, num2, num2);
				gameObject2.transform.SetAsLastSibling();
				Object.Destroy(gameObject2, 3f);
			}
		}
	}

	public void OnclickUsedWeaponIcon()
	{
		if (modSkillMode != null && modSkillMode.EquipmentItemModel != null)
		{
			_ = modSkillMode.EquipmentItemModel;
		}
	}



	#region mycode
	public TWDModelResult MakeModSkillCommandResult(string ID, string GroupID, out ModSkillMode skill)
	{
		skill = null;
		var player = GameManager.Instance.playerModel;
		ModSkillManager modSkillManager = player.ModSkillManager;
		if (modSkillManager == null)
		{
			return TWDModelResult.Error;
		}
		Dictionary<CurrencyType, int> makingCost = modSkillManager.GetMakingCost(ID);
		if (makingCost == null)
		{
			return TWDModelResult.Error;
		}
		if (!modSkillManager.CanMakeModSkill(ID))
		{
			return TWDModelResult.Error;
		}
		if (modSkillManager.HasModSkillMode(GroupID))
		{
			return TWDModelResult.Error;
		}
		if (!OfflineManager.IsFreeAll)
		{
			Cashier cashier = new Cashier(player.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.MakeModSkill);
			foreach (KeyValuePair<CurrencyType, int> item in makingCost)
			{
				cashierItem.SetCost(item.Key, item.Value);
			}
			cashier.AddItem(cashierItem);
			if (cashier.Pay(player.manager) == TWDModelResult.OK && modSkillManager.MakeModSkill(ID, GroupID, currentFilterClass) == null)
			{
				return TWDModelResult.Error;
			}
		}
		else
		{
			skill = modSkillManager.MakeModSkill(ID, GroupID, currentFilterClass);
			if (skill == null)
			{
				return TWDModelResult.Error;
			}
		}
		return TWDModelResult.OK;
	}

	public TWDModelResult UpgradeModSkillCommandResult(string ID, string GroupID)
	{
		var player = GameManager.Instance.playerModel;
		ModSkillManager modSkillManager = player.ModSkillManager;
		if (modSkillManager == null)
		{
			return TWDModelResult.Error;
		}
		Dictionary<CurrencyType, int> upgradeModSkillCost = modSkillManager.GetUpgradeModSkillCost(ID);
		if (upgradeModSkillCost == null)
		{
			return TWDModelResult.Error;
		}
		if (!modSkillManager.CanUpgradeModSkill(ID))
		{
			return TWDModelResult.Error;
		}
		if (!OfflineManager.IsFreeAll)
		{
			Cashier cashier = new Cashier(player.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeModSkill);
			foreach (KeyValuePair<CurrencyType, int> item in upgradeModSkillCost)
			{
				cashierItem.SetCost(item.Key, item.Value);
			}
			cashier.AddItem(cashierItem);
			if (cashier.Pay(player.manager) != TWDModelResult.OK)
			{
				return TWDModelResult.Error;
			}
		}

		if (modSkillManager.UpgradeModSkill(ID, GroupID) != TWDModelResult.OK)
		{
			return TWDModelResult.Error;
		}
		return TWDModelResult.OK;
	}
	#endregion
}
