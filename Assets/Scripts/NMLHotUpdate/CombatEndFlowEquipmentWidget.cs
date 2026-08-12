using TWDModel;
using UnityEngine;

public class CombatEndFlowEquipmentWidget : CombatEndWidget
{
	[SerializeField]
	private GameObject missionRewardSpecificEquipmentContainer;

	[SerializeField]
	private GameObject scrappedLabel;

	[SerializeField]
	private GameObject equipmentCardPrefab;

	private EquipmentItemModel equipmentItemModel;

	public override void OnEnable()
	{
		base.OnEnable();
		Helpers.GameObjectSetActive(scrappedLabel, value: false);
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnEquipmentUpdated")
		{
			bool flag = equipmentItemModel != null && equipmentItemModel.manager != null && equipmentItemModel.manager.Player.Equipment.Contains(equipmentItemModel);
			if (missionRewardSpecificEquipmentContainer != null)
			{
				missionRewardSpecificEquipmentContainer.SetActive(flag);
			}
			Helpers.GameObjectSetActive(scrappedLabel, !flag);
		}
	}

	public void SetEquipment(EquipmentItemModel equipmentItemModel)
	{
		this.equipmentItemModel = equipmentItemModel;
		if (missionRewardSpecificEquipmentContainer != null && equipmentItemModel != null)
		{
			missionRewardSpecificEquipmentContainer.RemoveAllChildren();
			EquipmentButton component = Helpers.InstantiateToParentAndLayer(equipmentCardPrefab, missionRewardSpecificEquipmentContainer).GetComponent<EquipmentButton>();
			if (component != null)
			{
				component.Setup(equipmentItemModel, null, null, "OnNewEquipmentCardSelected", showOwnerAndUpgradeIndicator: false);
				component.OpenEquipmentReceivedOnClick = true;
			}
		}
	}
}
