using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIScrollView))]
public class NUIScrollableList : MonoBehaviourExtended
{
	[Tooltip("At Reset() and Awake() override the NGUI UIScrollView settings with our general settings for UIScrollView lists.")]
	public bool overrideDefaults = true;

	[SerializeField]
	[Tooltip("Whether or not only visible elements should be created. Use this for extremely large lists that could otherwise exhibit performance issues.")]
	private bool createVisibleOnly;

	private List<NUIListItemBase> itemsList = new List<NUIListItemBase>();

	private List<NUIListItemBase> hiddenItems = new List<NUIListItemBase>();

	private List<object> dataObjects = new List<object>();

	private NUIListItemBase beforeScrollPadder;

	private NUIListItemBase afterScrollPadder;

	private UIScrollView uiScrollViewRef;

	private Vector3 newPosition = new Vector3(0f, 0f, 0f);

	private GameObject newItem;

	private Vector2 scrollPosition = new Vector2(0f, 0f);

	public UIScrollView uiScrollView
	{
		get
		{
			if (uiScrollViewRef == null)
			{
				uiScrollViewRef = GetComponent<UIScrollView>();
			}
			return uiScrollViewRef;
		}
		set
		{
			uiScrollViewRef = value;
		}
	}

	public int currentItemsCount
	{
		get
		{
			if (itemsList != null)
			{
				return itemsList.Count;
			}
			return 0;
		}
	}

	public List<NUIListItemBase> currentItemsList
	{
		get
		{
			if (itemsList == null)
			{
				itemsList = new List<NUIListItemBase>();
			}
			return itemsList;
		}
	}

	public virtual void Awake()
	{
		OverrideDefaults();
	}

	private void Reset()
	{
	}

	public NUIListItemBase InstantiateAdd(string prefabSrc)
	{
		if (IsUseCustomPrefabs)
		{
			if (!CustomSource) return null;

			return InstantiateAdd(CustomSource);
		}
		if (!string.IsNullOrEmpty(prefabSrc))
		{
			newItem = UnityUtils.LoadFromAssetBundle(prefabSrc, "uilistitems") as GameObject;
			if (newItem != null)
			{
				return InstantiateAdd(newItem);
			}
			Debug.LogWarning("NUIScrollableList: Could not LoadAsset from src: " + prefabSrc);
		}
		else
		{
			Debug.LogWarning("NUIScrollableList: Cannot LoadAsset with NULL or Empty src");
		}
		return null;
	}

	public NUIListItemBase InstantiateAdd(GameObject prefab)
	{
		if (prefab != null)
		{
			NUIListItemBase nUIListItemBase = Helpers.InstantiateWithComponent<NUIListItemBase>(prefab, base.gameObject);
			if (OfflineManager.IsLoadDataManager)
			{
				nUIListItemBase.transform.localScale = Vector3.one * ScaleFactor;
			}
			if (nUIListItemBase != null)
			{
				AddItem(nUIListItemBase);
				return nUIListItemBase;
			}
			Debug.LogWarning("NUIScrollableList: Could not instantiate " + prefab);
		}
		else
		{
			Debug.LogWarning("NUIScrollableList: Given prefab is NULL");
		}
		return null;
	}

	public virtual void AddItem(NUIListItemBase item)
	{
		if (item != null)
		{
			if (itemsList == null)
			{
				itemsList = new List<NUIListItemBase>();
			}
			itemsList.Add(item);
			item.AddedToParent(this);
		}
		else
		{
			Debug.LogWarning("NUIScrollableList: Can't Add() NULL object!");
		}
	}

	public virtual void RemoveItem(NUIListItemBase item)
	{
		if (item != null)
		{
			if (itemsList != null && itemsList.Contains(item))
			{
				itemsList.Remove(item);
				item.RemovedFromParent(this);
			}
			item.Clear();
			if (item.gameObject != null)
			{
				Helpers.DestroyOrCache(item.gameObject);
			}
		}
		else
		{
			Debug.LogWarning("NUIScrollableList: Can't Add() NULL object!");
		}
	}

	private void UpdateScrollPadders()
	{
		if (!(uiScrollViewRef == null) && !(uiScrollViewRef.panel == null) && itemsList.Count != 0 && createVisibleOnly && !(beforeScrollPadder == null) && !(afterScrollPadder == null) && uiScrollViewRef.movement == UIScrollView.Movement.Vertical)
		{
			BoxCollider component = beforeScrollPadder.GetComponent<BoxCollider>();
			if (component == null)
			{
				Debug.LogError("Empty padding prefab collider is null.");
				return;
			}
			Vector3 size = component.size;
			int num = Mathf.Clamp((int)(uiScrollViewRef.panel.GetViewSize().x / size.x), 1, int.MaxValue);
			Vector3 localPosition = beforeScrollPadder.transform.localPosition + new Vector3(0f, (0f - (float)(dataObjects.Count / num)) * size.y, 0f);
			afterScrollPadder.transform.localPosition = localPosition;
		}
	}

