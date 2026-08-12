using TWDModel;
using UnityEngine;

public class UpgradePathUILevel : MonoBehaviour
{
	[SerializeField]
	private GameObject doneGameObject;

	[SerializeField]
	private GameObject lockedGameObject;

	[SerializeField]
	private GameObject nextGameObject;

	[SerializeField]
	private UISprite icon;

	public int Level { get; set; }

	public UpgradeTraitsData TraitUpgradeData { get; set; }

	public EquipmentItemModel Equipment { get; set; }

	public bool IsBigUpgrade => TraitUpgradeData != null;

	public void ShowDone()
	{
		doneGameObject.SetActive(value: true);
		lockedGameObject.SetActive(value: false);
		nextGameObject.SetActive(value: false);
		_ = TraitUpgradeData;
	}

	public void ShowLocked()
	{
		doneGameObject.SetActive(value: false);
		lockedGameObject.SetActive(value: true);
		nextGameObject.SetActive(value: false);
		_ = TraitUpgradeData;
	}

	public void ShowNext()
	{
		doneGameObject.SetActive(value: false);
		lockedGameObject.SetActive(value: false);
		nextGameObject.SetActive(value: true);
		_ = TraitUpgradeData;
	}

	public void UpdateUI()
	{
		if (!(icon != null))
		{
			return;
		}
		if (Equipment != null && TraitUpgradeData.IsTactical)
		{
			EquipmentResourceEntry equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(Equipment);
			if (equipmentResourceEntry == null)
			{
				Debug.LogError("Could not load equipment resources prefab " + Equipment.Definition.ChargeEquipmentIdentifier + "!");
			}
			else
			{
				icon.spriteName = equipmentResourceEntry.IconSprite.Replace("Charge", "Trait_Charge");
			}
		}
		else
		{
			icon.spriteName = HelpersGfx.GetEquipmentTraitIconName(TraitUpgradeData);
		}
	}

	public void OnClick()
	{
		UIEvent.Send("UpgradePahtLevelClicked", this);
	}
}
