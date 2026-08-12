using TWDModel;
using UnityEngine;

public class UpgradePathUI : MonoBehaviour
{
	[Header("Upgrades")]
	[SerializeField]
	private GameObject smallUpgradePrefab;

	[SerializeField]
	private GameObject bigUpgradePrefab;

	[SerializeField]
	private int pixelsBetweenLevels;

	[SerializeField]
	private GameObject upgradesContainer;

	[SerializeField]
	private UISprite background;

	[Header("Info")]
	[SerializeField]
	private GameObject infoContainerSurvivor;

	[SerializeField]
	private GameObject infoContainerEquipment;

	[SerializeField]
	private UILabel infoLabelSurvivor;

	[SerializeField]
	private UILabel infoLabelEquipment;

	[SerializeField]
	private GameObject infoConnector;

	[SerializeField]
	private GameObject lockedUpdateSurvivor;

	[SerializeField]
	private GameObject lockedUpdateEquipment;

	private UpgradePathUILevel[] upgradesLevel;

	private UpgradePathData upgradePathData;

	private float originalBackgroundWidth = -1f;

	public bool ShowNextLevel { get; set; }

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void Init(UpgradePathData upgradePathData)
	{
		this.upgradePathData = upgradePathData;
		if (originalBackgroundWidth < 0f)
		{
			originalBackgroundWidth = background.width;
		}
		infoContainerSurvivor.SetActive(upgradePathData.Survivor != null);
		infoContainerEquipment.SetActive(upgradePathData.Equipment != null);
		if (upgradesLevel != null)
		{
			UpgradePathUILevel[] array = upgradesLevel;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<CacheableObject>().Destroy();
			}
		}
		upgradesLevel = new UpgradePathUILevel[upgradePathData.MaxLevel - upgradePathData.StartLevel];
		float num = 0f;
		for (int j = upgradePathData.StartLevel; j < upgradePathData.MaxLevel; j++)
		{
			UpgradeTraitsData upgradeData = upgradePathData.GetUpgradeData(j);
			UpgradePathUILevel component = Helpers.InstantiateToParent((upgradeData == null) ? smallUpgradePrefab : bigUpgradePrefab, upgradesContainer).GetComponent<UpgradePathUILevel>();
			component.Level = j;
			component.TraitUpgradeData = upgradeData;
			component.Equipment = this.upgradePathData.Equipment;
			component.UpdateUI();
			upgradesLevel[j - upgradePathData.StartLevel] = component;
			Vector3 localPosition = component.transform.localPosition;
			localPosition.x = num;
			component.transform.localPosition = localPosition;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(component.transform);
			num += (float)pixelsBetweenLevels + bounds.extents.x * 2f;
		}
	}

	public void UpdateUI(int currentLevel)
	{
		upgradePathData.CurrentLevel = currentLevel;
		UpgradePathUILevel upgradePathUILevel = null;
		UpgradePathUILevel upgradePathUILevel2 = null;
		for (int i = upgradePathData.StartLevel; i < upgradePathData.MaxLevel; i++)
		{
			UpgradePathUILevel upgradePathUILevel3 = upgradesLevel[i - upgradePathData.StartLevel];
			if (currentLevel > i)
			{
				if (upgradePathUILevel3.IsBigUpgrade)
				{
					upgradePathUILevel = upgradePathUILevel3;
				}
				upgradePathUILevel3.ShowDone();
				continue;
			}
			if (upgradePathUILevel2 == null && upgradePathUILevel3.IsBigUpgrade)
			{
				upgradePathUILevel2 = upgradePathUILevel3;
			}
			if (ShowNextLevel && currentLevel == i)
			{
				upgradePathUILevel3.ShowNext();
			}
			else
			{
				upgradePathUILevel3.ShowLocked();
			}
		}
		ShowUpgradeInfo(upgradePathUILevel2 ?? upgradePathUILevel);
	}

	private void ShowUpgradeInfo(UpgradePathUILevel upgradePathUiLevel)
	{
		bool flag = upgradePathUiLevel != null;
		infoLabelSurvivor.gameObject.SetActive(flag);
		infoLabelEquipment.gameObject.SetActive(flag);
		lockedUpdateSurvivor.SetActive(flag);
		lockedUpdateEquipment.SetActive(flag);
		infoConnector.SetActive(flag);
		if (!flag)
		{
			return;
		}
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(upgradePathUiLevel.TraitUpgradeData.Identifier);
		if (traitDefinition == null)
		{
			Debug.LogError("Trait definition not found for " + upgradePathUiLevel.TraitUpgradeData.Identifier);
			return;
		}
		if (upgradePathData.Equipment != null && upgradePathUiLevel.TraitUpgradeData.IsTactical)
		{
			infoLabelSurvivor.text = HelpersLocalization.GetChargeEquipmentTraitDescription(upgradePathData.Equipment);
			infoLabelEquipment.text = HelpersLocalization.GetChargeEquipmentTraitDescription(upgradePathData.Equipment);
		}
		else if (traitDefinition != null)
		{
			infoLabelSurvivor.text = HelpersLocalization.GetInstantiatedTraitDescription(upgradePathUiLevel.TraitUpgradeData);
			infoLabelEquipment.text = HelpersLocalization.GetInstantiatedTraitDescription(upgradePathUiLevel.TraitUpgradeData);
		}
		lockedUpdateSurvivor.SetActive(upgradePathData.CurrentLevel < upgradePathUiLevel.Level);
		lockedUpdateEquipment.SetActive(upgradePathData.CurrentLevel < upgradePathUiLevel.Level);
		Vector3 position = infoConnector.transform.position;
		position.x = upgradePathUiLevel.transform.position.x;
		infoConnector.transform.position = position;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "UpgradePahtLevelClicked")
		{
			ShowUpgradeInfo(parameter as UpgradePathUILevel);
		}
	}
}
