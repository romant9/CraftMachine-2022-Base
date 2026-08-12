using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BreakThroughLevelList : MonoBehaviour
{
	[SerializeField]
	private GameObject BTLevelEntryPrefab;

	[SerializeField]
	private GameObject BTLevelEntryContainer;

	private EquipmentItemModel equipmentItemModel;

	private readonly List<GameObject> BTLevelEntries = new List<GameObject>();

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void ShowBreakThroughLevelList()
	{
		if (this == null || equipmentItemModel == null)
		{
			return;
		}
		ClearBTLevelEntries();
		UITable component = BTLevelEntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = BTLevelEntryContainer.GetComponentInParent<UIScrollView>();
		int maxBreakThroughLevel = equipmentItemModel.GetMaxBreakThroughLevel();
		for (int i = 1; i <= maxBreakThroughLevel; i++)
		{
			GameObject gameObject = BTLevelEntryContainer.AddChild(BTLevelEntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<BreakThroughLevelEntry>(out var component2))
			{
				component2.SetContent(i, equipmentItemModel);
			}
			SetupEntryDrag(gameObject, componentInParent);
			BTLevelEntries.Add(gameObject);
		}
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private static void SetupEntryDrag(GameObject entry, UIScrollView scrollView)
	{
		if (!(entry == null) && !(scrollView == null))
		{
			NGUITools.AddWidgetCollider(entry);
			Helpers.AddComponent<UIDragScrollView>(entry).scrollView = scrollView;
		}
	}

	private void ClearBTLevelEntries()
	{
		for (int i = 0; i < BTLevelEntries.Count; i++)
		{
			NGUITools.Destroy(BTLevelEntries[i]);
		}
		BTLevelEntries.Clear();
	}

	public void UpdateUI()
	{
		ShowBreakThroughLevelList();
	}

	public void InitData(EquipmentItemModel itemModel)
	{
		equipmentItemModel = itemModel;
		UpdateUI();
	}
}