	public void UpdateVisibleItems<T>(string prefabSrc) where T : class
	{
		if (uiScrollView == null || uiScrollView.panel == null || !createVisibleOnly || dataObjects.Count == 0)
		{
			return;
		}
		UpdateScrollPadders();
		BoxCollider component = beforeScrollPadder.GetComponent<BoxCollider>();
		if (component == null)
		{
			Debug.LogError("Empty padding prefab collider is null.");
			return;
		}
		Vector3 size = component.size;
		Vector3 localPosition = beforeScrollPadder.transform.localPosition;
		int num = (int)(uiScrollView.panel.GetViewSize().x / size.x);
		Bounds bounds = uiScrollView.bounds;
		Vector4 finalClipRegion = uiScrollView.panel.finalClipRegion;
		Vector3 vector = uiScrollView.panel.localCorners[1];
		int num2 = (int)(finalClipRegion.w / size.y);
		int num3 = (int)((bounds.max.y - vector.y) / size.y) * num;
		int num4 = num3 + (num2 + 2) * num - 1;
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num4 < 0)
		{
			num4 = 0;
		}
		int num5 = num4 - num3;
		if (itemsList.Count > num5)
		{
			for (int i = 0; i < itemsList.Count; i++)
			{
				NUIListItem<T> nUIListItem = itemsList[i] as NUIListItem<T>;
				if (!(nUIListItem == null))
				{
					int num6 = dataObjects.IndexOf(nUIListItem.GetData());
					if (num6 >= 0 && (num6 < num3 || num6 > num4))
					{
						hiddenItems.Add(nUIListItem);
						itemsList.RemoveAt(i);
						i--;
					}
				}
			}
		}
		for (int j = num3; j <= num4 && j < dataObjects.Count; j++)
		{
			T val = dataObjects[j] as T;
			NUIListItem<T> nUIListItem2 = null;
			for (int k = 0; k < itemsList.Count; k++)
			{
				NUIListItem<T> nUIListItem3 = itemsList[k] as NUIListItem<T>;
				if (nUIListItem3 != null && nUIListItem3.GetData() == val)
				{
					nUIListItem2 = nUIListItem3;
					break;
				}
			}
			if (nUIListItem2 == null)
			{
				if (hiddenItems.Count == 0)
				{
					nUIListItem2 = InstantiateAdd(prefabSrc) as NUIListItem<T>;
				}
				else
				{
					nUIListItem2 = hiddenItems[hiddenItems.Count - 1] as NUIListItem<T>;
					hiddenItems.RemoveAt(hiddenItems.Count - 1);
					itemsList.Add(nUIListItem2);
				}
				nUIListItem2.SetData(val);
				nUIListItem2.UpdateUI();
				nUIListItem2.transform.localPosition = localPosition + new Vector3((float)(j % num) * size.x, (0f - (float)(j / num)) * size.y, 0f);
			}
		}
		for (int l = 0; l < hiddenItems.Count; l++)
		{
			RemoveItem(hiddenItems[l]);
		}
		hiddenItems.Clear();
	}

	public void UpdateWithList<T>(IList<T> list, string prefabSrc, string emptyPrefabSrc, bool callUpdateUI = false) where T : class
	{
		if (list != null && list.GetType() == typeof(T[]))
		{
			Debug.LogError("Error currently list does not support Arrays");
		}
		else if (list != null && list.Count > 0)
		{
			Clear();
			dataObjects.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				T val = list[i];
				if (val != null)
				{
					dataObjects.Add(val);
				}
			}
			if (uiScrollViewRef != null && createVisibleOnly && uiScrollViewRef.movement != UIScrollView.Movement.Vertical)
			{
				Debug.LogError("Currently it's only possible to limit the number of visible items on vertical layouts.");
				createVisibleOnly = false;
			}
			if (createVisibleOnly)
			{
				NUIListItem<T> nUIListItem = InstantiateAdd(prefabSrc) as NUIListItem<T>;
				nUIListItem.SetData(dataObjects[0] as T);
				if (callUpdateUI)
				{
					nUIListItem.UpdateUI();
				}
				if (beforeScrollPadder == null)
				{
					GameObject prefab;
					if (IsUseCustomPrefabs)
					{
						prefab = CustomScrollPadder;
					}
					else
					{
						prefab = UnityUtils.LoadFromAssetBundle(emptyPrefabSrc, "uilistitems") as GameObject;
					}
					if (prefab == null)
					{
						DebugTWD.LogError("Choose beforeScrollPadder prefab");
						return;
					}
					beforeScrollPadder = Helpers.InstantiateWithComponent<NUIListItemBase>(prefab, base.gameObject);
					beforeScrollPadder.AddedToParent(this);
					afterScrollPadder = Helpers.InstantiateWithComponent<NUIListItemBase>(prefab, base.gameObject);
					afterScrollPadder.AddedToParent(this);
				}
				if (itemsList.Count > 0)
				{
					beforeScrollPadder.transform.localPosition = itemsList[0].transform.localPosition;
				}
				return;
			}
			for (int j = 0; j < dataObjects.Count; j++)
			{
				InstantiateAdd(prefabSrc);
				NUIListItem<T> nUIListItem2 = ((itemsList.Count > j) ? (itemsList[j] as NUIListItem<T>) : null);
				if (nUIListItem2 == null)
				{
					Debug.LogWarning("NUIScrollableList: Encountered invalid typed list item.");
					continue;
				}
				nUIListItem2.SetData(dataObjects[j] as T);
				if (callUpdateUI)
				{
					nUIListItem2.UpdateUI();
				}
			}
		}
		else
		{
			Clear();
		}
	}

	public void UpdateUIICurrentItems()
	{
		if (currentItemsList == null)
		{
			return;
		}
		for (int i = 0; i < currentItemsList.Count; i++)
		{
			if (currentItemsList[i] != null)
			{
				currentItemsList[i].UpdateUI();
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		dataObjects.Clear();
		if (itemsList != null)
		{
			for (int i = 0; i < itemsList.Count; i++)
			{
				if (itemsList[i] != null)
				{
					itemsList[i].Clear();
					Helpers.DestroyOrCache(itemsList[i].gameObject);
				}
			}
			newItem = null;
			itemsList.Clear();
		}
		scrollPosition = new Vector2(0f, 0f);
	}

	[ContextMenu("SaveCurrentScrollPosition")]
	public void SaveCurrentScrollPosition()
	{
		if (uiScrollView != null && GetItemAtIndex(0) != null && GetItemAtIndex(-1) != null)
		{
			Vector2 vector = uiScrollView.panel.GetViewSize() * 0.5f;
			Vector2 vector2 = new Vector2(0f, 0f);
			vector2.x = 0f - vector.x + GetItemAtIndex(0).GetLocalSizeHalf(useLocalScale: true).x;
			vector2.y = vector.y - GetItemAtIndex(0).GetLocalSizeHalf(useLocalScale: true).y;
			Vector2 vector3 = new Vector2(0f, 0f);
			vector3.x = GetItemAtIndex(0).GetLocalSizeHalf(useLocalScale: true).x - uiScrollView.bounds.size.x + vector.x;
			vector3.y = 0f - GetItemAtIndex(0).GetLocalSizeHalf(useLocalScale: true).y + uiScrollView.bounds.size.y - vector.y;
			scrollPosition.x = Mathf.InverseLerp(vector2.x, vector3.x, uiScrollView.panel.transform.localPosition.x);
			scrollPosition.y = Mathf.InverseLerp(vector2.y, vector3.y, uiScrollView.panel.transform.localPosition.y);
		}
	}

	[ContextMenu("ReturnToSavedScrollPosition")]
	public void ReturnToSavedScrollPosition()
	{
		if (!(uiScrollView != null))
		{
			return;
		}
		if ((scrollPosition.x > 0f || scrollPosition.y > 0f) && ShouldScroll())
		{
			if (uiScrollView.movement == UIScrollView.Movement.Horizontal)
			{
				uiScrollView.SetDragAmount(scrollPosition.x, 0f, updateScrollbars: false);
			}
			else if (uiScrollView.movement == UIScrollView.Movement.Vertical)
			{
				uiScrollView.SetDragAmount(0f, scrollPosition.y, updateScrollbars: false);
			}
			else if (uiScrollView.movement == UIScrollView.Movement.Custom)
			{
				uiScrollView.SetDragAmount(scrollPosition.x, scrollPosition.y, updateScrollbars: false);
			}
		}
		else
		{
			uiScrollView.ResetPosition();
		}
	}

	[ContextMenu("RepositionItemsToMovement")]
	public void RepositionItemsToMovement()
	{
		if (uiScrollView != null)
		{
			if (uiScrollView.movement == UIScrollView.Movement.Horizontal)
			{
				RepositionItemsHorizontal();
			}
			else if (uiScrollView.movement == UIScrollView.Movement.Vertical)
			{
				RepositionItemsVertical();
			}
		}
	}

	[ContextMenu("RepostitionItemsHorizontal")]
	public void RepositionItemsHorizontal()
	{
		NUIListItemBase nUIListItemBase = null;
		NUIListItemBase nUIListItemBase2 = null;
		for (int i = 0; i < itemsList.Count; i++)
		{
			nUIListItemBase = itemsList[i];
			if (nUIListItemBase != null)
			{
				newPosition = new Vector3(0f, 0f, 0f);
				if (nUIListItemBase2 != null)
				{
					newPosition.x = nUIListItemBase2.GetLocalCorners(useLocalScale: true)[3].x + nUIListItemBase.GetLocalSizeHalf(useLocalScale: true).x;
					newPosition.y = 0f;
				}
				itemsList[i].SetPosition(newPosition, i);
			}
			nUIListItemBase2 = nUIListItemBase;
		}
	}

	[ContextMenu("RepostitionItemsVertical")]
	public void RepositionItemsVertical()
	{
		NUIListItemBase nUIListItemBase = null;
		NUIListItemBase nUIListItemBase2 = null;
		for (int i = 0; i < itemsList.Count; i++)
		{
			nUIListItemBase = itemsList[i];
			if (nUIListItemBase != null)
			{
				newPosition = new Vector3(0f, 0f, 0f);
				if (nUIListItemBase2 != null)
				{
					newPosition.x = 0f;
					newPosition.y = nUIListItemBase2.GetLocalCorners()[3].y - nUIListItemBase.GetLocalSizeHalf(useLocalScale: true).y;
				}
				itemsList[i].SetPosition(newPosition, i);
			}
			nUIListItemBase2 = nUIListItemBase;
		}
	}

	[ContextMenu("RepostitionItemsFillDownwards")]
	public void RepositionItemsFillDownwards()
	{
		NUIListItemBase nUIListItemBase = null;
		NUIListItemBase nUIListItemBase2 = null;
		if (!(uiScrollView != null) || !(uiScrollView.panel != null))
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < itemsList.Count; i++)
		{
			nUIListItemBase = itemsList[i];
			if (nUIListItemBase != null)
			{
				newPosition = new Vector3(0f, 0f, 0f);
				if (nUIListItemBase2 != null)
				{
					float num3 = nUIListItemBase2.GetLocalCorners()[3].x + nUIListItemBase.GetLocalSize(useLocalScale: true).x;
					float z = uiScrollView.panel.finalClipRegion.z;
					if (num3 > z)
					{
						newPosition.x = 0f;
						newPosition.y = num - nUIListItemBase.GetLocalSizeHalf(useLocalScale: true).y;
						num2 = num;
					}
					else
					{
						newPosition.x = nUIListItemBase2.GetLocalCorners()[3].x + nUIListItemBase.GetLocalSizeHalf(useLocalScale: true).x;
						newPosition.y = ((num2 == 0f) ? 0f : (num2 - nUIListItemBase.GetLocalSizeHalf(useLocalScale: true).y));
					}
				}
				itemsList[i].SetPosition(newPosition, i);
			}
			nUIListItemBase2 = nUIListItemBase;
			if (nUIListItemBase2.GetLocalCorners()[3].y < num)
			{
				num = nUIListItemBase2.GetLocalCorners()[3].y;
			}
		}
	}

	[ContextMenu("ResetScrollPosition")]
	public void ResetScrollPosition()
	{
		if (uiScrollView != null)
		{
			scrollPosition = new Vector2(0f, 0f);
			uiScrollView.ResetPosition();
		}
	}

	[ContextMenu("Sort")]
	public void Sort()
	{
		if (itemsList != null)
		{
			itemsList.StableSort((NUIListItemBase a, NUIListItemBase b) => b.GetSortValue().CompareTo(a.GetSortValue()));
		}
	}

	[ContextMenu("SortAndReset")]
	public void SortAndReset()
	{
		Sort();
		RepositionItemsToMovement();
		ResetScrollPosition();
	}

	[ContextMenu("SortAndReposition")]
	public void SortAndRepositionItems()
	{
		Sort();
		RepositionItemsToMovement();
		if (uiScrollView != null)
		{
			uiScrollView.InvalidateBounds();
		}
	}

	public Vector2 GetScrollPosition()
	{
		return scrollPosition;
	}

	public void SetScrollPosition(Vector2 newPosition)
	{
		scrollPosition = newPosition;
		ReturnToSavedScrollPosition();
	}

	public Vector3 GetCurrentScrollPanelLocalPosition()
	{
		if (uiScrollView != null && uiScrollView.panel != null)
		{
			return uiScrollView.panel.transform.localPosition;
		}
		return Vector3.zero;
	}

	public void RestoreScrollPanelLocalPosition(Vector3 localPosition)
	{
		if (!(uiScrollView == null) && !(uiScrollView.panel == null))
		{
			uiScrollView.panel.transform.localPosition = localPosition;
			uiScrollView.InvalidateBounds();
			uiScrollView.RestrictWithinBounds(instant: true);
		}
	}

	public NUIListItemBase GetItemAtIndex(int index)
	{
		if (currentItemsList != null)
		{
			index = ((index < 0) ? (currentItemsList.Count - 1) : index);
			if (currentItemsList.Count > index)
			{
				return currentItemsList[index];
			}
		}
		return null;
	}

	[ContextMenu("OverrideDefaults_EDITOR")]
	public virtual void OverrideDefaults()
	{
		if (overrideDefaults && uiScrollView != null)
		{
			uiScrollView.dragEffect = UIScrollView.DragEffect.MomentumAndSpring;
			uiScrollView.scrollWheelFactor = 0f;
			uiScrollView.momentumAmount = 60f;
			uiScrollView.restrictWithinPanel = true;
			uiScrollView.disableDragIfFits = true;
			uiScrollView.iOSDragEmulation = false;
		}
	}

	private bool ShouldScroll()
	{
		if (uiScrollView.movement == UIScrollView.Movement.Horizontal)
		{
			return uiScrollView.shouldMoveHorizontally;
		}
		if (uiScrollView.movement == UIScrollView.Movement.Vertical)
		{
			return uiScrollView.shouldMoveVertically;
		}
		return true;
	}


	#region myparams
	public bool IsUseCustomPrefabs;
	public GameObject CustomSource;
	public GameObject CustomScrollPadder;
	public float ScaleFactor = 1f;
	#endregion

	#region mycode
	public void UpdateWithList<T>(IList<T> list, GameObject prefabSrc, GameObject emptyPrefabSrc, bool callUpdateUI = false) where T : class
	{
		if (list != null && list.GetType() == typeof(T[]))
		{
			DebugTWD.LogError("Error currently list does not support Arrays");
		}
		else if (list != null && list.Count > 0)
		{
			Clear();
			dataObjects.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				T val = list[i];
				if (val != null)
				{
					dataObjects.Add(val);
				}
			}
			if (uiScrollViewRef != null && createVisibleOnly && uiScrollViewRef.movement != UIScrollView.Movement.Vertical)
			{
				DebugTWD.LogError("Currently it's only possible to limit the number of visible items on vertical layouts.");
				createVisibleOnly = false;
			}
			if (createVisibleOnly)
			{
				NUIListItem<T> nUIListItem = InstantiateAdd(prefabSrc) as NUIListItem<T>;
				nUIListItem.SetData(dataObjects[0] as T);
				if (callUpdateUI)
				{
					nUIListItem.UpdateUI();
				}
				if (beforeScrollPadder == null)
				{
					GameObject prefab = Instantiate(emptyPrefabSrc);
					prefab.SetActive(true);
					beforeScrollPadder = Helpers.InstantiateWithComponent<NUIListItemBase>(prefab, base.gameObject);
					beforeScrollPadder.AddedToParent(this);
					afterScrollPadder = Helpers.InstantiateWithComponent<NUIListItemBase>(prefab, base.gameObject);
					afterScrollPadder.AddedToParent(this);
				}
				if (itemsList.Count > 0)
				{
					beforeScrollPadder.transform.localPosition = itemsList[0].transform.localPosition;
				}
				return;
			}
			for (int j = 0; j < dataObjects.Count; j++)
			{
				InstantiateAdd(prefabSrc);
				NUIListItem<T> nUIListItem2 = ((itemsList.Count > j) ? (itemsList[j] as NUIListItem<T>) : null);
				if (nUIListItem2 == null)
				{
					DebugTWD.LogWarning("NUIScrollableList: Encountered invalid typed list item.");
					continue;
				}
				nUIListItem2.SetData(dataObjects[j] as T);
				if (callUpdateUI)
				{
					nUIListItem2.UpdateUI();
				}
			}
		}
		else
		{
			Clear();
		}
	}

	[ContextMenu("ScaleAllItems")]
	public void ScaleAllItems()
	{
		if (itemsList.Count > 0)
		{
			for (int i = 0; i < itemsList.Count; i++)
			{
				itemsList[i].transform.localScale = Vector3.one * ScaleFactor;
			}
			UpdateUIICurrentItems();
		}
	}
	#endregion
}
