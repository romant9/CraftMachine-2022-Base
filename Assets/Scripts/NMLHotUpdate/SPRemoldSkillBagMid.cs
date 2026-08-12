using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillBagMid : MonoBehaviour
{
	private enum OrderType
	{
		Star = 0,
		Level = 1
	}

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryHaveItemPrefab;

	[SerializeField]
	private GameObject EntryNotHavePrefab;

	[SerializeField]
	private GameObject EntryLinePrefab;

	[SerializeField]
	private UILabel OrderText;

	public SurvivorClass filterSurvivorClass;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private string firstSkillId;

	private static string pendingSelectSkillIdAfterListRefresh;

	private OrderType currentOrderType;

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
		if (type == "SPRemoldChangeSurvivorClassFilter" && parameter != null && parameter is SurvivorClass)
		{
			Setup((SurvivorClass)parameter);
			StartCoroutine(DelaySelectFirst());
		}
	}

	public void Setup(SurvivorClass filterSurvivorClass)
	{
		this.filterSurvivorClass = filterSurvivorClass;
		UpdateUI();
	}

	public void UpdateUI()
	{
		FreshListData();
	}

	private void FreshListData()
	{
		ClearEntries();
		List<ModSkillMode> list = GameManager.Instance.playerModel.ModSkillManager.GetAcquiredModSkillsByClass(filterSurvivorClass);
		List<ModSkillMode> list2 = GameManager.Instance.playerModel.ModSkillManager.GetUnlockableModSkills(filterSurvivorClass);
		if (currentOrderType == OrderType.Star)
		{
			OrderText.text = LocalizationManager.GetText("System.EquipInfo.OrderStar");
			if (list != null && list.Count > 0)
			{
				list = (from a in list
					orderby a.GetSpTraitsDefaultTrait().Star descending, a.GetSpTraitsDefaultTrait().Level descending
					select a).ToList();
			}
			if (list2 != null && list2.Count > 0)
			{
				list2 = (from a in list2
					orderby a.ModSkillLockState == ModSkillLockState.CanUnlock descending, a.GetSpTraitsDefaultTrait().Star descending, a.GetSpTraitsDefaultTrait().Level descending
					select a).ToList();
			}
		}
		else
		{
			OrderText.text = LocalizationManager.GetText("System.EquipInfo.OrderLevel");
			if (list != null && list.Count > 0)
			{
				list = (from a in list
					orderby a.GetSpTraitsDefaultTrait().Level descending, a.GetSpTraitsDefaultTrait().Star descending
					select a).ToList();
			}
			if (list2 != null && list2.Count > 0)
			{
				list2 = (from a in list2
					orderby a.ModSkillLockState == ModSkillLockState.CanUnlock descending, a.GetSpTraitsDefaultTrait().Level descending, a.GetSpTraitsDefaultTrait().Star descending
					select a).ToList();
			}
		}
		firstSkillId = ((list.Count > 0) ? list[0].ID : ((list2.Count > 0) ? list2[0].ID : null));
		int num = 0;
		if (list != null && list.Count > 0)
		{
			num = (list.Count + 5) / 6;
		}
		int num2 = 0;
		if (list2 != null && list2.Count > 0)
		{
			num2 = (list2.Count + 5) / 6;
		}
		for (int num3 = 0; num3 < num; num3++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryHaveItemPrefab);
			if (gameObject.TryGetComponent<SPRemoldSkillBagMidLine>(out var component))
			{
				if (num3 < num - 1)
				{
					List<ModSkillMode> range = list.GetRange(num3 * 6, 6);
					component.Setup(range);
				}
				else
				{
					List<ModSkillMode> range2 = list.GetRange(num3 * 6, list.Count - num3 * 6);
					component.Setup(range2);
				}
			}
			Entries.Add(gameObject);
		}
		GameObject item = EntryContainer.AddChild(EntryLinePrefab);
		Entries.Add(item);
		for (int num4 = 0; num4 < num2; num4++)
		{
			GameObject gameObject2 = EntryContainer.AddChild(EntryNotHavePrefab);
			if (gameObject2.TryGetComponent<SPRemoldSkillBagMidLine>(out var component2))
			{
				if (num4 < num2 - 1)
				{
					List<ModSkillMode> range3 = list2.GetRange(num4 * 6, 6);
					component2.Setup(range3);
				}
				else
				{
					List<ModSkillMode> range4 = list2.GetRange(num4 * 6, list2.Count - num4 * 6);
					component2.Setup(range4);
				}
			}
			Entries.Add(gameObject2);
		}
		EntryContainer.GetComponent<UITable>().Reposition();
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public static void SetPendingSelectAfterListRefresh(string modSkillId)
	{
		pendingSelectSkillIdAfterListRefresh = modSkillId;
	}

	private IEnumerator DelaySelectFirst()
	{
		yield return null;
		string text = ((!string.IsNullOrEmpty(pendingSelectSkillIdAfterListRefresh)) ? pendingSelectSkillIdAfterListRefresh : firstSkillId);
		pendingSelectSkillIdAfterListRefresh = null;
		if (!string.IsNullOrEmpty(text))
		{
			UIEvent.Send("SPRemoldBagItemClick", text);
		}
	}

	public void OnclickOrder()
	{
		currentOrderType = ((currentOrderType == OrderType.Star) ? OrderType.Level : OrderType.Star);
		UpdateUI();
	}
}
