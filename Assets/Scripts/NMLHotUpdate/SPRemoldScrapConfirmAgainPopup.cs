using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldScrapConfirmAgainPopup : HUDElement
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	private EquipmentItemModel equipmentItemModel;

	private Callback okCallback;

	private readonly List<GameObject> Entries = new List<GameObject>();

	public void Setup(EquipmentItemModel equipmentItemModel)
	{
		this.equipmentItemModel = equipmentItemModel;
		UpdateUI();
	}

	public new void UpdateUI()
	{
		FreshListData();
	}

	private void FreshListData()
	{
		ClearEntries();
		Rewards equipmentListScrapReward = GameManager.Instance.modelManager.Player.Equipment.GetEquipmentListScrapReward(new List<EquipmentItemModel> { equipmentItemModel });
		UITable component = EntryContainer.GetComponent<UITable>();
		int count = equipmentListScrapReward.RewardsList.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<SPRemoldScrapItemPreview>(out var component2))
			{
				component2.Setup(equipmentListScrapReward.RewardsList[i]);
			}
			Entries.Add(gameObject);
		}
		component.Reposition();
		contentLabel.text = LocalizationManager.GetText("Popup.ScrapConfirmation.Message{equipmentName}", HelpersLocalization.GetEquipmentName(equipmentItemModel.Definition.ID));
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public void SetCallbacks(Callback okCallback = null)
	{
		this.okCallback = okCallback;
	}

	public void OnclickConfirm()
	{
		if (okCallback != null)
		{
			okCallback();
		}
		Close();
	}
}
