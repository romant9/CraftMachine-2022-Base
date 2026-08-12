using System;
using TWDModel;
using UnityEngine;

public class EndScreenOpenLoot : MonoBehaviour
{
	public GameObject openLootButton;

	public GameObject openedLoot;

	public GameObject openEffect;

	public UILabel weaponNameLabel;

	public UILabel weaponDescriptionLabel;

	public UILabel weaponRarityLabel;

	public UILabel weaponDamageLabel;

	public UISprite weaponIcon;

	private EquipmentItemModel equipmentItemModel;

	public event OpenedHandler Opened;

	public void SetEquipment(EquipmentItemModel itemModel)
	{
		equipmentItemModel = itemModel;
	}

	private void OnEnable()
	{
		UIEventListener uIEventListener = UIEventListener.Get(openLootButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClicked));
		openLootButton.SetActive(value: true);
		openedLoot.SetActive(value: false);
	}

	private void OnDisable()
	{
		UIEventListener uIEventListener = UIEventListener.Get(openLootButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClicked));
	}

	private void NotifyOpened()
	{
		this.Opened?.Invoke();
	}

	private void OpenBox(string damage)
	{
		openLootButton.SetActive(value: false);
		openedLoot.SetActive(value: true);
		weaponNameLabel.text = HelpersLocalization.GetEquipmentName(equipmentItemModel);
		weaponRarityLabel.text = HelpersLocalization.GetRarityLevel(equipmentItemModel.RarityLevel);
		weaponRarityLabel.color = HelpersGfx.GetRarityColor(equipmentItemModel.RarityLevel);
		weaponDamageLabel.text = equipmentItemModel.Damage.ToString();
		weaponDamageLabel.color = new Color(0f, 0f, 0f);
		EquipmentResourceEntry equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(equipmentItemModel);
		if (equipmentResourceEntry != null && !string.IsNullOrEmpty(equipmentResourceEntry.IconSprite))
		{
			weaponIcon.spriteName = equipmentResourceEntry.IconSprite + "_Full";
			if (weaponIcon.GetAtlasSprite() == null)
			{
				weaponIcon.spriteName = equipmentResourceEntry.IconSprite;
			}
		}
		if (weaponIcon.GetAtlasSprite() != null)
		{
			int num = Mathf.Min(weaponIcon.GetAtlasSprite().height, 100);
			float num2 = (float)weaponIcon.GetAtlasSprite().width / (float)weaponIcon.GetAtlasSprite().height;
			weaponIcon.width = (int)((float)num * num2);
			weaponIcon.height = num;
		}
		GameObject obj = UnityEngine.Object.Instantiate(openEffect);
		obj.transform.parent = base.transform;
		obj.transform.localPosition = new Vector3(-150f, -30f, 0f);
		NotifyOpened();
	}

	private void OnClicked(GameObject button)
	{
		OpenBox(50.ToString() ?? "");
	}
}
